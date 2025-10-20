using System.Runtime.InteropServices;
using Barebones.Config;

namespace Barebones 
{ 
    public static class BarebonesInit
    {
        [DllImport("kernel32.dll", EntryPoint = "GetStdHandle", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", EntryPoint = "AllocConsole", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        static extern int AllocConsole();

        const int STD_OUTPOUT_HANDLE = -11;

        public static void ConsoleInit(string[] args)
        {
            Engine.CheckLaunchArguments(args);

            if (Verbose.ShowConsole)
            {
                AllocConsole();
                IntPtr stdHandle = GetStdHandle(STD_OUTPOUT_HANDLE);
                Microsoft.Win32.SafeHandles.SafeFileHandle safeFileHandle = new Microsoft.Win32.SafeHandles.SafeFileHandle(stdHandle, true);
                FileStream fileStream = new FileStream(safeFileHandle, FileAccess.Write);
                System.Text.Encoding encoding = System.Text.Encoding.ASCII;
                StreamWriter standardOutput = new StreamWriter(fileStream, encoding);
                standardOutput.AutoFlush = true;
                Console.SetOut(standardOutput);
            }
        }
    }

}