using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace PoeSuite.Utilities
{
    public class Logger
    {
        private readonly object _lockObject = new object();
        private StreamWriter _logFileStream = null;
        private static Logger _instance = null;

        public static Logger Get => _instance ?? (_instance = new Logger());

        private Logger() { }

        public void EnableFileLogging(string fileName = "log.txt")
        {
            if (!(_logFileStream is null))
                throw new InvalidOperationException();

            lock (_lockObject)
            {
                try
                {
                    _logFileStream = new StreamWriter(
                        File.Open(fileName, FileMode.Append, FileAccess.Write, FileShare.Read), Encoding.UTF8)
                    {
                        AutoFlush = true
                    };
                }
                catch (Exception ex)
                {
                    this.Error($"Failed to open or create log file: {ex.Message}");
                }
            }
        }

        [Conditional("DEBUG")]
        public void Debug(string msg)
        {
            Log(msg, ConsoleColor.Cyan);
        }

        public void Info(string msg)
        {
            Log(msg, ConsoleColor.White);
        }

        public void Success(string msg)
        {
            Log(msg, ConsoleColor.Green);
        }

        public void Warning(string msg)
        {
            Log(msg, ConsoleColor.Magenta);
        }

        public void Error(string msg)
        {
            Log(msg, ConsoleColor.Red);
        }

        private void Log(string msg, ConsoleColor clr, [CallerMemberName] string caller = "")
        {
            lock (_lockObject)
            {
                msg = $"[{DateTime.Now.ToString()}]{caller.PadLeft(8, ' ')}| {msg}";

#if DEBUG
                Console.ForegroundColor = clr;
                Console.WriteLine(msg);
                Console.ResetColor();
#endif

                _logFileStream?.WriteLine(msg);
            }
        }
    }
}
