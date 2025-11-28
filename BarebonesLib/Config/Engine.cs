using Barebones.Asset;
using Barebones.Drawable.Particles;
using Barebones.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NLua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Barebones.Config
{
    /// <summary>
    /// Various core Engine functions and properties.
    /// </summary>
    public static class Engine
    {

        internal const string LOGGING_PATH = "logs/";

        #region Global Properties

        private static SpriteBatch _spriteBatch;

        /// <summary>
        /// Global pointer to the SpriteBatch for drawing assets.
        /// </summary>
        public static SpriteBatch SpriteBatch
        {
            get { return _spriteBatch; }
        }


        private static GameTime _gameTime;

        /// <summary>
        /// Global pointer to the GameTime for timekeeping.
        /// </summary>
        public static GameTime GameTime
        {
            get { return _gameTime; }
        }


        private static GraphicsDeviceManager _graphicsDevice;

        /// <summary>
        /// Global pointer to the GraphicsDeviceManager for graphics settings.
        /// </summary>
        public static GraphicsDeviceManager Graphics
        {
            get { return _graphicsDevice; }
        }

        /// <summary>
        /// Sets the global pointer to the specified GraphicsDeviceManager.
        /// </summary>
        /// <param name="graphicsDevice">The GraphicsDeviceManager to point to.</param>
        public static void SetGraphics(GraphicsDeviceManager graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
        }

        private static Game _game;

        /// <summary>
        /// Global pointer to the running game itself.
        /// </summary>
        public static Game Game
        {
            get { return _game; }
        }

        /// <summary>
        /// Sets the global pointer to the running game.
        /// </summary>
        /// <param name="game"></param>
        public static void SetGame(Game game)
        {
            _game = game;
            SetGraphics(new GraphicsDeviceManager(game));
        }

        private static long _textureCacheMaxSize = 16777216L;

        /// <summary>
        /// The maximum size of the texture cache, in bytes.
        /// </summary>
        public static long TextureCacheMaxSize
        {
            get { return _textureCacheMaxSize; }
        }

        /// <summary>
        /// Default: 16,777,216 bytes (16 megabytes)
        /// Set the maximum texture cache size, in bytes.
        /// </summary>
        /// <param name="maxSize">The maximum size of the cache, in bytes.</param>
        public static void SetTextureCacheMaxSize(long maxSize)
        {
            _textureCacheMaxSize = maxSize;
        }


        private static long _soundCacheMaxSize = 16777216L;

        /// <summary>
        /// The maximum size of the sound cache, in bytes.
        /// </summary>
        public static long SoundCacheMaxSize
        {
            get { return _soundCacheMaxSize; }
        }

        /// <summary>
        /// Default: 16,777,216 bytes (16 megabytes)
        /// Set the maximum sound cache size, in bytes.
        /// </summary>
        /// <param name="maxSize">The maximum size of the cache, in bytes.</param>
        public static void SetSoundCacheMaxSize(long maxSize)
        {
            _soundCacheMaxSize = maxSize;
        }


        private static long _scriptCacheMaxSize = 1048576L;

        /// <summary>
        /// The maximum size of the script cache, in bytes.
        /// </summary>
        public static long ScriptCacheMaxSize
        {
            get { return _scriptCacheMaxSize; }
        }

        /// <summary>
        /// Default: 1,048,576 bytes (1 megabyte)
        /// Set the maximum script cache seize, in bytes.
        /// </summary>
        /// <param name="maxSize">The maximum size of the cache, in bytes.</param>
        public static void SetScriptCacheMaxSize(long maxSize)
        {
            _scriptCacheMaxSize = maxSize;
        }

        private static NLua.Lua _luaState;
        
        /// <summary>
        /// The Lua State machine.
        /// </summary>
        public static NLua.Lua LuaState
        {
            get { return _luaState; }
        }

        private static int _defaultUDPHostPort = 51234;

        /// <summary>
        /// The port the engine will try to host on, if no port is specified when called.
        /// </summary>
        public static int UDPHostPort
        {
            get { return _defaultUDPHostPort; }
        }

        private static IPAddress _defaultUDPHostAddress = IPAddress.Loopback;

        /// <summary>
        /// The address the engine will try to send packets to, if no port is specified when called.
        /// </summary>
        public static IPAddress UDPHostAddress
        {
            get { return _defaultUDPHostAddress; }
        }

        private static long _timeoutDuration = 5000;

        /// <summary>
        /// How long should the netcode wait to receive a packet from a client, before asking if they're still there?
        /// </summary>
        public static long NetworkTimeoutDuration
        {
            get { return _timeoutDuration; }
            set { _timeoutDuration = value; }
        }

        private static int _timeoutRetries = 5;

        /// <summary>
        /// How many times should the netcode attempt to ask a client if they're still alive?
        /// </summary>
        public static int TimeoutMaxRetries
        {
            get { return _timeoutRetries; }
            set { _timeoutRetries = value; }
        }

        #endregion



        private static KeyboardState _oldKeyboardState;

        internal static KeyboardState OldKeyboardState
        {
            get { return _oldKeyboardState; }
        }

        private static KeyboardState _newKeyboardState;

        internal static KeyboardState NewKeyboardState
        {
            get { return _newKeyboardState; }
        }



        /// <summary>
        /// Initialize the Barebones engine for the given 
        /// </summary>
        public static void Initialize()
        {
            _spriteBatch = new SpriteBatch(_graphicsDevice.GraphicsDevice);
            Asset.Textures.Shared.Init();
            Asset.Sound.Shared.Init();
            _luaState = new NLua.Lua();
            _luaState.LoadCLRPackage();
            Lua.Script.RunScript(@"
                import('Barebones', 'Barebones.Lua')
                import('System.Threading')
                function Wait(ms)
                    Thread.Sleep(ms)
                end
                ");
        }

        /// <summary>
        /// Executes engine logic that must be at the start of every tick.
        /// </summary>
        public static void PreUpdate(GameTime gameTime)
        {
            _gameTime = gameTime;
            _newKeyboardState = Keyboard.GetState();
            ParticleHandler.Update();
        }

        /// <summary>
        /// Executes engine logic that must be done at the end of every tick.
        /// </summary>
        public static void PostUpdate()
        {
            Connections.UpdateNetwork();
            Asset.Sound.DisposeStoppedInstances();
            ParticleHandler.AwaitSystems();
            ShowStatus();
            _oldKeyboardState = _newKeyboardState;
        }

        /// <summary>
        /// When the game stops, call this to do any shutdown logic the engine may require.
        /// </summary>
        public static void Close()
        {
            Verbose.CloseFilestream();
        }

        /// <summary>
        /// Checks the launch arguments and sets the appropriate settings.
        /// </summary>
        /// <param name="args">The launch arguments, passed in from the system.</param>
        internal static void CheckLaunchArguments(string[] args)
        {
            // Default all the console outputs to false,
            bool errorMajor = false;
            bool errorMinor = false;
            bool logMajor = false;
            bool logMinor = false;
            bool saveOutput = false;
            // If we have launch arguments, get to work.
            if (args.Length > 0)
            {
                string reformattedArgs = "";
                for (int i = 0; i < args.Length; i++)
                {
                    reformattedArgs += args[i] + " ";
                }
                string[] formattedArgs = reformattedArgs.Split('-');
                foreach (string arg in formattedArgs)
                {
                    string lowerCase = arg.ToLower();
                    string[] splitArg = lowerCase.Split(' '); // E.G. split '-console ErrorMinor LogMajor
                    switch (splitArg[0])
                    {
                        // If the console should be enabled, then check for more args for how verbose we're going to be.
                        case "console":
                        {
                            if (splitArg.Length > 1)
                            {
                                for (int i = 1; i < splitArg.Length; i++)
                                {
                                    switch (splitArg[i])
                                    {
                                        case "errormajor":
                                        {
                                            errorMajor = true;
                                            break;
                                        }
                                        case "errorminor":
                                        {
                                            errorMinor = true;
                                            errorMajor = true;
                                            break;
                                        }
                                        case "logmajor":
                                        {
                                            logMajor = true;
                                            break;
                                        }
                                        case "logminor":
                                        {
                                            logMinor = true;
                                            logMajor = true;
                                            break;
                                        }
                                    }
                                }
                            }
                            else // If we're not given any specified verboseness, just output everything.
                            {
                                errorMajor = true;
                                errorMinor = true;
                                logMajor = true;
                                logMinor = true;
                            }
                            break;
                        }
                        case "saveconsoleoutput":
                        {
                            saveOutput = true;
                            break;
                        }
                        case "ip":
                        {
                            if (splitArg.Length > 1)
                                {
                                    if (!IPAddress.TryParse(splitArg[1], out _defaultUDPHostAddress))
                                        _defaultUDPHostAddress = IPAddress.Loopback;
                                }
                                if (splitArg.Length > 2)
                                    if (!int.TryParse(splitArg[2], out _defaultUDPHostPort))
                                        _defaultUDPHostPort = 51234;
                                break;
                        }
                    }
                }
            }

            // Set the console variables based on the arguments. Always do it in this order.
            Verbose.SetConsoleOutputs(errorMajor, errorMinor, logMajor, logMinor);
            Verbose.SetSaveConsole(saveOutput);
        }


        private static int _updateNum = 0;
        private static double _updateTime = 0;

        internal static void ShowStatus()
        {
            if (Verbose.ShowConsole)
            {
                _updateNum++;
                _updateTime += GameTime.ElapsedGameTime.TotalMilliseconds;
                if (_updateNum >= 60)
                {
                    _updateNum = 0;
                    float avg = (float)Math.Round((1000.0 / _updateTime) * 60.0, 1);
                    _updateTime = 0.0;
                    string status = $"{Game.Window.Title} - FPS: {avg} ScC: {ScriptFinder.CacheSize} TC: {Textures.CacheSize} SnC: {Sound.CacheSize} PS: {ParticleHandler.SystemCount} PP: {ParticleHandler.ParticleCount}";
                    Console.Title = status;
                }
            }
        }
    }
}
