using System;
using System.Runtime.InteropServices;

namespace MotionPController
{
    class Resolution
    {
        [DllImport("User32.dll")]
        static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("User32.dll")]
        static extern int ReleaseDC(IntPtr hwnd, IntPtr dc);

        [DllImport("gdi32.dll")]
        static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

        public static int GetX() {
            
            IntPtr primary = Resolution.GetDC(IntPtr.Zero);
            int DESKTOPHORZRES = 118;
            int actualPixelsX = GetDeviceCaps(primary, DESKTOPHORZRES);
            ReleaseDC(IntPtr.Zero, primary);
            return actualPixelsX;
        }

        public static int GetY() {
            
            IntPtr primary = Resolution.GetDC(IntPtr.Zero);
            int DESKTOPVERTRES = 117;
            int actualPixelsY = GetDeviceCaps(primary, DESKTOPVERTRES);
            ReleaseDC(IntPtr.Zero, primary);
            return actualPixelsY;
        }
    }
}
