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
}
