using PoeSuite.Imports;
using PoeSuite.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

// TODO: adjust to poe screen position and size

namespace PoeSuite
{
    /// <summary>
    /// Interaction logic for Overlay.xaml
    /// </summary>
    public partial class Overlay : Window
    {
        public Overlay()
        {
            InitializeComponent();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            this.Width = SystemParameters.PrimaryScreenWidth;

            this.Height = SystemParameters.PrimaryScreenHeight;

            this.Topmost = true;

            this.Top = 0;

            this.Left = 0;
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            this.Topmost = true;

            this.Activate();
        }

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            //base.OnSourceInitialized(e);

            //Set the window style to noactivate.
            WindowInteropHelper helper = new WindowInteropHelper(this);
            User32.SetWindowLong(helper.Handle, User32.GWL_EXSTYLE, User32.GetWindowLong(helper.Handle, User32.GWL_EXSTYLE) | User32.WS_EX_NOACTIVATE);
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
