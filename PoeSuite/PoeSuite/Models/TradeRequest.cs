﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using PoeSuite.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoeSuite.Models
{
    public class TradeRequest : ViewModelBase
    {
        private bool _playerJoinedArea;

        public string PlayerName { get; set; }
        public string ItemName { get; set; }
        public string StashTabName { get; set; }
        public System.Numerics.Vector<int> ItemPosition { get; set; }
        public int Price { get; set; }
        public string CurrencyName { get; set; }

        public bool Outgoing { get; set; }

        public bool PlayerJoinedArea
        {
            get { return _playerJoinedArea; }
            set
            { 
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
