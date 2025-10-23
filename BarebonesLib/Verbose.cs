using Barebones.Config;

namespace Barebones
{
    /// <summary>
    /// The system for writing messages to the console with varying levels of verbosity.
    /// </summary>
    public static class Verbose
    {
        private static bool _showErrorMajor = false;
        private static bool _showErrorMinor = false;
        private static bool _showLogMajor = false;
        private static bool _showLogMinor = false;

        private static StreamWriter? _fileOutput;


        private static Mutex _mut = new Mutex();

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
        internal static void SetConsoleOutputs(bool errorMajor, bool errorMinor, bool logMajor, bool logMinor)
        {
            _showErrorMajor = errorMajor;
            _showErrorMinor = errorMinor;
            _showLogMajor = logMajor;
            _showLogMinor = logMinor;
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
        internal static void CloseFilestream()
        {
            _fileOutput?.Close();
        }

        /// <summary>
        /// Write a major error message to the console, if enabled.
        /// </summary>
        /// <param name="message">The message to write.</param>
        public static void WriteErrorMajor(string message)
        {
            if (_showErrorMajor)
            {
                _mut.WaitOne();
                string time = GetTimestamp();
                string output = $" !!MAJOR ERROR!!: {message}";
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(time);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(output);
                _fileOutput?.Write(time);
                _fileOutput?.WriteLine(output);
                _mut.ReleaseMutex();
            }
        }

        /// <summary>
        /// Write a minor error message to the console, if enabled.
        /// </summary>
        /// <param name="message">The message to write.</param>
        public static void WriteErrorMinor(string message)
        {
            if (_showErrorMinor)
            {
                _mut.WaitOne();
                string time = GetTimestamp();
                string output = $" !MINOR ERROR!: {message}";
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(time);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(output);
                _fileOutput?.Write(time);
                _fileOutput?.WriteLine(output);
                _mut.ReleaseMutex();
            }
        }

        /// <summary>
        /// Write a major log message to the console, if enabled.
        /// </summary>
        /// <param name="message">The message to write.</param>
        public static void WriteLogMajor(string message)
        {
            if (_showLogMajor)
            {
                _mut.WaitOne();
                string time = GetTimestamp();
                string output = $" MAJOR Log: {message}";
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(time);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(output);
                _fileOutput?.Write(time);
                _fileOutput?.WriteLine(output);
                _mut.ReleaseMutex();
            }
        }

        /// <summary>
        /// Write a minor log message to the console, if enabled.
        /// </summary>
        /// <param name="message">The message to write.</param>
        public static void WriteLogMinor(string message)
        {
            if (_showLogMinor)
            {
                _mut.WaitOne();
                string time = GetTimestamp();
                string output = $" MINOR Log: {message}";
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(time);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(output);
                _fileOutput?.Write(time);
                _fileOutput?.WriteLine(output);
                _mut.ReleaseMutex();
            }
        }

        private static string GetTimestamp()
        {
            return $"{DateTime.Now:HH:mm:ss:fff}";
        }
    }
}
