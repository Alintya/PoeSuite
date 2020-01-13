using System;
using System.Diagnostics;
using System.IO;
using System.Security.Permissions;

namespace PoeSuite
{
    class LogListener
    {
        private string _logFilePath;
        private FileSystemWatcher _watcher = new FileSystemWatcher();

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public LogListener(string path)
        {

            // TODO: check for file
            _logFilePath = path;

            _watcher.Path = _logFilePath;
            // Watch for changes in LastAccess and LastWrite times, and
            // the renaming of files or directories.
            _watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite;

            _watcher.Filter = "Client.txt";

            // Add event handlers.
            _watcher.Changed += OnChanged;

            // Begin watching.
            _watcher.EnableRaisingEvents = true;
            Debug.WriteLine("Watching Client log.");
        }

        // Define the event handlers.
        private void OnChanged(object source, FileSystemEventArgs e) =>
            // Specify what is done when a file is changed, created, or deleted.
            Debug.WriteLine($"File: {e.FullPath} {e.ChangeType}\nline: {ReadLastLineFromUTF8EncodedFile(Path.Combine(_logFilePath, "Client.txt"))}");

        public static string ReadLastLineFromUTF8EncodedFile(string path)
        {
            using (Stream fs = File.OpenRead(path))
            {
                if (fs.Length == 0)
                    return null;

                // start at end of file
                fs.Position = fs.Length - 1;

                // the file must end with a '\n' char, if not a partial line write is in progress
                int byteFromFile = fs.ReadByte();

                if (byteFromFile != '\n')
                {
                    // partial line write in progress, do not return the line yet
                    return null;
                }

                fs.Position--;

                // read bytes backwards until '\n' byte is hit
                while (fs.Position > 0)
                {
                    fs.Position--;

                    byteFromFile = fs.ReadByte();

                    if (byteFromFile < 0)
                        throw new IOException("Error reading from file at " + path);
                    else if (byteFromFile == '\n')
                        break;

                    fs.Position--;
                }

                // fs.Position will be right after the '\n' char or position 0 if no '\n' char
                byte[] bytes = new BinaryReader(fs).ReadBytes((int)(fs.Length - fs.Position));
                return System.Text.Encoding.UTF8.GetString(bytes);
            }
        }
    }
}
