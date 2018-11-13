using System;
using System.Windows.Forms;

namespace HotKeyApi
{
    internal class KeyboardHook
    {
        private int _currentID = 0;
        private static CreateWindow _window = new CreateWindow();

        internal int currentID { get => _currentID; }

        /// <summary>
        /// Event handler when Window detects a WM_Hotkey message
        /// </summary>
        internal event EventHandler<KeyboardHookCallEventArgs> KeyPressed;
        

        /// <summary>
        /// internal constructor of KeyboardHook 
        /// </summary>
        internal KeyboardHook() : this(_window)
        {
        }
        private KeyboardHook(CreateWindow window)
        {
            _window.KeyPressed += (s, e) =>
            {
                KeyPressed?.Invoke(this, e);
            };
            _window = window;
        }
        
        /// <summary>
        /// Takes in two strings and tries to register those into a hotkey.
        /// </summary>
        /// <param name="modkey">A string with Modifier Key names</param>
        /// <param name="key">A string with Keys name</param>
        internal void RegisterHotKey(string modkey, string key)
        {
            var modifiers = GetModKeys(modkey);
            _currentID++;
            var keys = (Keys)Enum.Parse(typeof(Keys), key);
            WinImportApi.RegisterHotKey(_window.Handle, _currentID, modifiers, (int)keys);
        }
        
        internal ModifierKeys GetModKeys(string modkey)
        {
            var mod = ModifierKeys.NoRepeat;
            if (modkey.Contains(", "))
            {
                var modkeys = modkey.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var mods in modkeys)
                {
                    switch (mods)
                    {
                        case "Alt":
                            mod |= ModifierKeys.Alt;
                            break;
                        case "Shift":
                            mod |= ModifierKeys.Shift;
                            break;
                        case "Control":
                            mod |= ModifierKeys.Control;
                            break;
                        case "Win":
                            mod |= ModifierKeys.Win;
                            break;
                        default:
                            mod = GetModKeys(modkey);
                            break;
                    }
                }
            }
            else
            {
                switch (modkey)
                {
                    case "Alt":
                        mod |= ModifierKeys.Alt;
                        break;
                    case "Shift":
                        mod |= ModifierKeys.Shift;
                        break;
                    case "Control":
                        mod |= ModifierKeys.Control;
                        break;
                    case "Win":
                        mod |= ModifierKeys.Win;
                        break;
                    default:
                        mod = GetModKeys(modkey);
                        break;
                }
            }
            return mod;
        }
        internal bool GetComBool (string command)
        {
            switch(command.ToLower())
            {
                case "false": return false;
                case "true": return true;
                default: throw new InvalidCastException("Item must be True or False");
            }
        }
        
        /// <summary>
        /// Clears all of the registered HotKeys
        /// </summary>
        internal void ClearHotKeys()
        {
            for (int i = _currentID; i > 0; i--)
            {
                WinImportApi.UnregisterHotKey(_window.Handle, i);
            }
            _currentID = 0;
        }

        ~KeyboardHook()
        {
            _window.Dispose();
        }
    }
}
