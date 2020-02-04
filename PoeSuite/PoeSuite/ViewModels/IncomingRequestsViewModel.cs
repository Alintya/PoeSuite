using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using PoeSuite.DataTypes;
using PoeSuite.Messages;
using PoeSuite.Models;
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
        public RelayCommand SendTradeCommand { get; private set; }
        public RelayCommand SendInviteCommand { get; private set; }
        public RelayCommand EnterHideoutCommand { get; private set; }
        public RelayCommand KickCommand { get; private set; }
        public RelayCommand SendMessageCommand { get; private set; }

        public IncomingRequestsViewModel()
        {
            // mockup
            ActiveRequests = new ObservableCollection<TradeRequest>() { new TradeRequest { ItemName = "testItem12345678", PlayerName= "huehue" }};
            /*
            HotkeysManager.Get.AddCallback("OpenSettings", () =>
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        MessengerInstance.Send(new IncomingTradeMessage(new TradeRequest { ItemName = "test32" }));
                        MessengerInstance.Send(new PlayerJoinedAreaMessage { PlayerName="huehue" });
                    });
            });
            */

            CloseTabCommand = new RelayCommand(ExecuteCloseTab);

            SendTradeCommand = new RelayCommand(() =>
            {
                MessengerInstance.Send(new Messages.SendChatMessage(CreateChatCommand("/tradewith")));
            });

            SendInviteCommand = new RelayCommand(() =>
            {
                MessengerInstance.Send(new SendChatMessage(CreateChatCommand("/invite")));
            });

            EnterHideoutCommand = new RelayCommand(() =>
            {
                MessengerInstance.Send(new SendChatMessage(CreateChatCommand("/hideout")));
            });

            KickCommand = new RelayCommand(() =>
            {
                MessengerInstance.Send(new SendChatMessage(CreateChatCommand("/kick")));
            });


            MessengerInstance.Register<IncomingTradeMessage>(this, msg =>
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    Add(msg.Request);
                    ShouldBeVisible = true;
                });
            });
        }

        private ChatMessage CreateChatCommand(string command)
        {
            return new ChatMessage { Channel = DataTypes.Enums.ChatMessageChannel.ChatCommand,
                Sender = _selectedTab.PlayerName, Message = command };
        }

        private void ExecuteCloseTab()
        {
            Remove(_selectedTab);
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