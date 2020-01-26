using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using PoeSuite.Messages;
using PoeSuite.Models;
using System.Collections.ObjectModel;

namespace PoeSuite.ViewModels
{
    //[NotifyPropertyChanged]
    public class IncomingRequestsViewModel : ViewModelBase
    {
        private ObservableCollection<TradeRequest> _activeRequests;

        public ObservableCollection<TradeRequest> ActiveRequests
        {
            get { return _activeRequests; }
            set
            {
                if (_activeRequests != value)
                {
                    _activeRequests = value;
                    RaisePropertyChanged(nameof(ActiveRequests));
                }
            }
        }

        public bool ShouldBeVisible => ActiveRequests.Count > 0;

        public RelayCommand<TradeRequest> CloseTabCommand { get; private set; }

        public IncomingRequestsViewModel()
        {
            // mockup
            ActiveRequests = new ObservableCollection<TradeRequest>{
                new TradeRequest { PlayerName = "huehue" },
                new TradeRequest { },
                new TradeRequest { },
                new TradeRequest { }
            };

            HotkeysManager.Get.AddCallback("EnterHideout", () =>
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        Add(new TradeRequest());
                    });
            });

            CloseTabCommand = new RelayCommand<TradeRequest>(tab => ExecuteCloseTab(tab));

            MessengerInstance.Register<IncomingTradeMessage>(this, msg =>
            {
                Add(msg.Request);
            });
        }

        private void ExecuteCloseTab(TradeRequest tab)
        {
            Remove(tab);
        }

        private void Add(TradeRequest x)
        {
            _activeRequests.Add(x);
        }

        private void Remove(TradeRequest x)
        {
            _activeRequests.Remove(x);
        }
    }
}