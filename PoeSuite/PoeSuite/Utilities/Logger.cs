using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Text;
using System.IO;
using System;
using System.ComponentModel;
using System.Globalization;
using PoeSuite.Imports;

namespace PoeSuite.Utilities
{
    public class Logger : IDisposable
    {
        private readonly object _lockObject = new object();
        private StreamWriter _logFileStream = null;
        private static Logger _instance = null;
        private bool _disposed;

        /// <summary>
        /// Indicates if a console window is attached
        /// </summary>
        private bool _consoleAttached = Kernel32.GetConsoleWindow() != IntPtr.Zero;

        public static Logger Get => _instance ?? (_instance = new Logger());

        private Logger() { }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Creates and attaches a console window if necessary
        /// </summary>
        public void AttachConsole()
        {
            if (_consoleAttached)
                throw new InvalidOperationException();

            lock (_lockObject)
            {
                if (!Kernel32.AttachConsole(-1) && !Kernel32.AllocConsole())
                    throw new Win32Exception();

                Console.SetOut(new StreamWriter(Console.OpenStandardOutput(), Console.OutputEncoding)
                {
                    AutoFlush = true
                });

                _consoleAttached = true;
            }
        }

        /// <summary>
        /// Removes the attached console window
        /// </summary>
        public void DetachConsole()
        {
            if (_consoleAttached)
                throw new InvalidOperationException();

            lock (_lockObject)
            {
                _consoleAttached = !Kernel32.FreeConsole();
            }
        }

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
                msg = $"[{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]{caller?.PadLeft(8, ' ')}| {msg}";

#if DEBUG
                if (_consoleAttached)
                {
                    Console.ForegroundColor = clr;
                    Console.WriteLine(msg);
                    Console.ResetColor();
                }
#endif

                _logFileStream?.WriteLine(msg);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                if (_instance == this)
                    _instance = null;

                _logFileStream?.Dispose();
                _logFileStream = null;
            }

            _instance = null;
            _disposed = true;
        }
    }
}
