using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Ioc;
using PoeSuite.DataTypes.Interfaces;
using PoeSuite.Imports;
using PoeSuite.Messages;
using PoeSuite.Utilities;
using PoeSuite.Utilities.Services;
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
        private IOService _ioService;


        public MainViewModel()
        {
            if (IsInDesignMode)
                WindowTitle = "PoeSuite (Designmode)";
            else
                WindowTitle = "PoeSuite - Settings";

            _ioService = SimpleIoc.Default.GetInstance<IOService>();

            OpenFileDialogCommand = new RelayCommand(OpenFile);


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
            IntPtr m_hhook = User32.SetWinEventHook(User32.EVENT_SYSTEM_FOREGROUND, User32.EVENT_SYSTEM_FOREGROUND,
                IntPtr.Zero, _winEventCallback, 0, 0, User32.WINEVENT_OUTOFCONTEXT);
        }

        private void OpenFile()
        {
            var path = _ioService.OpenFileDialog();

            if (path != null && !string.IsNullOrEmpty(path))
                Properties.Settings.Default.PoeFilePath = path;
        }

        public void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild,
            uint dwEventThread, uint dwmsEventTime)
        {
            //Logger.Get.Debug("Active window changed to " + User32.GetActiveWindowTitle());

            // TODO: or one of our overlay windows
            if (!(Poe is null) && (Poe.IsWindowHandle(hwnd) || false))
            {
                HotkeysManager.Get.IsEnabled = true;
                MessengerInstance.Send(new GameActiveStatusChanged { IsInForeground = true });
            }
            else
            {
                HotkeysManager.Get.IsEnabled = false;
                MessengerInstance.Send(new GameActiveStatusChanged { IsInForeground = false });
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
                _timer.Stop();

                Poe = new Game(instances.First());
                Poe.GameProcessExited += Poe_GameProcessExited;
                Properties.Settings.Default.PoeFilePath = instances.First().MainModule.FileName;

                //MessengerInstance.Send(new Messages.GameInstanceUpdated { GameInstance = (IGame)Poe });

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
