using LowLevelInput.Hooks;
using PoeSuite.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace PoeSuite
{
    // TODO: put this in its own file
    internal class HotkeyCommand
    {
        public VirtualKeyCode KeyCode;
        public KeyState State = KeyState.Up;
        public List<Action> Actions = new List<Action>();
    }

    internal class HotkeysManager
    {
        private readonly LowLevelKeyboardHook _keyboardHook;
        private readonly Dictionary<string, HotkeyCommand> _hotkeys;

        public HotkeysManager()
        {
            _keyboardHook = new LowLevelKeyboardHook(true);
            _hotkeys = new Dictionary<string, HotkeyCommand>();

            foreach (var settings in Properties.Hotkeys.Default.Properties.Cast<SettingsProperty>())
            {
                if (!(Properties.Hotkeys.Default[settings.Name] is VirtualKeyCode keyCode))
                    continue;

                _hotkeys.Add(settings.Name, new HotkeyCommand
                {
                    KeyCode = keyCode
                });

                Logger.Get.Info($"Added hotkey {keyCode} for action {settings.Name}");
            }

            Properties.Hotkeys.Default.PropertyChanged += OnSettingsPropertyChanged;

            _keyboardHook.OnKeyboardEvent += OnKeyboardEvent;
            _keyboardHook.InstallHook();
        }

        public void AddCallbacks(string command, List<Action> callbacks)
        {
            callbacks.ForEach(x => AddCallback(command, x));
        }

        public void AddCallback(string command, Action callback)
        {
            if (!_hotkeys.TryGetValue(command, out var hotkeyCmd))
            {
                Logger.Get.Error($"Unknown hotkey command {command}");
                return;
            }

            if (hotkeyCmd.Actions.Contains(callback))
            {
                Logger.Get.Error($"{callback.Method.Name} is already registered for command {command}");
                return;
            }

            hotkeyCmd.Actions.Add(callback);
        }

        private void OnSettingsPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!(Properties.Hotkeys.Default[e.PropertyName] is VirtualKeyCode keyCode))
                return;
            if (!_hotkeys.TryGetValue(e.PropertyName, out var hotkeyCmd))
                return;

            hotkeyCmd.KeyCode = keyCode;

            Logger.Get.Success($"Changed hotkey for {e.PropertyName} to {keyCode}");
        }

        private void OnKeyboardEvent(VirtualKeyCode key, KeyState state)
        {
            Logger.Get.Debug($"KeyEvent {key} [{state}]");

            var hotkey = _hotkeys.FirstOrDefault(x => x.Value.KeyCode == key && x.Value.State == state);
            if (hotkey.Equals(default) || hotkey.Value is null)
                return;

            hotkey.Value.Actions.ForEach(x => x.BeginInvoke(x.EndInvoke, null));
        }
    }
}