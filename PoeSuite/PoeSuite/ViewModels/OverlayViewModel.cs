using GalaSoft.MvvmLight;
using PoeSuite.Messages;
using PoeSuite.Utilities;
using System.ComponentModel;
using System.Windows;

namespace PoeSuite.ViewModels
{
    public class OverlayViewModel : ViewModelBase
    {
        private bool _shouldBeVisible;

        public bool ShouldBeVisible
        {
            get => _shouldBeVisible;
            set
            {
                if (_shouldBeVisible == value)
                    return;

                _shouldBeVisible = value;
                RaisePropertyChanged(nameof(ShouldBeVisible));
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
                MessengerInstance.Register<GameActiveStatusChanged>(this, msg =>
                {
                    ShouldBeVisible = msg.IsInForeground;
                });
            }
        }
    }
}