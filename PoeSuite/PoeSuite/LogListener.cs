using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PoeSuite
{
    public class LogListener : IDisposable
    {
        private readonly StreamReader _logFileStream;
        private readonly int _interval;
        private readonly Dictionary<Regex, Action<string, Match>> _listeners;
        private Thread _workerThread;
        private bool _stopThread;
        private bool _disposed;

        public LogListener(string filePath, int checkInterval = 1000)
        {
            if (!File.Exists(filePath))
                throw new ArgumentException("File doesnt exist");

            _logFileStream = new StreamReader(
                File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite), Encoding.UTF8, false, 1024);

            _interval = checkInterval;
            _listeners = new Dictionary<Regex, Action<string, Match>>();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void AddListener(string regex, Action<string, Match> action)
        {
            if (_listeners.Any(x => x.Key.ToString() == regex))
                throw new ArgumentException("Listener for this regex already exists");

            _listeners.Add(new Regex(regex), action);
        }

        public void StartListening()
        {
            if (_workerThread != null)
                return;

            _workerThread = new Thread(DoWork)
            {
                IsBackground = true,
                Priority = ThreadPriority.BelowNormal
            };

            _stopThread = false;
            _workerThread.Start();
        }

        public void StopListening()
        {
            if (_workerThread == null || _stopThread)
                return;

            _stopThread = true;
            _workerThread.Join();
            _workerThread = null;
        }

        private void DoWork()
        {
            var lineCache = new List<string>();

            _logFileStream.BaseStream.Seek(0, SeekOrigin.End);

            while (!_stopThread)
            {
                while (!_logFileStream.EndOfStream)
                {
                    var currLine = _logFileStream.ReadLine();
                    if (currLine.Length == 0)
                        continue;

                    lineCache.Add(currLine);
                }

                if (lineCache.Count > 0)
                {
                    lineCache.Reverse();

                    foreach (var listener in _listeners)
                    {
                        foreach (var line in lineCache)
                        {
                            var match = listener.Key.Match(line);
                            if (match.Success)
                            {
                                listener.Value(line, match);
                                break;
                            }
                        }
                    }

                    lineCache.Clear();
                }

                Thread.Sleep(_interval);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
                this.StopListening();

            _disposed = true;
        }
    }
}
