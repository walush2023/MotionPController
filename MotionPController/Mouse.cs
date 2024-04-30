using System.Runtime.InteropServices;
using System.Diagnostics;
using System;
using System.Threading.Tasks;

namespace MotionPController
{
    class Mouse
    {
        private const int MOUSEEVENTF_MOVE = 0x0001; /* mouse move */
        private const int MOUSEEVENTF_LEFTDOWN = 0x0002; /* left button down */
        private const int MOUSEEVENTF_LEFTUP = 0x0004; /* left button up */
        private const int MOUSEEVENTF_RIGHTDOWN = 0x0008; /* right button down */
        private const int MOUSEEVENTF_RIGHTUP = 0x0010; /* right button up */
        private const int MOUSEEVENTF_MIDDLEDOWN = 0x0020; /* middle button down */
        private const int MOUSEEVENTF_MIDDLEUP = 0x0040; /* middle button up */
        private const int MOUSEEVENTF_XDOWN = 0x0080; /* x button down */
        private const int MOUSEEVENTF_XUP = 0x0100; /* x button down */
        private const int MOUSEEVENTF_WHEEL = 0x0800; /* wheel button rolled */
        private const int MOUSEEVENTF_HWHEEL = 0x1000; /* wheel button tilted */
        private const int MOUSEEVENTF_VIRTUALDESK = 0x4000; /* map to entire virtual desktop */
        private const int MOUSEEVENTF_ABSOLUTE = 0x8000; /* absolute move */

        private static int WHEEL_DELTA = 120;

        [StructLayout(LayoutKind.Sequential)]
        public struct Point
        {
            public int X;
            public int Y;

            public Point(int x, int y)
            {
                X = x;
                Y = y;
            }
        }

        [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out Point lpMousePoint);

        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        [DllImport("kernel32.dll")]
        extern static short QueryPerformanceCounter(ref long x);
        [DllImport("kernel32.dll")]
        extern static short QueryPerformanceFrequency(ref long x);

        public static Point GetCursorPosition()
        {
            Point currentMousePoint;
            var gotPoint = GetCursorPos(out currentMousePoint);
            if (!gotPoint) { currentMousePoint = new Point(0, 0); }
            return currentMousePoint;
        }

        private enum Event
        {
            Move,
            MoveOffset,
            LeftDown,
            LeftUp,
            LeftClick,
            RightDown,
            RightUp,
            RightClick,
            WheelForward,
            WheelBackward,
            WheelLeft,
            WheelRight
        }

        public static void Trigger(byte[] buf, int bufIndex)
        {
            // 5: event, lx, hx, ly, hy
            // check big little endian
            Event mouseEvent = (Event)buf[bufIndex];
            int x = BitConverter.ToInt16(new byte[] { buf[bufIndex + 2], buf[bufIndex + 1] }, 0);
            int y = BitConverter.ToInt16(new byte[] { buf[bufIndex + 4], buf[bufIndex + 3] }, 0);

            Point pt = GetCursorPosition();
            Debug.WriteLine(string.Format("Mouse: {0}, {1}, {2}, posX {3}, posY {4}",
                (Event)mouseEvent, x, y, pt.X, pt.Y));

            switch (mouseEvent)
            {
                case Event.Move:
                    SetCursorPos(pt.X + x, pt.Y + y);
                    break;
                case Event.MoveOffset:
                    int rx = Resolution.GetX();
                    int ry = Resolution.GetY();

                    int mx = (int)(rx * (x / 32767.0));
                    int my = (int)(ry * (y / 32767.0));

                    SetCursorPos(mx, my);
                    break;
                case Event.LeftDown:
                    mouse_event(MOUSEEVENTF_LEFTDOWN, pt.X, pt.Y, 0, 0);
                    break;
                case Event.LeftUp:
                    mouse_event(MOUSEEVENTF_LEFTUP, pt.X, pt.Y, 0, 0);
                    break;
                case Event.LeftClick:
                    mouse_event(MOUSEEVENTF_LEFTDOWN, pt.X, pt.Y, 0, 0);
                    mouse_event(MOUSEEVENTF_LEFTUP, pt.X, pt.Y, 0, 0);
                    break;
                case Event.RightDown:
                    mouse_event(MOUSEEVENTF_RIGHTDOWN, pt.X, pt.Y, 0, 0);
                    break;
                case Event.RightUp:
                    mouse_event(MOUSEEVENTF_RIGHTUP, pt.X, pt.Y, 0, 0);
                    break;
                case Event.RightClick:
                    mouse_event(MOUSEEVENTF_RIGHTDOWN, pt.X, pt.Y, 0, 0);
                    mouse_event(MOUSEEVENTF_RIGHTUP, pt.X, pt.Y, 0, 0);
                    break;
                case Event.WheelForward:
                    mouse_event(MOUSEEVENTF_WHEEL, pt.X, pt.Y, WHEEL_DELTA, 0);
                    break;
                case Event.WheelBackward:
                    mouse_event(MOUSEEVENTF_WHEEL, pt.X, pt.Y, -1 * WHEEL_DELTA, 0);
                    break;
                case Event.WheelLeft:
                    mouse_event(MOUSEEVENTF_HWHEEL, pt.X, pt.Y, -1 * WHEEL_DELTA, 0);
                    break;
                case Event.WheelRight:
                    mouse_event(MOUSEEVENTF_HWHEEL, pt.X, pt.Y, WHEEL_DELTA, 0);
                    break;
            }
        }

        private static void LinearSmoothMove(int x, int y, int steps)
        {
            Point newPosition = new Point(x, y);
            Point start = GetCursorPosition();
            Point iterPoint = start;

            long freq = 0;
            QueryPerformanceFrequency(ref freq);  //獲取CPU頻率

            // Find the slope of the line segment defined by start and newPosition
            Point slope = new Point(newPosition.X - start.X, newPosition.Y - start.Y);

            // Divide by the number of steps
            slope.X = slope.X / steps;
            slope.Y = slope.Y / steps;

            // Move the mouse to each iterative point.
            for (int i = 0; i < steps; i++)
            {
                iterPoint = new Point(iterPoint.X + slope.X, iterPoint.Y + slope.Y);
                SetCursorPos(iterPoint.X, iterPoint.Y);
                //Thread.Sleep(1);
                Task.Delay(new TimeSpan(100));
            }

            // Move the mouse to the final destination.
            SetCursorPos(newPosition.X, newPosition.Y);
        }

        private static void LinearSmoothMoveOffset(int x, int y, int steps)
        {
            Point start = GetCursorPosition();
            Point newPosition = new Point(start.X + x, start.Y+y);
            Point iterPoint = start;

            // Find the slope of the line segment defined by start and newPosition
            Point slope = new Point(newPosition.X - start.X, newPosition.Y - start.Y);

            // Divide by the number of steps
            slope.X = slope.X / steps;
            slope.Y = slope.Y / steps;

            // Move the mouse to each iterative point.
            for (int i = 0; i < steps; i++)
            {
                iterPoint = new Point(iterPoint.X + slope.X, iterPoint.Y + slope.Y);
                SetCursorPos(iterPoint.X, iterPoint.Y);
                //Thread.Sleep(new TimeSpan(1));
                Task.Delay(new TimeSpan(10));
            }

            // Move the mouse to the final destination.
            SetCursorPos(newPosition.X, newPosition.Y);
        }
    }
}
