using PoeSuite.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace PoeSuite
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
		private void Application_Startup(object sender, StartupEventArgs e)
		{
			MainWindow wnd = new MainWindow();
			wnd.Show();

			Overlay overlay = new Overlay();
            //overlay.Show();


            // Validate UserSettings
            if (String.IsNullOrEmpty(PoeSuite.Properties.Settings.Default.AccountName))
            {
                var dialog = new TextBoxPrompt();

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
