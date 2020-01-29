﻿using GalaSoft.MvvmLight;
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
                        MessengerInstance.Send<IncomingTradeMessage>(new IncomingTradeMessage(new TradeRequest { ItemName = "test32" }));
                    });
            });

            CloseTabCommand = new RelayCommand<TradeRequest>(tab => ExecuteCloseTab(tab));

            MessengerInstance.Register<IncomingTradeMessage>(this, msg =>
            {
                Add(msg.Request);
                ShouldBeVisible = true;
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
            if (_activeRequests.Count > 0)
                _activeRequests.Remove(x);

            if (_activeRequests.Count == 0)
                ShouldBeVisible = false;
        }
    }
}