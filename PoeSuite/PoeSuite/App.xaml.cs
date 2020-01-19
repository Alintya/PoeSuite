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
			//EventManager.RegisterClassHandler(typeof(UIElement), UIElement.PreviewKeyUpEvent, new RoutedEventHandler(OnPreviewKeyUp));


			MainWindow wnd = new MainWindow();

			wnd.Title = "PoeSuite - Settings";

			wnd.Show();

			Overlay overlay = new Overlay();
			overlay.Show();
		}

		/*
		private static void OnPreviewKeyUp(object source, RoutedEventArgs e)
		{
			//var x = e as System.Windows.Forms.KeyEventArgs;

			//Console.WriteLine($"Key pressed: {x.KeyCode}");
		}
		*/
	}
}
