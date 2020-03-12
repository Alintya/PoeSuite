using PoeSuite.Messages;

using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight;

using System.Drawing;

namespace PoeSuite.Models
{
    public class TradeRequest : ViewModelBase
    {
        private bool _playerJoinedArea;

        public string PlayerName { get; set; }
        public string ItemName { get; set; }
        public int ItemAmount { get; set; }
        public string StashTabName { get; set; }
        public Point ItemPosition { get; set; }
        public int Price { get; set; }
        public string CurrencyName { get; set; }
        public bool IsCurrencyExchange { get; set; }
        public bool IsOutgoing { get; set; }

        public bool PlayerJoinedArea
        {
            get => _playerJoinedArea;
            set
            {
                if (value == _playerJoinedArea)
                    return;

                _playerJoinedArea = value;
                RaisePropertyChanged(nameof(PlayerJoinedArea));
            }
        }

        public TradeRequest()
        {
            Messenger.Default.Register<PlayerJoinedAreaMessage>(this, msg =>
            {
                if (msg.PlayerName.Equals(PlayerName))
                    PlayerJoinedArea = true;
            });
        }
    }
}
