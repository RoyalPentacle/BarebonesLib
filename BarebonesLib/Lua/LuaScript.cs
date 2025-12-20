using Barebones.Config;
using Barebones.Drawable.Particles;
using Microsoft.Xna.Framework;
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
        public static void RunScript(string luaScript)
        {
            // Do some logic here to sanitize the script depending on the allowGlobal variable, either making it a local function, or just a normal one.
            try
            {
                Engine.GlobalLua.DoString(luaScript);
            }
            catch (Exception ex)
            {
                Verbose.WriteErrorMajor($"LUA: Failed to execute script!\n Ex: {ex.Message}");
            }
        }

        public static void CreateParticleSystem(string scriptPath, float posX, float posY, float forceX, float forceY)
        {
            ParticleHandler.AddParticleSystem(scriptPath, new Vector2(posX, posY), new Vector2(forceX, forceY));
        }
        public static void 
    }
}
