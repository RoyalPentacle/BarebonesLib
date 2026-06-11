using Barebones.Asset;
using Barebones.DataStructures;
using Barebones.Drawable;
using Barebones.Drawable.Particles;
using Barebones.Network;
using Barebones.States;
using Barebones.Windows;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Net;

namespace Barebones
{
    /// <summary>
    /// Various core Engine functions and properties.
    /// </summary>
    public static class Engine
    {
        private static bool _isColourizing = false;
        private static Color _colourizeDestColour;

        private static Color _backbufferColour;

        /// <summary>
        /// The colour used for clearing the backbuffer.
        /// </summary>
        public static Color BackBufferColour
        {
            get { return _backbufferColour; }
        }


        private static ColorF _colourizeCurrentColour;
        private static ColorF _colourizeChangeOverTime;
        private static double _colourizeDuration;
        private static double _colourizeElapsedTime;

        private static void UpdateColour()
        {
            if (_isColourizing)
            {
                _colourizeCurrentColour += _colourizeChangeOverTime;
                _colourizeElapsedTime += Engine.GameTime.ElapsedGameTime.TotalMilliseconds;
                if (_colourizeElapsedTime >= _colourizeDuration)
                {
                    _isColourizing = false;
                    _backbufferColour = _colourizeDestColour;
                }
                else
                    _backbufferColour = _colourizeCurrentColour.GetColour;
            }
        }

        /// <summary>
        /// Sets the colour for clearing the backbuffer.
        /// </summary>
        /// <param name="colour">The new colour.</param>
        public static void SetBackbufferColour(Color colour)
        {
            _isColourizing = false;
            _backbufferColour = colour;
        }

        /// <summary>
        /// Change the backbuffer colour to the provided colour, over the provided milliseconds.
        /// </summary>
        /// <param name="colour">The colour to change to.</param>
        /// <param name="milliseconds">The milliseconds over which the change should occur.</param>
        public static void ColourizeBackbuffer(Color colour, float milliseconds)
        {
            _colourizeDestColour = colour;
            _colourizeDuration = milliseconds;
            _colourizeElapsedTime = 0;
            _colourizeCurrentColour = new ColorF(_backbufferColour);
            _colourizeChangeOverTime = ColorF.GetChangeOverTime(_backbufferColour, colour, milliseconds);
            _isColourizing = true;
        }


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

        private static Thread _mainThread;

        /// <summary>
        /// The main thread the game is running on.
        /// </summary>
        public static Thread MainThread
        {
            get { return _mainThread; }
        }

        /// <summary>
        /// Checks if the thread that called this property is the main thread or not.
        /// </summary>
        public static bool IsMainThread
        {
            get { return Thread.CurrentThread == _mainThread; }
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
        /// Set the maximum script cache size, in bytes.
        /// </summary>
        /// <param name="maxSize">The maximum size of the cache, in bytes.</param>
        public static void SetScriptCacheMaxSize(long maxSize)
        {
            _scriptCacheMaxSize = maxSize;
        }

        private static long _meshCacheMaxSize = 67108864L;


        /// <summary>
        /// The maximum size of the mesh cache, in bytes.
        /// </summary>
        public static long MeshCacheMaxSize
        {
            get { return _meshCacheMaxSize; }
        }


        /// <summary>
        /// Default: 65,108,864 bytes (64 megabytes)
        /// Set the maximum mesh cache size, in bytes.
        /// </summary>
        /// <param name="maxSize">The maximum size of the cache, in bytes.</param>
        public static void SetMeshCacheMaxSize(long maxSize)
        {
            _meshCacheMaxSize = maxSize;
        }

        private static NLua.Lua _luaState;
        
        /// <summary>
        /// The Shared Synchronous Lua State machine.
        /// </summary>
        public static NLua.Lua GlobalLua
        {
            get { return _luaState; }
        }

        private static float _particleMultiplier = 1.0f;

        /// <summary>
        /// Multiplies the number of spawned particles per ParticleGenerator spawn cycle by this number.
        /// Cannot be below 0.
        /// </summary>
        public static float ParticleMultiplier
        {
            get { return _particleMultiplier; }
            set { _particleMultiplier = Math.Max(0, value); }
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

        private static bool _targetGraphWrapSelection;

        /// <summary>
        /// Do TargetGraph objects wrap around when selection fails to find a target in a given direction?
        /// </summary>
        /// <remarks>
        /// That is, if there is no target on the left, do we instead select the furthest on the right?
        /// </remarks>
        public static bool TargetGraphWrapSelection
        {
            get { return _targetGraphWrapSelection; }
            set { _targetGraphWrapSelection = value; }
        }

        private static bool _isClosing = false;

        /// <summary>
        /// Is the game shutting down?
        /// </summary>
        public static bool IsClosing
        {
            get { return _isClosing; }
        }

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

        private static MouseState _oldMouseState;

        internal static MouseState OldMouseState
        {
            get { return _oldMouseState; }
        }

        private static MouseState _newMouseState;

        internal static MouseState NewMouseState
        {
            get { return _newMouseState; }
        }

        private static float _masterVolume = 1.0f;
        private static float _musicVolume = 1.0f;
        private static float _soundVolume = 1.0f;

        private static bool _masterMute = false;
        private static bool _musicMute = false;
        private static bool _soundMute = false;

        /// <summary>
        /// Mute all audio.
        /// </summary>
        public static bool MasterMute
        {
            get { return _masterMute; }
            set 
            {
                if (value)
                    SoundEffect.MasterVolume = 0f;
                else
                    SoundEffect.MasterVolume = _masterVolume;
                _masterMute = value; 
            }
        }

        /// <summary>
        /// Mute music.
        /// </summary>
        public static bool MusicMute
        {
            get { return _musicMute; }
            set { _musicMute = value; }
        }

        /// <summary>
        /// Mute sounds.
        /// </summary>
        public static bool SoundMute
        {
            get { return _soundMute; }
            set { _soundMute = value; }
        }

        /// <summary>
        /// Gets and sets the master volume for all sound effects and music.
        /// Clamped between 0.0f and 1.0f.
        /// Internally is just a wrapper for SoundEffect.MasterVolume.
        /// </summary>
        public static float MasterVolume
        {
            get 
            {
                if (!_masterMute)
                {
                    SoundEffect.MasterVolume = _masterVolume;
                    return _masterVolume;
                }
                else
                {
                    SoundEffect.MasterVolume = 0f;
                    return 0f;
                }
            }
            set 
            {
                float vol = value * 100;
                vol = (float)Math.Round(vol);
                vol /= 100f;
                _masterVolume = (float)Math.Clamp(vol, 0.0, 1.0);
                SoundEffect.MasterVolume = _masterVolume;
                _masterMute = false;
            }
        }

        /// <summary>
        /// Gets and sets the volume for all music.
        /// Clamped between 0.0f and 1.0f.
        /// </summary>
        public static float MusicVolume
        {
            get 
            { 
                if (!_musicMute)
                    return _musicVolume;
                else 
                    return 0f;
            }
            set 
            {
                float vol = value * 100;
                vol = (float)Math.Round(vol);
                vol /= 100f;
                _musicVolume = (float)Math.Clamp(vol, 0.0, 1.0);
                _musicMute = false;
            }
        }

        /// <summary>
        /// Gets and sets the volume for all sound effects.
        /// Clamped between 0.0f and 1.0f.
        /// </summary>
        public static float SoundVolume
        {
            get 
            { 
                if (!_soundMute)
                    return _soundVolume;
                else
                    return 0f;
            }
            set 
            {
                float vol = value * 100;
                vol = (float)Math.Round(vol);
                vol /= 100f;
                _soundVolume = (float)Math.Clamp(vol, 0.0, 1.0);
                _soundMute = false;
            }
        }

        /// <summary>
        /// The camera object for the game.
        /// </summary>
        public static Camera2D Camera
        {
            get { return Camera2D.Camera; }
        }

        private static bool _isEditor = false;

        /// <summary>
        /// Is the engine forced into editor mode?
        /// </summary>
        public static bool IsEditor
        {
            get { return _isEditor; }
        }

        /// <summary>
        /// Initialize the Barebones engine
        /// </summary>
        public static void Initialize()
        {
            _mainThread = Thread.CurrentThread;
            _spriteBatch = new SpriteBatch(_graphicsDevice.GraphicsDevice);
            Asset.Textures.Shared.Init();
            Asset.Sound.Shared.Init();
            Game.Window.TextInput += Config.Control.TextInputHandler;
            _luaState = new NLua.Lua();
            _luaState.LoadCLRPackage();
            Lua.Functions.RunScript(@"
                import('Barebones', 'Barebones.Lua')
                import('System.Threading')
                function Wait(ms)
                    Thread.Sleep(ms)
                end
                ");
            if (_isEditor)
                StateHandler.ChangeState(State.Select);
        }

        /// <summary>
        /// Executes engine logic that must be at the start of every tick.
        /// </summary>
        public static void PreUpdate(GameTime gameTime)
        {
            _gameTime = gameTime;
            _newKeyboardState = Keyboard.GetState();
            _newMouseState = Mouse.GetState();
            ParticleHandler.Update();
            WindowHandler.Update();
        }

        /// <summary>
        /// Executes engine logic to be executed instead of a games. I.E. The built in script editors.
        /// </summary>
        /// <remarks>Place game update logic in a negated if statement for this function.</remarks>
        /// <returns>True if we are overriding other logic, false otherwise.</returns>
        public static bool OverrideUpdate()
        {
            if (StateHandler.State == State.None)
                return false;
            else
            {
                StateHandler.Update();
                return true;
            }
        }

        /// <summary>
        /// Executes engine logic that must be done at the end of every tick.
        /// </summary>
        public static void PostUpdate()
        {
            UpdateColour();
            Connections.UpdateNetwork();
            Asset.Sound.DisposeStoppedInstances();
            Music.DisposeStoppedInstances();
            Camera.UpdateCamera();
            ParticleHandler.AwaitSystems();
            Textures.LoadAsyncQueue();
            Verbose.PrintConsoleOutput();
            ShowStatus();
            _oldKeyboardState = _newKeyboardState;
            _oldMouseState = _newMouseState;
        }

        /// <summary>
        /// Executes engine draw calls to be executed instead of a games. I.E. The built in script editors.
        /// </summary>
        /// <remarks>Place game draw logic in a negated if statement for this function.</remarks>
        /// <returns>True if we are overriding other draw logic, false otherwise.</returns>
        public static bool OverrideDraw()
        {
            if (StateHandler.State == State.None)
                return false;
            else
            {
                StateHandler.Draw();
                return true;
            }
        }

        /// <summary>
        /// When the game stops, call this to do any shutdown logic the engine may require.
        /// </summary>
        public static void Close()
        {
            _isClosing = true;
            Verbose.Close();
            ParticleHandler.Close();
        }

        /// <summary>
        /// Checks the launch arguments and sets the appropriate settings.
        /// </summary>
        /// <param name="args">The launch arguments, passed in from the system.</param>
        public static void PreInitialize(string[] args)
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
                                    if (!IPAddress.TryParse(splitArg[1], out IPAddress? address))
                                    {
                                            _defaultUDPHostAddress = IPAddress.Loopback;
                                    } 
                                    else
                                    {
                                        if (address == null)
                                            _defaultUDPHostAddress = IPAddress.Loopback;
                                        else
                                            _defaultUDPHostAddress = address;
                                    }
                                }
                                if (splitArg.Length > 2)
                                    if (!int.TryParse(splitArg[2], out _defaultUDPHostPort))
                                        _defaultUDPHostPort = 51234;
                                break;
                        }
                        case "bareboneseditor":
                            {
                                _isEditor = true;
                                break;
                            }
                    }
                }
            }

            // Set the console variables based on the arguments. Always do it in this order.
            Verbose.Initalize(errorMajor, errorMinor, logMajor, logMinor);
            Verbose.SetSaveConsole(saveOutput);
        }


        private static int _updateNum = 0;
        internal static void ShowStatus()
        {
            if (Verbose.ShowConsole)
            {
                _updateNum++;
                if (_updateNum >= 60)
                {
                    _updateNum = 0;
                    string status = $"Barebones - ScC: {ScriptFinder.CacheSize} TC: {Textures.CacheSize} SnC: {Sound.CacheSize} PS: {ParticleHandler.SystemCount} PP: {ParticleHandler.ParticleCount}";
                    Console.Title = status;
                }
            }
        }
    }
}
