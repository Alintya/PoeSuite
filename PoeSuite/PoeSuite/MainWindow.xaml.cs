using PoeSuite.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Hardcodet.Wpf.TaskbarNotification;
using System.IO;
using System.ComponentModel;

namespace PoeSuite
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private LogListener _logListener;
        private HotKeyManager hotkeys = null;

        public MainWindow()
        {
            InitializeComponent();

            hotkeys = new HotKeyManager();
            hotkeys.HotkeyPressed += new HotKeyManager.HotkeyDelegate(hotkeys_HotkeyPressed);

            /*
            TaskbarIcon trayIcon = new TaskbarIcon();
            Stream iconStream = Application.GetResourceStream(new Uri("pack://application:,,,/Icon1.ico")).Stream;
            trayIcon.Icon = new System.Drawing.Icon(iconStream);
            */
            myNotifyIcon.TrayMouseDoubleClick += (object x, RoutedEventArgs y) =>
                {
                    base.Show();
                    base.WindowState = WindowState.Normal;
                };
        }

        #region Minimize to tray

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
                base.Hide();

            base.OnStateChanged(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            // setting cancel to true will cancel the close request
            // so the application is not closed
            e.Cancel = true;

            base.Hide();

            base.OnClosing(e);
        }

        #endregion

        private void hotkeys_HotkeyPressed(int ID)
        {
            var key = (System.Windows.Forms.Keys)ID;
            Console.WriteLine($"Key pressed: {key.ToString()}");
            switch (ID)
            {
                case 1001:
                    MessageBox.Show("Alt-1");
                    break;

                case 1002:
                    MessageBox.Show("Alt-2");
                    break;

                case 1003: // Alt-Q
                    //Application.Exit();
                    break;
                default:
                    break;
            }
        }
    }
}
