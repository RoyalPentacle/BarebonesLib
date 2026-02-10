using System.Collections.Concurrent;
using System.Runtime.InteropServices;

namespace Barebones
{
    /// <summary>
    /// The system for writing messages to the console with varying levels of verbosity.
    /// </summary>
    public static class Verbose
    {
        [DllImport("kernel32.dll", EntryPoint = "GetStdHandle", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", EntryPoint = "AllocConsole", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        static extern int AllocConsole();

        [DllImport("kernel32.dll", EntryPoint = "CancelIoEx", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern bool CancelIoEx(IntPtr handle, IntPtr lpOverlapped);

        const int STD_INPUT_HANDLE = -10;
        const int STD_OUTPOUT_HANDLE = -11;
        


        private struct ConsoleMessage
        {
            public string Message;
            public string Prefix;
            public string TimeStamp;
            public ConsoleColor Color;

            public ConsoleMessage(string message, string timestamp, string prefix, ConsoleColor color)
            {
                Message = message; 
                Prefix = prefix; 
                TimeStamp = timestamp; 
                Color = color;
            }
        }

        private static bool _showErrorMajor = false;
        private static bool _showErrorMinor = false;
        private static bool _showLogMajor = false;
        private static bool _showLogMinor = false;

        private static StreamWriter? _fileOutput;
        private static string _input = "";
        private static string _lastInput = "";
        private static Thread _inputThread;
        
        private static readonly ConcurrentQueue<ConsoleMessage> _consoleOutput = new ConcurrentQueue<ConsoleMessage>();

        private static Mutex _mut = new Mutex();

        private const ConsoleColor ERROR_MAJOR_COLOR = ConsoleColor.Red;
        private const ConsoleColor ERROR_MINOR_COLOR = ConsoleColor.Yellow;
        private const ConsoleColor LOG_MAJOR_COLOR = ConsoleColor.Green;
        private const ConsoleColor LOG_MINOR_COLOR = ConsoleColor.Cyan;
        private const ConsoleColor COMMAND_COLOR = ConsoleColor.Gray;

        /// <summary>
        /// Should the console be shown?
        /// Depends on if anything is set to be shown.
        /// </summary>
        internal static bool ShowConsole
        {
            get 
            { 
                return _showErrorMajor || _showErrorMinor || _showLogMajor || _showLogMinor; 
            }
        }

        /// <summary>
        /// Sets which console outputs should be shown.
        /// </summary>
        /// <param name="errorMajor">Should the console show Major Errors?</param>
        /// <param name="errorMinor">Should the console show Minor Errors?</param>
        /// <param name="logMajor">Should the console show Major Logs?</param>
        /// <param name="logMinor">Should the console show Minor Logs?</param>
        internal static void Initalize(bool errorMajor, bool errorMinor, bool logMajor, bool logMinor)
        {
            _showErrorMajor = errorMajor;
            _showErrorMinor = errorMinor;
            _showLogMajor = logMajor;
            _showLogMinor = logMinor;

            if (ShowConsole)
            {
                AllocConsole();
                IntPtr stdHandle = GetStdHandle(STD_OUTPOUT_HANDLE);
                Microsoft.Win32.SafeHandles.SafeFileHandle safeFileHandle = new Microsoft.Win32.SafeHandles.SafeFileHandle(stdHandle, true);
                FileStream fileStream = new FileStream(safeFileHandle, FileAccess.Write);
                System.Text.Encoding encoding = System.Text.Encoding.ASCII;
                StreamWriter standardOutput = new StreamWriter(fileStream, encoding);
                standardOutput.AutoFlush = true;
                Console.SetOut(standardOutput);
                _inputThread = new Thread(ReadConsoleInput);
                _inputThread.Start();
            }
        }
        
        internal static void ExecuteCommand()
        {
            WriteCommand(_input);
            Lua.Functions.RunScript(_input);
        }

        internal static void ReadConsoleInput()
        {
            while (!Engine.IsClosing)
            {
                ConsoleKeyInfo? input = null;
                try
                {
                    Thread.Sleep(50);
                    input = Console.ReadKey(true);
                }
                catch { }
                if (input.HasValue)
                {
                    if (input.Value.Key == ConsoleKey.Enter)
                    {
                        ExecuteCommand();
                        _lastInput = _input;
                        _input = "";
                    }
                    else if (input.Value.Key == ConsoleKey.UpArrow)
                    {
                        string temp = _input;
                        _input = _lastInput;
                        _lastInput = temp;
                        _mut.WaitOne();
                        RefreshInput();
                        _mut.ReleaseMutex();
                    }
                    else if (input.Value.Key == ConsoleKey.Backspace)
                    {
                        if (_input != "")
                        {
                            _input = _input.Remove(_input.Length - 1);
                            _mut.WaitOne();
                            RefreshInput();
                            _mut.ReleaseMutex();
                        }
                    }
                    else
                    {
                        if (input.Value.KeyChar != '\u0000')
                        {
                            char command = input.Value.KeyChar;
                            _input += command;
                            _mut.WaitOne();
                            RefreshInput();
                            _mut.ReleaseMutex();
                        }
                    }
                }
            }
        }

        internal static void RefreshInput()
        {
            // TODO: If the user types so many characters that it wordwraps, this fails horribly.
            Console.CursorLeft = 0;
            Console.Write(new string(' ', Console.WindowWidth - 1));
            Console.CursorLeft = 0;
            Console.Write(_input);
        }

        internal static void PrintConsoleOutput()
        {
            _mut.WaitOne();
            if (_consoleOutput.Count > 0)
            {
                while (_consoleOutput.TryDequeue(out ConsoleMessage msg))
                {
                    Console.CursorLeft = 0;
                    Console.Write(new string(' ', Console.WindowWidth - 1));
                    Console.CursorLeft = 0;
                    WriteLog(msg.Message, msg.TimeStamp, msg.Prefix, msg.Color);
                }
                RefreshInput();
            }
            _mut.ReleaseMutex();
        }

        /// <summary>
        /// Sets whether the console output should be saved. Only if we're also showing the console.
        /// </summary>
        /// <param name="saveConsole">Should we be saving the console to a file?</param>
        internal static void SetSaveConsole(bool saveConsole)
        {
            if (saveConsole && ShowConsole)
            {
                Directory.CreateDirectory(Engine.LOGGING_PATH);
                _fileOutput = File.CreateText($"{Engine.LOGGING_PATH} BarebonesLog_{DateTime.Now:yyyy_MM_dd_HH_mm_ss}.txt");
                _fileOutput.AutoFlush = true;
            }
        }

        /// <summary>
        /// Close the filestream of the console output.
        /// </summary>
        internal static void Close()
        {
            _fileOutput?.Close();
            _input = "";
            var handle = GetStdHandle(STD_INPUT_HANDLE);
            CancelIoEx(handle, IntPtr.Zero);  
        }

        /// <summary>
        /// Write a major error message to the console, if enabled.
        /// </summary>
        /// <param name="message">The message to write.</param>
        public static void WriteErrorMajor(string message)
        {
            if (_showErrorMajor)
                _consoleOutput.Enqueue(new ConsoleMessage(message, GetTimestamp(), "!!MAJOR ERROR!!", ERROR_MAJOR_COLOR));
        }

        /// <summary>
        /// Write a minor error message to the console, if enabled.
        /// </summary>
        /// <param name="message">The message to write.</param>
        public static void WriteErrorMinor(string message)
        {
            if (_showErrorMinor)
                _consoleOutput.Enqueue(new ConsoleMessage(message, GetTimestamp(), "!MINOR ERROR!", ERROR_MINOR_COLOR));
        }

        /// <summary>
        /// Write a major log message to the console, if enabled.
        /// </summary>
        /// <param name="message">The message to write.</param>
        public static void WriteLogMajor(string message)
        {
            if (_showLogMajor)
                _consoleOutput.Enqueue(new ConsoleMessage(message, GetTimestamp(), "MAJOR Log", LOG_MAJOR_COLOR));
        }

        /// <summary>
        /// Write a minor log message to the console, if enabled.
        /// </summary>
        /// <param name="message">The message to write.</param>
        public static void WriteLogMinor(string message)
        {
            if (_showLogMinor)
                _consoleOutput.Enqueue(new ConsoleMessage(message, GetTimestamp(), "MINOR Log", LOG_MINOR_COLOR));
        }

        /// <summary>
        /// Write a command message to the console, if enabled.
        /// Typically this is done only by the console itself when the user enters a command.
        /// </summary>
        /// <param name="message">The message to write.</param>
        public static void WriteCommand(string message)
        {
            _consoleOutput.Enqueue(new ConsoleMessage(message, GetTimestamp(), "Command", COMMAND_COLOR));
        }

        /// <summary>
        /// Gets the current timestamp as a formatted string.
        /// </summary>
        /// <returns>A string timestamp in the format of HH:mm:ss:fff</returns>
        public static string GetTimestamp()
        {
            return $"{DateTime.Now:HH:mm:ss:fff}";
        }

        /// <summary>
        /// Writes a message to the console.
        /// Used internally by the various WriteError/Log functions, but exposed for extension.
        /// </summary>
        /// <param name="message">The message to print.</param>
        /// <param name="timestamp">The timestamp of the message.</param>
        /// <param name="prefix">The prefix of the message.</param>
        /// <param name="color">The color the message should be.</param>
        public static void WriteLog(string message, string timestamp, string prefix, ConsoleColor color)
        {
            string time = GetTimestamp();
            string output = $" {prefix}: {message}";
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(timestamp);
            Console.ForegroundColor = color;
            Console.WriteLine(output);
            _fileOutput?.Write(timestamp);
            _fileOutput?.WriteLine(output);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
