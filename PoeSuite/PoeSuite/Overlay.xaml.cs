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
using System.Windows.Shapes;

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
            this.Width = System.Windows.SystemParameters.PrimaryScreenWidth;

            this.Height = System.Windows.SystemParameters.PrimaryScreenHeight;

            this.Topmost = true;

            this.Top = 0;

            this.Left = 0;
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            this.Topmost = true;

            this.Activate();
        }
    }
}
