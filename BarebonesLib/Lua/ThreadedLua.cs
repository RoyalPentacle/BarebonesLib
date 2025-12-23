using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barebones.Lua
{
    /// <summary>
    /// Contains functions for running Lua asynchronously.
    /// </summary>
    public static class ThreadedLua
    {
        /// <summary>
        /// Runs a script in a fresh Lua machine asynchronously
        /// </summary>
        /// <param name="script">The lua script to run.</param>
        public static void RunScript(string script)
        {

            Task.Run(() => {
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
            });
        }
    }
}
