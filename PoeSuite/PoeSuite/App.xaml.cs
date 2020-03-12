using PoeSuite.Utilities;
using PoeSuite.Imports;
using PoeSuite.Views;

using System.Diagnostics;
using System.Threading;
using System.Windows;
using System;

namespace PoeSuite
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Mutex _instanceMutex;

        private void Application_Startup(object sender, StartupEventArgs e)
		{
            if (Mutex.TryOpenExisting("Local\\PoESuite", out _))
            {
                var currProc = Process.GetCurrentProcess();
                var targProc = Array.Find(Process.GetProcessesByName(currProc.ProcessName), x => x.Id != currProc.Id);
                if (targProc != null)
                    User32.SetForegroundWindow(targProc.MainWindowHandle);

                Environment.Exit(-1);
            }

            Logger.Get.EnableFileLogging();

            _instanceMutex = new Mutex(true, "Local\\PoESuite", out var createdNew);

            GC.KeepAlive(_instanceMutex);

            this.MainWindow = new MainWindow();
            this.MainWindow.Show();

            if (string.IsNullOrEmpty(PoeSuite.Properties.Settings.Default.AccountName))
            {
                var dialog = new TextBoxPrompt("Settings", "Enter your PoE account name.");
                if (dialog.ShowDialog() == true)
                {
                    PoeSuite.Properties.Settings.Default.AccountName = dialog.ResponseText;
                    PoeSuite.Properties.Settings.Default.Save();

                    Logger.Get.Success($"Updated AccountName setting due to user input: {dialog.ResponseText}");
                }
                else
                {
                    Logger.Get.Error("AccountName setting was empty but not updated by user");
                }
            }
        }
	}
}
