using System;
using System.Runtime.InteropServices;

namespace HotKeyApi
{
    public class WinImportApi
    {
        private const int VK_SHIFT = 0x10;

        private const int VK_CONTROL = 0x11;

        private const int VK_MENU = 0x12;

        private const int VK_CAPITAL = 0x14;
       
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, ModifierKeys fsModifiers, int vk);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}
