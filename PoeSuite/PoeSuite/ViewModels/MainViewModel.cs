﻿using GalaSoft.MvvmLight;
using PoeSuite.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading.Tasks;
using System.Timers;

namespace PoeSuite.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public string WindowTitle { get; private set; }

        public Game Poe = default;

        private readonly Timer _timer;

        private HotkeysManager _hotkeys = default;


        public MainViewModel()
        {
            if (IsInDesignMode)
                WindowTitle = "PoeSuite (Designmode)";
            else
                WindowTitle = "PoeSuite - Settings";

            _timer = new Timer(250);
            _timer.Elapsed += new ElapsedEventHandler(OnTimerElapsed);
            _timer.Start();


            _hotkeys = new HotkeysManager();
            _hotkeys.AddCallback("Logout", () =>
            {
                Poe?.CloseTcpConnections();
                Logger.Get.Info("logoutCommand called");
            });
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            var instances = Game.GetRunningInstances();

            if (instances.Count() > 1)
            {
                // TODO
            }
            if (instances.Count() == 1)
            {
                Poe = new Game(instances.First().Id);
                _timer.Stop();

                Poe.GameProcessExited += Poe_GameProcessExited;

                Logger.Get.Success("Found poe instance");
            }
        }

        private void Poe_GameProcessExited(object sender, EventArgs e)
        {
            _timer.Start();
        }
    }
}