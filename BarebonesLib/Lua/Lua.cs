using Barebones.Asset;
using Barebones.Asset.Scripts;
using Barebones.Config;
using Barebones.Drawable.Particles;
using Microsoft.Xna.Framework;
using NLua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barebones.Lua
{


    /// <summary>
    /// Contains functions for running lua scripts.
    /// </summary>
    public static class Functions
    {
        /// <summary>
        /// Run the given script in lua.
        /// </summary>
        /// <param name="luaScript">The script to execute.</param>
        [LuaHide]
        public static void RunScript(string luaScript)
        {
            // Do some logic here to sanitize the script depending on the allowGlobal variable, either making it a local function, or just a normal one.
            try
            {
                try
                {
                    Engine.GlobalLua.DoString(luaScript);
                }
                catch (NLua.Exceptions.LuaException e)
                {
                    Verbose.WriteErrorMajor($"LUA: Failed to execute script!\n Ex: {e.Message}");
                }
            }
            catch (Exception ex)
            {
                Verbose.WriteErrorMajor($"LUA: Failed to execute script!\n Ex: {ex.Message}");
            }
        }

        /// <summary>
        /// Play a sound from a given soundScript path.
        /// </summary>
        /// <param name="scriptPath"></param>
        public static void PlaySound(string scriptPath)
        {
            SoundScript soundScript = ScriptFinder.FindScript<SoundScript>(scriptPath);
            Asset.Sound.PlaySound(soundScript.SoundPath);
        }

        /// <summary>
        /// Create a particle system with the given arguments.
        /// </summary>
        /// <param name="scriptPath">The path to the particle script.</param>
        /// <param name="posX">The X coordinate of the particle system.</param>
        /// <param name="posY">The Y coordinate of the particle system.</param>
        /// <param name="forceX">The constant force on the X axis to apply to the system.</param>
        /// <param name="forceY">The constant force on the Y axis to apply to the system.</param>
        public static void CreateParticleSystem(string scriptPath, float posX, float posY, float forceX, float forceY)
        {
            ParticleHandler.AddParticleSystem(scriptPath, new Vector2(posX, posY), new Vector2(forceX, forceY));
        }

    }

    /// <summary>
    /// Contains functions for running Lua asynchronously.
    /// </summary>
    public static class Threaded
    {
        /// <summary>
        /// Runs a script in a fresh Lua machine asynchronously
        /// </summary>
        /// <param name="script">The lua script to run.</param>
        [LuaHide]
        public static void RunScript(string script)
        {

            Task.Run(() => {
                try
                {
                    using (var lua = new NLua.Lua())
                    {
                        lua.LoadCLRPackage();
                        lua.DoString(@"
                        import('Barebones', 'Barebones.Lua')
                        import('System.Threading')
                        function Wait(ms)
                            Thread.Sleep(ms)
                        end
                        ");
                        lua.DoString(script);
                        lua.Dispose();
                    }
                } catch (NLua.Exceptions.LuaException e)
                {
                    Verbose.WriteErrorMajor($"LUA: Failed to execute script!\n Ex: {e.Message}");
                }
            });
        }
    }
}
