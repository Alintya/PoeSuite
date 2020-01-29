using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using PoeSuite.Imports;
using PoeSuite.Messages;
using PoeSuite.Utilities;
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

        public RelayCommand OpenFileDialogCommand { get; private set; }

        public Game Poe;

        private readonly Timer _timer;
        private User32.WinEventDelegate _winEventCallback;


        public MainViewModel()
        {
            if (IsInDesignMode)
                WindowTitle = "PoeSuite (Designmode)";
            else
                WindowTitle = "PoeSuite - Settings";

            // TODO
            OpenFileDialogCommand = new RelayCommand(() => { });


            _timer = new Timer(250);
            _timer.Elapsed += OnTimerElapsed;

            if (Properties.Settings.Default.AutoStartPoe
                && !string.IsNullOrEmpty(Properties.Settings.Default.PoeFilePath)
                && Game.ValidateGamePath(Properties.Settings.Default.PoeFilePath)
                && !Game.GetRunningInstances().Any())
            {
                Poe = Game.Launch(Properties.Settings.Default.PoeFilePath);
            }
            else
            {
                _timer.Start();
            }
       
            HotkeysManager.Get.AddCallback("Logout", () =>
            {
                Poe?.CloseTcpConnections();
                Logger.Get.Info("logoutCommand called");
            });
            //HotkeysManager.Get.AddModifier("Logout", LowLevelInput.Hooks.VirtualKeyCode.Lshift);

            _winEventCallback = new User32.WinEventDelegate(WinEventProc);
            IntPtr m_hhook = User32.SetWinEventHook(User32.EVENT_SYSTEM_FOREGROUND, User32.EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, _winEventCallback, 0, 0, User32.WINEVENT_OUTOFCONTEXT);
        }

        public void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            //Logger.Get.Debug("Active window changed to " + User32.GetActiveWindowTitle());

            // TODO: or one of our overlay windows
            if (!(Poe is null) && (Poe.IsForegroundWindow || false))
            {
                HotkeysManager.Get.IsEnabled = true;
            }
            else
            {
                HotkeysManager.Get.IsEnabled = false;
            }
            
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            var instances = Game.GetRunningInstances();

            if (instances.Count() > 1)
            {
                // TODO: show selection prompt
                
            }
            if (instances.Count() == 1)
            {
                Poe = new Game(instances.First());
                _timer.Stop();

                Poe.GameProcessExited += Poe_GameProcessExited;
                Properties.Settings.Default.PoeFilePath = instances.First().MainModule.FileName;

                Logger.Get.Success("Found poe instance");
            }
        }

        private void Poe_GameProcessExited(object sender, EventArgs e)
        {
            Poe.Dispose();
            _timer.Start();
        }
    }
}
