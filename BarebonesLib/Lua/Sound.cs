using Barebones.Config;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barebones.Lua
{
    public static class Script
    {

        public static void RunScript(string luaScript)
        {
            // Do some logic here to sanitize the script depending on the allowGlobal variable, either making it a local function, or just a normal one.
            try
            {
                Engine.LuaState.DoString(luaScript);
            } 
            catch (Exception ex)
            {
                Verbose.WriteErrorMajor($"LUA: Failed to execute script!\n Ex: {ex.Message}");
            }
        }

    }

}
