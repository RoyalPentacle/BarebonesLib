using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NLua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barebones.Config
{
    public static class Engine
    {

        public const string LOGGING_PATH = "logs/";



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

        /// <summary>
        /// Sets the global pointer to the specified Gametime.
        /// </summary>
        /// <param name="gameTime">The GameTime to point to.</param>
        public static void SetGameTime(GameTime gameTime)
        {
            _gameTime = gameTime;
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
            _luaState.DoString(@"
                import('Barebones.Lua')
                import('System.Threading')
                function Wait(ms)
                    Thread.Sleep(ms)
                end
                ");
 
        }

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
                    }
                }
            }

            // Set the console variables based on the arguments. Always do it in this order.
            Verbose.SetConsoleOutputs(errorMajor, errorMinor, logMajor, logMinor);
            Verbose.SetSaveConsole(saveOutput);
        }
    }
}
