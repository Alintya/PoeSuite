using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using PoeSuite.DataTypes;
using PoeSuite.Messages;
using System.Collections.ObjectModel;

namespace PoeSuite.ViewModels
{
    //[NotifyPropertyChanged]
    public class IncomingRequestsViewModel : ViewModelBase
    {
        private ObservableCollection<TradeRequest> _activeRequests;
        private bool _shouldBeVisible = true;
        private TradeRequest _selectedTab;

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

        public TradeRequest SelectedTab
        {
            get { return _selectedTab; }
            set
            {
                if (_selectedTab != value)
                {
                    _selectedTab = value;
                    RaisePropertyChanged(nameof(SelectedTab));
                }
            }
        }

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

        public RelayCommand CloseTabCommand { get; private set; }

        public IncomingRequestsViewModel()
        {
            // mockup
            ActiveRequests = new ObservableCollection<TradeRequest>{
                new TradeRequest { PlayerName = "huehue", ItemName = "sdsf" },
                new TradeRequest { ItemName = "sdssdfasdff" },
                new TradeRequest { ItemName = "asc" },
                new TradeRequest { }
            };

            HotkeysManager.Get.AddCallback("EnterHideout", () =>
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        Add(new TradeRequest());
                        MessengerInstance.Send<IncomingTradeMessage>(new IncomingTradeMessage(new TradeRequest { ItemName = "test32" }));
                    });
            });

            CloseTabCommand = new RelayCommand(() => ExecuteCloseTab());

            MessengerInstance.Register<IncomingTradeMessage>(this, msg =>
            {
                Add(msg.Request);
                ShouldBeVisible = true;
            });
        }

        private void ExecuteCloseTab()
        {
            Remove(SelectedTab);
        }

        private void Add(TradeRequest x)
        {
            _activeRequests.Add(x);
        }

        private void Remove(TradeRequest x)
        {
            if (_activeRequests.Count > 0)
                _activeRequests.Remove(x);

            if (_activeRequests.Count == 0)
                ShouldBeVisible = false;
        }
    }
}