using PoeSuite.DataTypes;
using PoeSuite.Messages;
using PoeSuite.Models;

using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight;

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
            get => _activeRequests;
            set
            {
                if (_activeRequests == value)
                    return;

                _activeRequests = value;
                RaisePropertyChanged(nameof(ActiveRequests));
            }
        }

        public TradeRequest SelectedTab
        {
            get => _selectedTab;
            set
            {
                if (_selectedTab == value)
                    return;

                _selectedTab = value;
                RaisePropertyChanged(nameof(SelectedTab));
            }
        }

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

        public RelayCommand CloseTabCommand { get; }
        public RelayCommand SendTradeCommand { get; }
        public RelayCommand SendInviteCommand { get; }
        public RelayCommand EnterHideoutCommand { get; }
        public RelayCommand KickCommand { get; }
        public RelayCommand SendMessageCommand { get; }

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
            SendTradeCommand = new RelayCommand(() => MessengerInstance.Send(new SendChatMessage(CreateChatCommand("/tradewith"))));
            SendInviteCommand = new RelayCommand(() => MessengerInstance.Send(new SendChatMessage(CreateChatCommand("/invite"))));
            EnterHideoutCommand = new RelayCommand(() => MessengerInstance.Send(new SendChatMessage(CreateChatCommand("/hideout"))));
            KickCommand = new RelayCommand(() => MessengerInstance.Send(new SendChatMessage(CreateChatCommand("/kick"))));

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
            return new ChatMessage
            {
                Channel = DataTypes.Enums.ChatMessageChannel.ChatCommand,
                Sender = _selectedTab.PlayerName,
                Message = command
            };
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