using Barebones.Config;
using NVorbis.Contracts;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Barebones.Network
{
    /// <summary>
    /// Contains the data received from the network, and the remote endpoint it came from.
    /// </summary>
    public struct Packet
    {
        private byte[] _data;
        private IPEndPoint _endpoint;

        /// <summary>
        /// The data contained within the received packet.
        /// </summary>
        public byte[] Data
        {
            get { return _data; }
        }

        /// <summary>
        /// The remote endpoint the packet was received from.
        /// </summary>
        public IPEndPoint EndPoint
        {
            get { return _endpoint; }
        }

        /// <summary>
        /// The IP Address of the remote endpoint.
        /// Functionally this is EndPoint.Address, just exposed here for ease of access.
        /// </summary>
        public IPAddress Address
        {
            get { return _endpoint.Address; }
        }

        /// <summary>
        /// The Port of the remote endpoint.
        /// Functionally this is EndPoint.Port, just exposed here for ease of access.
        /// </summary>
        public int Port
        {
            get { return _endpoint.Port; }
        }

        /// <summary>
        /// Construct a new packet out of a UdpReceiveResult.
        /// </summary>
        /// <param name="result">The UdpReceiveResult to convert .</param>
        public Packet(UdpReceiveResult result)
        {
            _data = result.Buffer;
            _endpoint = result.RemoteEndPoint;
        }

        /// <summary>
        /// Construct a new packet by getting a provided byte array and IPEndPoint.
        /// I don't know why you'd want to do this, but you can.
        /// </summary>
        /// <param name="data">The byte array to copy.</param>
        /// <param name="endpoint">The IPEndPoint.</param>
        public Packet(byte[] data, IPEndPoint endpoint)
        {
            _data = data;
            _endpoint = endpoint;
        }
    }

    /// <summary>
    /// This class contains functions and properties for sending and receiving packets in both UDP and TCP.
    /// </summary>
    public static class Connections
    {
        private class HeartbeatTimer
        {
            private long _time = 0;
            private readonly Stopwatch _stopwatch = Stopwatch.StartNew();
            private int _retryCount = 0;
            private byte _id;

            public HeartbeatTimer(byte id)
            {
                _id = id;
            }

            public void Update()
            {
                Interlocked.Exchange(ref _time, _stopwatch.ElapsedMilliseconds);
                if (_time > Engine.NetworkTimeoutDuration)
                {
                    SendHeartbeatRequestPacket(_id);
                    IncrementRetry();
                    Verbose.WriteLogMinor($"Sent Heartbeat request to client {_id}. Attempt: {_retryCount}/{Engine.TimeoutMaxRetries}");
                    if (_retryCount > Engine.TimeoutMaxRetries)
                    {
                        Verbose.WriteLogMinor($"Enqueueing client {_id} for removal.");
                        _clientsToRemoveTimeout.Enqueue(_id);
                    }
                }
            }

            private void IncrementRetry()
            {
                Interlocked.Increment(ref _retryCount);
                _stopwatch.Restart();
            }
            
            public void Reset()
            {
                Interlocked.Exchange(ref _retryCount, 0);
                _stopwatch.Restart();
            }
        }

        private static UdpClient? _udpClient;
        private static byte _clientID;
        private static bool _isHost;
        private static bool _isOnline;

        private readonly static BijectiveConcurrentDictionary<string, byte> _packetTypes = new();
        private readonly static Dictionary<byte, Action<Packet>> _typeActions = new();

        private readonly static BijectiveConcurrentDictionary<IPEndPoint, byte> _clientDict = new();
        private readonly static ConcurrentDictionary<byte, HeartbeatTimer> _clientTimeoutDict = new();

        private readonly static ConcurrentQueue<byte> _clientsToRemoveTimeout = new ConcurrentQueue<byte>();
        private readonly static ConcurrentQueue<byte> _clientsToRemoveNormal = new ConcurrentQueue<byte>();

        private static Func<Packet, bool>? _routeToGameConnectionRequest;
        private static Action<Packet>? _routeToGamePostConnectionRequest;
        private static Func<Packet, bool>? _routeToGameConnectionAcknowledge;
        private static Action<Packet>? _routeToGamePostConnectionAcknowledge;
        private static Func<Packet, bool>? _routeToGameDisconnectRequest;
        private static Action<Packet>? _routeToGamePostDisconnectRequest;
        private static Func<Packet, bool>? _routeToGameDisconnectAcknowledge;
        private static Action<Packet>? _routeToGamePostDisconnectAcknowledge;
        private static Func<Packet, bool>? _routeToGameClientDisconnectAlert;
        private static Action<Packet>? _routeToGamePostClientDisconnectAlert;
        private static Action<byte>? _routeToGameClientTimeout;


        private static CancellationTokenSource _cts = new CancellationTokenSource();

        private static Mutex _mut = new Mutex();

        /// <summary>
        /// The ID assigned to this instance.
        /// </summary>
        public static byte ClientID
        {
            get { return _clientID; }
        }

        /// <summary>
        /// Is this instance the host?
        /// </summary>
        public static bool IsHost
        {
            get { return _isHost; }
        }

        /// <summary>
        /// Is this instance online?
        /// </summary>
        public static bool IsOnline
        {
            get { return _isOnline; }
        }

        /// <summary>
        /// A dictionary containing IPEndpoints and the client IDs associated with those endpoint.
        /// </summary>
        public static BijectiveConcurrentDictionary<IPEndPoint, byte> ClientDict
        {
            get { return _clientDict; }
        }

        /// <summary>
        /// A dictionary containing packet type names, and the byte value associated with them.
        /// </summary>
        public static BijectiveConcurrentDictionary<string, byte> PacketType
        {
            get { return _packetTypes; }
        }

        static Connections()
        {
            RegisterPacketType("HeartbeatRequest", 0, ReceiveHeartbeatRequestPacket);
            RegisterPacketType("HeartbeatAcknowledge", 1, ReceiveHeartbeatAcknowledgePacket);
            RegisterPacketType("ConnectionRequest", 2, ReceiveConnectionRequestPacket);
            RegisterPacketType("ConnectionAcknowledge", 3, ReceiveConnectionAcknowledgePacket);
            RegisterPacketType("DisconnectRequest", 4, ReceiveDisconnectRequestPacket);
            RegisterPacketType("DisconnectAcknowledge", 5, ReceiveDisconnectAcknowledgePacket);
            RegisterPacketType("ClientDisconnectAlert", 6, ReceiveClientDisconnectAlert);
        }

        /// <summary>
        /// Register new packet types with the UDP handler.
        /// Currently you can not have more than 256 packet types.
        /// </summary>
        /// <param name="name">The name of the packet type.</param>
        /// <param name="value">The byte value associated.</param>
        /// <param name="action">The method to call when this type is received.</param>
        /// <remarks>
        /// Pre-set Types:
        /// HeartbeatRequest = 0,
        /// HeartbeatAcknowledge = 1,
        /// ConnectionRequest = 2,
        /// ConnectionAcknowledge = 3,
        /// DisconnectRequest = 4,
        /// DisconnectAcknowledge = 5
        /// </remarks>
        public static void RegisterPacketType(string name, byte value, Action<Packet> action)
        {
            _packetTypes.Add(name, value);
            _typeActions.Add(value, action);
        }

        private static bool AddClient(IPEndPoint endPoint, byte clientID)
        {

            if (_clientDict.TryAdd(endPoint, clientID) && _clientTimeoutDict.TryAdd(clientID, new HeartbeatTimer(clientID)))
            {
                Verbose.WriteLogMajor($"Added client: {endPoint} with ID {clientID}.");
                return true;
            }
            else
            {
                _clientDict.Remove(endPoint);
                _clientTimeoutDict.Remove(clientID, out _);
                Verbose.WriteErrorMinor($"Failed to add client: {endPoint} with ID {clientID}. Likely an endpoint shared with another client, or too many client connected clients. Client Count: {_clientDict.Count}");
                return false;
            }

        }
        private static byte GetUnusedClientID()
        {
            for (byte i = 1; i <= 255; i++)
            {
                if (!_clientDict.Contains(i))
                {
                    return i;
                }
            }
            return 255;
        }

        /// <summary>
        /// Set the function to be called when a connection request packet is received.
        /// It should return true if we can go ahead with the connection, or false otherwise.
        /// If this is null, we always assume true.
        /// </summary>
        /// <param name="callback">The function to call.</param>
        public static void SetConnectionRequestFunc(Func<Packet, bool> callback)
        {
            _routeToGameConnectionRequest = callback;
        }

        /// <summary>
        /// Set the function to be called after a connection request packet has been received and processed.
        /// If this is null, we do nothing.
        /// </summary>
        /// <param name="callback">The function to call.</param>
        public static void SetPostConnectionRequestAction(Action<Packet> callback)
        {
            _routeToGamePostConnectionRequest = callback;
        }

        /// <summary>
        /// Set the function to be called when a connection acknowledge packet is received.
        /// It should return true if we can go ahead with the connection, or false otherwise.
        /// If this is null, we always assume true.
        /// </summary>
        /// <param name="callback">The function to call.</param>
        public static void SetConnectionAcknowledgeFunc(Func<Packet, bool> callback)
        {
            _routeToGameConnectionAcknowledge = callback;
        }

        /// <summary>
        /// Set the function to be called after a connection acknowledge packet has been received and processed.
        /// If this is null, we do nothing.
        /// </summary>
        /// <param name="callback">The function to call.</param>
        public static void SetPostConnectionAcknowledgeAction(Action<Packet> callback)
        {
            _routeToGamePostConnectionAcknowledge = callback;
        }

        /// <summary>
        /// Set the function to be called when a Disconnect request packet is received.
        /// It should return true if we can go ahead with ending the connection, or false otherwise.
        /// If this is null, we always assume true.
        /// </summary>
        /// <param name="callback">The function to call.</param>
        public static void SetDisconnectRequestFunc(Func<Packet, bool> callback)
        {
            _routeToGameDisconnectRequest = callback;
        }

        /// <summary>
        /// Set the function to be called after a disconnect request packet has been received and processed.
        /// If this is null, we do nothing.
        /// </summary>
        /// <param name="callback">The function to call.</param>
        public static void SetPostDisconnectRequestAction(Action<Packet> callback)
        {
            _routeToGamePostDisconnectRequest = callback;
        }

        /// <summary>
        /// Set the function to be called when a disconnect acknowledge packet is received.
        /// It should return true if we can go ahead with ending the connection, or false otherwise.
        /// If this is null, we always assume true.
        /// </summary>
        /// <param name="callback">The function to call.</param>
        public static void SetDisconnectAcknowledgeFunc(Func<Packet, bool> callback)
        {
            _routeToGameDisconnectAcknowledge = callback;
        }

        /// <summary>
        /// Set the function to be called after a disconnect acknowledge packet has been received and processed.
        /// If this is null, we do nothing.
        /// </summary>
        /// <param name="callback">The function to call.</param>
        public static void SetPostDisconnectAcknowledgeAction(Action<Packet> callback)
        {
            _routeToGamePostDisconnectAcknowledge = callback;
        }

        /// <summary>
        /// Set the function to be called after a client has timed out.
        /// If this is null, we do nothing.
        /// </summary>
        /// <param name="callback">The function to call.</param>
        public static void SetClientTimeoutAction(Action<byte> callback)
        {
            _routeToGameClientTimeout = callback;
        }

        /// <summary>
        /// Set the function to be called when a client disconnect alert packet is received.
        /// It should return true if we can go ahead, or false otherwise.
        /// If this is null, we always assume true.
        /// </summary>
        /// <param name="callback">The function to call.</param>
        public static void SetClientDisconnectAlertFunc(Func<Packet, bool> callback)
        {
            _routeToGameClientDisconnectAlert = callback;
        }

        /// <summary>
        /// Set the function to be called after a client disconnect alert packet has been received and processed.
        /// If this is null, we do nothing.
        /// </summary>
        /// <param name="callback">The function to call.</param>
        public static void SetPostClientDisconnectAlertAction(Action<Packet> callback)
        {
            _routeToGamePostClientDisconnectAlert = callback;
        }

        private static void StartUDP(int port)
        {
            if (_udpClient == null)
            {
                _udpClient = new UdpClient(port);
                uint IOC_IN = 0x80000000;
                uint IOC_VENDOR = 0x18000000;
                uint SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12;
                _udpClient.Client.IOControl((int)SIO_UDP_CONNRESET, [Convert.ToByte(false)], null);
            }
            bool connReq = _routeToGameConnectionRequest != null;
            bool postConnReq = _routeToGamePostConnectionRequest != null;
            bool connAck = _routeToGameConnectionAcknowledge != null;
            bool postConnAck = _routeToGamePostConnectionAcknowledge != null;
            bool discReq = _routeToGameDisconnectRequest != null;
            bool postDiscReq = _routeToGamePostDisconnectRequest != null;
            bool discAck = _routeToGameDisconnectAcknowledge != null;
            bool postDiscAck = _routeToGamePostDisconnectAcknowledge != null;
            bool clientDiscAlert = _routeToGameClientDisconnectAlert != null;
            bool postClientDiscAlert = _routeToGamePostClientDisconnectAlert != null;
            bool clientTimeout = _routeToGameClientTimeout != null;
            bool unboundDelegate = connReq && postConnReq && connAck && postConnAck && discReq && postDiscReq && discAck && postDiscAck && clientDiscAlert && postClientDiscAlert && clientTimeout;
            if (!unboundDelegate)
            {
                string msg = "";
                if (!connReq)
                    msg += "routeToGameConnectionRequest is unbound.\n";
                if (!postConnReq)
                    msg += "routeToGamePostConnectionRequest is unbound.\n";
                if (!connAck)
                    msg += "routeToGameConnectionAcknowledge is unbound.\n";
                if (!postConnAck)
                    msg += "routeToGamePostConnectionAcknowledge is unbound.\n";
                if (!discReq)
                    msg += "routeToGameDisconnectRequest is unbound.\n";
                if (!postDiscReq)
                    msg += "routeToGamePostDisconnectRequest is unbound.\n";
                if (!discAck)
                    msg += "routeToGameDisconnectAcknowledge is unbound.\n";
                if (!postDiscAck)
                    msg += "routeToGamePostDisconnectAcknowledge is unbound.\n";
                if (!clientDiscAlert)
                    msg += "routeToGameClientDisconnectAlert is unbound.\n";
                if (!postClientDiscAlert)
                    msg += "routeToGamePostClientDisconnectAlert is unbound.\n";
                if (!clientTimeout)
                    msg += "routeToGameClientTimeout is unbound.\n";

                Verbose.WriteLogMinor($"Delegate Alert: \n{msg}");
                Verbose.WriteLogMinor($"This is probably not a problem. This is for debug purposes.");
            }
            Task.Factory.StartNew(ReceiveUDPAsync);
            Task.Run(DoClientTimeout);
        }

        /// <summary>
        /// Create a UDP client and start listening for packets.
        /// </summary>
        /// <param name="isHost">Is this instance the host of a server?</param>
        /// <remarks>
        /// Specifically, if true, this will create a UDP client listening on the port defined by <see cref="Engine.UDPHostPort"/>.
        /// If false, it creates a UDP client listening on port 0, causing the OS to provide a free port.
        /// </remarks>
        public static void StartUDP(bool isHost)
        {
            _isHost = isHost;
            _isOnline = true;
            if (isHost)
                StartUDP(Engine.UDPHostPort);
            else
                StartUDP(0);


        }

        /// As time ticks on, we track how long it's been since we've heard from a client on the main thread.
        /// When the time is greater than the max duration, ask for a heartbeat.
        /// If we don't receive any packets, retry again, up the max number of times.
        /// Then, remove the client.
        /// But, Concurrence.
        /// The checks for timeout happens on the main thread
        /// Simultaeneously, there is the UDP thread that is checking and possibly modifying the collection of clients
        /// So we have to be very careful about how we do this, so that it's threadsafe on both ends.

        internal static void UpdateNetwork()
        {
            while (_clientsToRemoveTimeout.TryDequeue(out var id))
            {
                _mut.WaitOne();
                RemoveClient(id, true, "Connection Timed Out.");
                _mut.ReleaseMutex();
            }
            while (_clientsToRemoveNormal.TryDequeue(out var id))
            {
                _mut.WaitOne();
                RemoveClient(id, false, "Requested Disconnect.");
                _mut.ReleaseMutex();
            }
            
        }

        private static async Task DoClientTimeout()
        {
            while (!_cts.IsCancellationRequested)
            {
                await Task.Delay(1000);
                List<HeartbeatTimer> timers = _clientTimeoutDict.Values.ToList();
                foreach (HeartbeatTimer timer in timers)
                {
                    timer.Update();
                }
            }
        }

        

        private static async void ReceiveUDPAsync()
        {
            try
            {
                while (!_cts.IsCancellationRequested)
                {
                    try
                    {
                        UdpReceiveResult receivedResult = await _udpClient.ReceiveAsync(_cts.Token);
                        Packet packet = new Packet(receivedResult);

                        if (packet.Data.Length > 0)
                        {
                            if (_typeActions.ContainsKey(packet.Data[0]))
                            {
                                _mut.WaitOne();
                                if (_clientDict.TryGetValue(packet.EndPoint, out byte id))
                                {
                                    _clientTimeoutDict[id].Reset();
                                }
                                _mut.ReleaseMutex();
                                _typeActions[packet.Data[0]](packet);
                            }
                            else
                                Verbose.WriteErrorMinor($"Unknown Packet Type Received! Byte value: {packet.Data[0]}");
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        Verbose.WriteLogMinor($"UDP Client cancelled.");
                    }
                    catch (Exception e)
                    {
                        Verbose.WriteErrorMajor($"{e.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Verbose.WriteErrorMajor($"Network Error! A problem with the UDP client has occured! \n {ex.Message}");
            }
            finally
            {
                CloseUDPClient();
            }
        }

        private static void CloseUDPClient()
        {
            _isOnline = false;
            _isHost = false;
            try
            {
                _udpClient?.Close();
            }
            catch (SocketException ex)
            {
                Verbose.WriteErrorMinor($"Error closing the UDP socket? \n {ex.Message}");
            }
            _udpClient?.Dispose();
            _udpClient = null;
            _clientDict.Clear();
            _clientTimeoutDict.Clear();
            _clientsToRemoveTimeout.Clear();
            _clientsToRemoveNormal.Clear();
            _cts.TryReset();
        }

        /// <summary>
        /// Closes the current UDP connection and stops listening, disposing the client.
        /// </summary>
        public static void CloseUDP()
        {
            _cts.Cancel(true);
        }

        /// <summary>
        /// Send a UDP packet to all clients.
        /// Will prepend packet type to packet.
        /// </summary>
        /// <param name="packet">The packet to send.</param>
        /// <param name="type">The type of packet.</param>
        public static void SendUDPPacket(byte[] packet, string type)
        {
            byte b = _packetTypes[type];
            SendUDPPacket(packet, b);
        }

        /// <summary>
        /// Send a UDP packet to all clients.
        /// Will prepend packet type to packet.
        /// </summary>
        /// <param name="packet">The packet to send.</param>
        /// <param name="type">The type of packet.</param>
        public static void SendUDPPacket(byte[] packet, byte type)
        {
            byte[] dataToSend = new byte[packet.Length + 1];
            Array.Copy(packet, 0, dataToSend, 1, packet.Length);
            dataToSend[0] = type;
            SendUDPPacket(dataToSend);
        }

        /// <summary>
        /// Send a UDP packet to all clients.
        /// Does not modify packet.
        /// </summary>
        /// <param name="packet">The packet to send.</param>
        public static void SendUDPPacket(byte[] packet)
        {
            IEnumerator<KeyValuePair<IPEndPoint, byte>> enumerator = _clientDict.GetEnumerator();
            while (enumerator.MoveNext())
                SendUDPPacket(packet, enumerator.Current.Key);
        }

        /// <summary>
        /// Send a UDP packet to a specific endpoint.
        /// Will prepend packet type to packet.
        /// </summary>
        /// <param name="packet">The packet to send.</param>
        /// <param name="type">The type of packet.</param>
        /// <param name="endPoint">The endpoint to send the packet to.</param>
        public static void SendUDPPacket(byte[] packet, string type, IPEndPoint endPoint)
        {
            byte b = _packetTypes[type];
            SendUDPPacket(packet, b, endPoint);
        }

        /// <summary>
        /// Send a UDP packet to a specific endpoint.
        /// Will prepend packet type to packet.
        /// </summary>
        /// <param name="packet">The packet to send.</param>
        /// <param name="type">The type of packet.</param>
        /// <param name="endPoint">The endpoint to send the packet to.</param>
        public static void SendUDPPacket(byte[] packet, byte type, IPEndPoint endPoint)
        {
            byte[] dataToSend = new byte[packet.Length + 1];
            Array.Copy(packet, 0, dataToSend, 1, packet.Length);
            dataToSend[0] = type;
            SendUDPPacket(dataToSend, endPoint);
        }

        /// <summary>
        /// Send a UDP packet to a specific endpoint.
        /// Does not modify packet.
        /// </summary>
        /// <param name="packet">The packet to send.</param>
        /// <param name="endPoint">The endpoint to send the packet to.</param>
        public static void SendUDPPacket(byte[] packet, IPEndPoint endPoint)
        {
            _udpClient?.SendAsync(packet, endPoint);
        }


        private static void ReceiveHeartbeatRequestPacket(Packet packet)
        {
            Verbose.WriteLogMinor($"Received Heartbeat Request from {packet.EndPoint}. Sent back an acknowledge.");
            SendHeartbeatAcknowledgePacket(packet.EndPoint);
        }

        private static void SendHeartbeatRequestPacket(byte id)
        {
            byte[] buffer = [id];
            if (_clientDict.TryGetKey(id, out IPEndPoint ip))
            {
                SendUDPPacket(buffer, "HeartbeatRequest", ip);
            }
            else
            {
                Verbose.WriteErrorMinor($"Heartbeat Error! Tried to send Heartbeat Request but couldn't get the endpoint to send to! Client ID: {id}");
            }
        }

        private static void ReceiveHeartbeatAcknowledgePacket(Packet packet)
        {
            // I don't think we really need to do anything here, but maybe? The act of receiving the packet at all resets the timeout.
        }

        private static void SendHeartbeatAcknowledgePacket(IPEndPoint ip)
        {
            byte[] buffer = [_clientID];
            SendUDPPacket(buffer, "HeartbeatAcknowledge", ip);
            Verbose.WriteLogMinor($"Sent back a heartbeat acknowledge to {ip}.");
        }


        /// <summary>
        /// Send a UDP packet to a specified IPEndpoint
        /// </summary>
        /// <param name="packet"></param>
        private static void ReceiveConnectionRequestPacket(Packet packet)
        {
            if (_routeToGameConnectionRequest == null || _routeToGameConnectionRequest(packet))
            {
                byte id = GetUnusedClientID();
                _mut.WaitOne();
                if (AddClient(packet.EndPoint, id))
                {
                    byte[] buffer = [id];
                    SendConnectionAcknowledgePacket(buffer, packet.EndPoint);
                    if (_routeToGamePostConnectionRequest != null)
                        _routeToGamePostConnectionRequest(packet);
                }
                _mut.ReleaseMutex();
            }
        }

        /// <summary>
        /// Send a connection request packet to the specified address and port.
        /// </summary>
        /// <param name="address">The address to send to.</param>
        /// <param name="port">The port to send to.</param>
        public static void SendConnectionRequestPacket(IPAddress address, int port)
        {
            byte[] packet = new byte[0]; // This is an empty packet for a request, possibly transmit other data in the request in the future? Maybe allow overriding for transmiting other identifying info, steam ID etc?
            SendUDPPacket(packet, "ConnectionRequest", new IPEndPoint(address, port));
        }

        /// <summary>
        /// Send a connection request packet to the specified address, using the default port.
        /// </summary>
        /// <param name="address">The address to send to.</param>
        public static void SendConnectionRequestPacket(IPAddress address)
        {
            SendConnectionRequestPacket(address, Engine.UDPHostPort);
        }

        /// <summary>
        /// Send a connection request packet to the default address and default port.
        /// </summary>
        public static void SendConnectionRequestPacket()
        {
            SendConnectionRequestPacket(Engine.UDPHostAddress, Engine.UDPHostPort);
        }



        private static void ReceiveConnectionAcknowledgePacket(Packet packet)
        {
            if (_routeToGameConnectionAcknowledge == null || _routeToGameConnectionAcknowledge(packet))
            {
                if (packet.Data.Length > 1)
                {
                    _mut.WaitOne();
                    if (AddClient(packet.EndPoint, 0))
                    {
                        _clientID = packet.Data[1];
                        if (_routeToGamePostConnectionAcknowledge != null)
                            _routeToGamePostConnectionAcknowledge(packet);
                    }
                    _mut.ReleaseMutex();
                }
            }
        }

        private static void SendConnectionAcknowledgePacket(byte[] packet, IPEndPoint endpoint)
        {
            SendUDPPacket(packet, "ConnectionAcknowledge", endpoint);
        }

        /// <summary>
        /// Sends a disconnect request to all connected clients.
        /// </summary>
        public static void SendDisconnectRequestPacket()
        {
            byte[] buffer = [];
            SendUDPPacket(buffer, "DisconnectRequest");
        }

        private static void ReceiveDisconnectRequestPacket(Packet packet)
        {
            if (_routeToGameDisconnectRequest == null || _routeToGameDisconnectRequest(packet))
            {
                _mut.WaitOne();
                if (_clientDict.Contains(packet.EndPoint))
                {
                    byte id = _clientDict[packet.EndPoint];
                    _clientsToRemoveNormal.Enqueue(id);
                    SendDisconnectAcknowledgePacket(packet.EndPoint);
                    if (_routeToGamePostDisconnectRequest != null)
                    {
                        _routeToGamePostDisconnectRequest(packet);
                    }
                }
                _mut.ReleaseMutex();
            }
        }

        private static void SendDisconnectAcknowledgePacket(IPEndPoint endpoint)
        {
            byte[] buffer = [];
            SendUDPPacket(buffer, "DisconnectAcknowledge", endpoint);
        }

        private static void ReceiveDisconnectAcknowledgePacket(Packet packet)
        {
            if (_routeToGameDisconnectAcknowledge == null || _routeToGameDisconnectAcknowledge(packet))
            {
                _mut.WaitOne();
                if (_clientDict.Contains(packet.EndPoint))
                {
                    byte id = _clientDict[packet.EndPoint];
                    _clientsToRemoveNormal.Enqueue(id);
                    if (_routeToGamePostDisconnectAcknowledge != null)
                    {
                        _routeToGamePostDisconnectAcknowledge(packet);
                    }
                }
                _mut.ReleaseMutex();
            }
        }

        /// <summary>
        /// Inform all connected clients that a specified client has disconnected.
        /// </summary>
        /// <param name="id">The ID of the client to disconnect.</param>
        public static void SendClientDisconnectAlert(byte id)
        {
            byte[] buffer = [id];
            SendUDPPacket(buffer, "ClientDisconnectAlert");
            Verbose.WriteLogMinor($"Sending disconnect alert for client {id}");        }

        private static void ReceiveClientDisconnectAlert(Packet packet)
        {
            if (_routeToGameClientDisconnectAlert == null || _routeToGameClientDisconnectAlert(packet))
            {
                if (packet.Data.Length > 1)
                {
                    _mut.WaitOne();
                    if (_clientDict.Contains(packet.Data[1]))
                    {
                        _clientsToRemoveNormal.Enqueue(packet.Data[1]);
                    }
                    if (_routeToGamePostClientDisconnectAlert != null)
                    {
                        _routeToGamePostClientDisconnectAlert(packet);
                    }
                    _mut.ReleaseMutex();
                }
            }
        }

        internal static void RemoveClient(byte id, bool timeout, string? reason)
        {
            _clientDict.Remove(id);
            _clientTimeoutDict.Remove(id, out _);
            if (timeout)
            {
                if(_routeToGameClientTimeout != null)
                {
                    _routeToGameClientTimeout(id);
                }
            }
            Verbose.WriteLogMinor($"Client {id} disconnected. Reason: {reason}");
        }
    }
}
