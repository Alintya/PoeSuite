using GalaSoft.MvvmLight;
using PoeSuite.Messages;
using PoeSuite.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PoeSuite.ViewModels
{
    public class OverlayViewModel : ViewModelBase
    {
        private bool _shouldBeVisible;
        


        public bool ShouldBeVisible
        {
            get
            {
                return _shouldBeVisible;
            }
            set
            {
                _shouldBeVisible = value;
                RaisePropertyChanged(nameof(ShouldBeVisible));
            }
        }

        public Thickness IncomingOverlayPosition
        {
            get
            {
                return new Thickness(Properties.Settings.Default.IncomingOverlayX,
                    Properties.Settings.Default.IncomingOverlayY, 555, 555);
            }
            set
            {
                Properties.Settings.Default.IncomingOverlayX = value.Left;
                Properties.Settings.Default.IncomingOverlayY = value.Top;

                Logger.Get.Debug("window moved");

                RaisePropertyChanged(nameof(IncomingOverlayPosition));
            }
        }


        public OverlayViewModel()
        {
            if (IsInDesignMode)
            {
                _shouldBeVisible = true;
            }
            else
            {
                MessengerInstance.Register<GameActiveStatusChanged>(this, msg => ShouldBeVisible = msg.IsInForeground);
            }
        }
    }
}
