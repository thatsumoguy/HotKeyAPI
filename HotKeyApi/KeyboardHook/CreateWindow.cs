using System;
using System.Windows.Forms;

namespace HotKeyApi
{
    public class CreateWindow : NativeWindow, IDisposable
    {
        private const int WM_HOTKEY = 0x0312;

        public CreateWindow()
        {
            CreateHandle(new CreateParams());
        }

        protected override void WndProc(ref Message msg)
        {

            // check if we got a hot key pressed.
            if (msg.Msg == WM_HOTKEY)
            {
                const int shiftbit = 16;
                const int compare = 0xFFFF;
                // get the keys.
                Keys key = (Keys)(((int)msg.LParam >> shiftbit) & compare);
                ModifierKeys modifier = (ModifierKeys)((int)msg.LParam & compare);
                // invoke the event to notify the parent.
                KeyPressed?.Invoke(this, new KeyboardHookCallEventArgs(modifier, key));
            }
            base.WndProc(ref msg);
        }

        public event EventHandler<KeyboardHookCallEventArgs> KeyPressed;

        public void Dispose()
        {
            DestroyHandle();
        }
    }
}
