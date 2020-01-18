using LowLevelInput.Hooks;
using PoeSuite.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PoeSuite
{
    internal class HotkeyCommand
    {
        public VirtualKeyCode KeyCode;
        public KeyState State = KeyState.Pressed;
        public List<Action> Actions = new List<Action>();
    }

    internal class HotkeysManager
    {
        private readonly LowLevelKeyboardHook _keyboardHook;
        
        private Dictionary<string, HotkeyCommand> _hotkeys;



        private readonly Dictionary<(VirtualKeyCode, KeyState), List<Action>> _listeners;


        public HotkeysManager()
        {
            _keyboardHook = new LowLevelKeyboardHook(true);


            _listeners = new Dictionary<(VirtualKeyCode, KeyState), List<Action>>();


            _hotkeys = new Dictionary<string, HotkeyCommand>();


            // load stored hotkeys
            foreach (System.Configuration.SettingsProperty key in Properties.Hotkeys.Default.Properties)
            {
                var value = (VirtualKeyCode)Properties.Hotkeys.Default[key.Name];
                _hotkeys.Add(key.Name, new HotkeyCommand { KeyCode = value });

                App.Log.Info($"Loaded hotkey {value} for {key.Name}");
            }


            // listen to settings changes
            Properties.Hotkeys.Default.PropertyChanged += OnSettingsPropertyChanged;
            _keyboardHook.OnKeyboardEvent += OnKeyboardEvent;

            _keyboardHook.InstallHook();
        }

        private void OnSettingsPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (_hotkeys.ContainsKey(e.PropertyName))
            {
                var updatedValue = (VirtualKeyCode)Properties.Hotkeys.Default[e.PropertyName];
                _hotkeys[e.PropertyName].KeyCode = updatedValue;

                App.Log.Success($"Changed hotkey for {e.PropertyName} to {updatedValue.ToString()}");
            }
        }

        public void AddHotkey(VirtualKeyCode key, KeyState state, Action action)
        {
            if (_listeners.TryGetValue((key, state), out var actions))
            {
                if (actions.Contains(action))
                    throw new ArgumentException("Listeners already contains this actions");

                actions.Add(action);
            }
            else
                _listeners.Add((key, state), new List<Action> { action });
        }

        public void AddHotkey()
        {

        }

        public void AddCallback(string command, Action callback)
        {
            if (!_hotkeys.Any(x => x.Key.Equals(command)))
            {
                App.Log.Error($"Unknown hotkey command {command}");
                return;
            }

            if (_hotkeys[command].Actions.Contains(callback))
            {
                App.Log.Warning($"{callback.Method.Name} is already registered for command {command}");
                return;
            }

            _hotkeys[command].Actions.Add(callback);
        }

        public void AddCallbacks(string command, List<Action> callbacks)
        {
            foreach (var c in callbacks)
                AddCallback(command, c);
        }

        public void RemoveCallback(Action action)
        {
            var test = _listeners.Where(x => x.Value.Contains(action));
            if (test is null)
                return;

            foreach (var miep in test)
                miep.Value.Remove(action);
        }

        private void OnKeyboardEvent(VirtualKeyCode key, KeyState state)
        {
            Console.WriteLine("Hotkey pressed: " + key.ToString());

            var command = _hotkeys.First(x => x.Value.KeyCode == key /* && x.Value.State == state*/).Value;

            if (!(command is null))
            {
                foreach (var action in command.Actions)
                    action.BeginInvoke(action.EndInvoke, null);
            }

            if (state == KeyState.Pressed)
            {
                if (!_listeners.TryGetValue((key, state), out var actions))
                    return;

                actions.ForEach(x => x.Invoke());
            }


        }
    }
}