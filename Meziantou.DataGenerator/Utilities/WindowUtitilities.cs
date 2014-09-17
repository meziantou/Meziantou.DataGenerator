using System;
using System.Windows;
using System.Windows.Interop;

namespace Meziantou.DataGenerator.Utilities
{
    public static class WindowUtitilities
    {
        public class Win32Window : System.Windows.Forms.IWin32Window, System.Windows.Interop.IWin32Window
        {
            public IntPtr Handle { get; private set; }

            public Win32Window(Window wpfWindow)
            {
                Handle = new WindowInteropHelper(wpfWindow).Handle;
            }
        }

        public static Win32Window AsWin32Window(this Window window)
        {
            return new Win32Window(window);
        }
    }
}
