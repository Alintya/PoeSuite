using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using PoeSuite.DataTypes.Interfaces;
using PoeSuite.Messages;
using PoeSuite.Utilities.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoeSuite.ViewModels
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();
        public OverlayViewModel OverlayCanvas => ServiceLocator.Current.GetInstance<OverlayViewModel>();
        public IncomingRequestsViewModel IncomingRequests => ServiceLocator.Current.GetInstance<IncomingRequestsViewModel>();

        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (ViewModelBase.IsInDesignModeStatic)
            {
                // Create design time view services and models                
            }
            else
            {
                // Create run time view services and models     
                SimpleIoc.Default.Register<IOService, IOServiceContainer>();
                SimpleIoc.Default.Register<Features.TradeHelper>();
            }

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<OverlayViewModel>();
            SimpleIoc.Default.Register<IncomingRequestsViewModel>();
        }

        public static void Cleanup()
        {
        }
    }
}
