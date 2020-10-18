﻿using PoeSuite.DataTypes;
using PoeSuite.Utilities;

using LowLevelInput.Hooks;

using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System;
using GalaSoft.MvvmLight.Messaging;
using PoeSuite.Messages;

namespace PoeSuite
{
    internal class HotkeysManager
    {
        private static HotkeysManager _instance = null;
        private readonly Dictionary<string, HotkeyCommand> _hotkeys;
        private VirtualKeyCode _lastModifier;

        public static HotkeysManager Get => _instance ?? (_instance = new HotkeysManager());
        public bool IsEnabled { get; set; }

        private HotkeysManager()
        {
            var keyboardHook = new LowLevelKeyboardHook(true);
            var mouseHook = new LowLevelMouseHook(true);

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

            keyboardHook.OnKeyboardEvent += OnKeyboardEvent;
            keyboardHook.InstallHook();

            mouseHook.OnMouseEvent += MouseHook_OnMouseEvent;
            mouseHook.InstallHook();

            Messenger.Default.Register<GameActiveStatusChanged>(this, msg =>
            {
                mouseHook.CaptureMouseMove = msg.IsInForeground;
            });
        }

        public bool AddModifier(string command, VirtualKeyCode mod)
        {
            if (!IsModifierKey(mod))
            {
                Logger.Get.Error($"{mod} is not a valid modifier key!");
                return false;
            }

            if (!_hotkeys.TryGetValue(command, out var hotkeyCmd))
            {
                Logger.Get.Error($"Unknown hotkey command {command}");
                return false;
            }

            hotkeyCmd.Modifier = mod;

            return true;
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

        public void AddCallback(string command, Action<VirtualKeyCode, int, int> callback)
        {
            if (!_hotkeys.TryGetValue(command, out var hotkeyCmd))
            {
                Logger.Get.Error($"Unknown hotkey command {command}");
                return;
            }

            if (hotkeyCmd.MouseActions.Contains(callback))
            {
                Logger.Get.Error($"{callback.Method.Name} is already registered for command {command}");
                return;
            }

            hotkeyCmd.MouseActions.Add(callback);
        }

        public void ClearCallbacks(string command)
        {
            if (!_hotkeys.TryGetValue(command, out var hotkeyCmd))
            {
                Logger.Get.Error($"Unknown hotkey command {command}");
                return;
            }

            hotkeyCmd.Actions.Clear();
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
            //Logger.Get.Debug($"KeyEvent {key} [{state}]");

            if (!IsEnabled)
                return;

            if (IsModifierKey(key) && state == KeyState.Down)
                _lastModifier = key;
            else if (key == _lastModifier && state == KeyState.Up)
                _lastModifier = VirtualKeyCode.Hotkey;

            var hotkey = _hotkeys.FirstOrDefault(x => x.Value.KeyCode == key && x.Value.State == state);
            if (hotkey.Equals(default) || hotkey.Value is null)
                return;

            hotkey.Value.Actions.ForEach(x =>
            {
                if (hotkey.Value.Modifier == VirtualKeyCode.Invalid || hotkey.Value.Modifier == _lastModifier)
                    x.BeginInvoke(x.EndInvoke, null);
            });
        }

        private void MouseHook_OnMouseEvent(VirtualKeyCode key, KeyState state, int xz, int yz)
        {
            //Logger.Get.Debug($"MouseEvent {key} [{state}]");

            if (!IsEnabled/* || key == VirtualKeyCode.Invalid*/)
                return;
            
            var hotkey = _hotkeys.FirstOrDefault(x => x.Value.KeyCode == key);
            if (hotkey.Equals(default) || hotkey.Value is null)
                return;

            hotkey.Value.MouseActions.ForEach(x =>
            {
                if (hotkey.Value.Modifier == VirtualKeyCode.Invalid || hotkey.Value.Modifier == _lastModifier)
                    x.BeginInvoke(key, xz, yz, x.EndInvoke, null);
            });
            
        }

        // TODO: make extension method?
        private static bool IsModifierKey(VirtualKeyCode key)
        {
            return key == VirtualKeyCode.Lmenu || key == VirtualKeyCode.Lcontrol || key == VirtualKeyCode.Lshift;
        }
    }
}