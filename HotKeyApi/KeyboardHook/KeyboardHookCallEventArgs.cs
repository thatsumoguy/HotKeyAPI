using System;
using System.Windows.Forms;

namespace HotKeyApi
{
    /// <summary>
    /// Event Args for the event that is fired after the hot key has been pressed.
    /// </summary>
    public class KeyboardHookCallEventArgs : EventArgs
    {
        private ModifierKeys _modifier;
        private Keys _key;

        public KeyboardHookCallEventArgs(ModifierKeys modifier, Keys key)
        {
            _modifier = modifier;
            _key = key;
        }

        public ModifierKeys Modifier => _modifier;
        public Keys Key => _key;
    }

    /// <summary>
    /// The enumeration of possible modifiers.
    /// </summary>
    [Flags]
    public enum ModifierKeys : uint
    {
        Alt = 1,
        Control = 2,
        Shift = 4,
        Win = 8,
        NoRepeat = 16384
    }
}
