using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barebones.Lua
{
    public static class ThreadedLua
    {
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
