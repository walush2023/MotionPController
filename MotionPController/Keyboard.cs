using System.Runtime.InteropServices;
using System.Diagnostics;

namespace MotionPController
{
    class Keyboard
    {
        // https://learn.microsoft.com/zh-tw/windows/win32/inputdev/virtual-key-codes
        private const byte VK_LBUTTON = 0x01;
        private const byte VK_RBUTTON = 0x02;
        private const byte VK_BACK = 0x08;
        private const byte VK_TAB = 0x09;
        private const byte VK_CLEAR = 0x0C;
        private const byte VK_RETURN = 0x0D;
        private const byte VK_SHIFT = 0x10;
        private const byte VK_CONTROL = 0x11;
        private const byte VK_MENU = 0x12;
        private const byte VK_PAUSE = 0x13;
        private const byte VK_CAPITAL = 0x14;
        private const byte VK_ESCAPE = 0x1B;
        private const byte VK_SPACE = 0x20;
        private const byte VK_PRIOR = 0x21;
        private const byte VK_NEXT = 0x22;
        private const byte VK_END = 0x23;
        private const byte VK_HOME = 0x24;
        private const byte VK_LEFT = 0x25;
        private const byte VK_UP = 0x26;
        private const byte VK_RIGHT = 0x27;
        private const byte VK_DOWN = 0x28;
        private const byte VK_SELECT = 0x29;
        private const byte VK_PRbyte = 0x2A;
        private const byte VK_EXECUTE = 0x2B;
        private const byte VK_SNAPSHOT = 0x2C;
        private const byte VK_INSERT = 0x2D;
        private const byte VK_DELETE = 0x2E;
        private const byte VK_0 = 0x30;
        private const byte VK_1 = 0x31;
        private const byte VK_2 = 0x32;
        private const byte VK_3 = 0x33;
        private const byte VK_4 = 0x34;
        private const byte VK_5 = 0x35;
        private const byte VK_6 = 0x36;
        private const byte VK_7 = 0x37;
        private const byte VK_8 = 0x38;
        private const byte VK_9 = 0x39;
        private const byte VK_A = 0x41;
        private const byte VK_B = 0x42;
        private const byte VK_C = 0x43;
        private const byte VK_D = 0x44;
        private const byte VK_E = 0x45;
        private const byte VK_F = 0x46;
        private const byte VK_G = 0x47;
        private const byte VK_H = 0x48;
        private const byte VK_I = 0x49;
        private const byte VK_J = 0x4A;
        private const byte VK_K = 0x4B;
        private const byte VK_L = 0x4C;
        private const byte VK_M = 0x4D;
        private const byte VK_N = 0x4E;
        private const byte VK_O = 0x4F;
        private const byte VK_P = 0x50;
        private const byte VK_Q = 0x51;
        private const byte VK_R = 0x52;
        private const byte VK_S = 0x53;
        private const byte VK_T = 0x54;
        private const byte VK_U = 0x55;
        private const byte VK_V = 0x56;
        private const byte VK_W = 0x57;
        private const byte VK_X = 0x58;
        private const byte VK_Y = 0x59;
        private const byte VK_Z = 0x5A;
        private const byte VK_LWIN = 0x5B;
        private const byte VK_RWIN = 0x5C;
        private const byte VK_APPS = 0x5D;
        private const byte VK_SLEEP = 0x5F;
        private const byte VK_NUMPAD0 = 0x60;
        private const byte VK_NUMPAD1 = 0x61;
        private const byte VK_NUMPAD2 = 0x62;
        private const byte VK_NUMPAD3 = 0x63;
        private const byte VK_NUMPAD4 = 0x64;
        private const byte VK_NUMPAD5 = 0x65;
        private const byte VK_NUMPAD6 = 0x66;
        private const byte VK_NUMPAD7 = 0x67;
        private const byte VK_NUMPAD8 = 0x68;
        private const byte VK_NUMPAD9 = 0x69;
        private const byte VK_MULTIPLY = 0x6A;
        private const byte VK_ADD = 0x6B;
        private const byte VK_SEPARATOR = 0x6C;
        private const byte VK_SUBTRACT = 0x6D;
        private const byte VK_DECIMAL = 0x6E;
        private const byte VK_DIVIDE = 0x6F;
        private const byte VK_F1 = 0x70;
        private const byte VK_F2 = 0x71;
        private const byte VK_F3 = 0x72;
        private const byte VK_F4 = 0x73;
        private const byte VK_F5 = 0x74;
        private const byte VK_F6 = 0x75;
        private const byte VK_F7 = 0x76;
        private const byte VK_F8 = 0x77;
        private const byte VK_F9 = 0x78;
        private const byte VK_F10 = 0x79;
        private const byte VK_F11 = 0x7A;
        private const byte VK_F12 = 0x7B;
        private const byte VK_F13 = 0x7C;
        private const byte VK_F14 = 0x7D;
        private const byte VK_F15 = 0x7E;
        private const byte VK_F16 = 0x7F;
        private const byte VK_F17 = 0x80;
        private const byte VK_F18 = 0x81;
        private const byte VK_F19 = 0x82;
        private const byte VK_F20 = 0x83;
        private const byte VK_F21 = 0x84;
        private const byte VK_F22 = 0x85;
        private const byte VK_F23 = 0x86;
        private const byte VK_F24 = 0x87;
        private const byte VK_NUMLOCK = 0x90;
        private const byte VK_SCROLL = 0x91;
        private const byte VK_LSHIFT = 0xA0;
        private const byte VK_RSHIFT = 0xA1;
        private const byte VK_LCONTROL = 0xA2;
        private const byte VK_RCONTROL = 0xA3;
        private const byte VK_LMENU = 0xA4;
        private const byte VK_RMENU = 0xA5;
        private const byte VK_VOLUME_MUTE = 0xAD;
        private const byte VK_VOLUME_DOWN = 0xAE;
        private const byte VK_VOLUME_UP = 0xAF;
        private const byte VK_MEDIA_NEXT_TRACK = 0xB0;
        private const byte VK_MEDIA_PREV_TRACK = 0xB1;
        private const byte VK_MEDIA_STOP = 0xB2;
        private const byte VK_MEDIA_PLAY_PAUSE = 0xB3;
        private const byte VK_LAUNCH_MAIL = 0xB4;
        private const byte VK_LAUNCH_MEDIA_SELECT = 0xB5;
        private const byte VK_LAUNCH_APP1 = 0xB6;
        private const byte VK_LAUNCH_APP2 = 0xB7;
        private const int VK_OEM_1 = 0xBA; // ';:' for US
        private const int VK_OEM_PLUS = 0xBB; //  '+' any country
        private const int VK_OEM_COMMA = 0xBC; //  ',' any country
        private const int VK_OEM_MINUS = 0xBD; //  '-' any country
        private const int VK_OEM_PERIOD = 0xBE; //  '.' any country
        private const int VK_OEM_2 = 0xBF; //  '/?' for US
        private const int VK_OEM_3 = 0xC0; //  '`~' for US

        private const int KEYEVENTF_KEYDOWN = 0x0000;
        private const int KEYEVENTF_EXTENDEDKEY = 0x0001;
        private const int KEYEVENTF_KEYUP = 0x0002;

        [DllImport("user32.dll", EntryPoint = "keybd_event", SetLastError = true)]
        private static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);
        private enum Event
        {
            Click,
            KeyDown,
            KeyUp,
            ScaleUp,
            ScaleDown,
            ActionCenter,
            TaskPress,
            TaskRelease,
            TaskCenter,
            Desktop,
            VirtualDesktopLeft,
            VirtualDesktopRight,
            Back
        }
        private static void Combination(byte[] code, int[] downUp)
        {
            int len = code.Length;
            for (int i = 0; i < len; i++)
            {
                keybd_event(code[i], 0, KEYEVENTF_EXTENDEDKEY | downUp[i], 0);
            }
        }
        public static void Trigger(byte[] buf, int bufIndex)
        {
            // 2: event, code
            Event keybdEvent = (Event)buf[bufIndex];
            byte code = buf[bufIndex+1];

            Debug.WriteLine(string.Format("Keyboard: {0}, {1}", (Event)keybdEvent, code));

            switch ((Event)keybdEvent)
            {
                case Event.Click:
                    keybd_event(code, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYDOWN, 0);
                    keybd_event(code, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
                    break;
                case Event.KeyDown:
                    keybd_event(code, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYDOWN, 0);
                    break;
                case Event.KeyUp:
                    keybd_event(code, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
                    break;
                case Event.ScaleUp:
                    Combination(
                        new byte[] { VK_CONTROL, VK_ADD, VK_ADD, VK_CONTROL },
                        new int[] { KEYEVENTF_KEYDOWN, KEYEVENTF_KEYDOWN, 
                                    KEYEVENTF_KEYUP, KEYEVENTF_KEYUP }
                    );
                    break;
                case Event.ScaleDown:
                    Combination(
                        new byte[] { VK_CONTROL, VK_SUBTRACT, VK_SUBTRACT, VK_CONTROL },
                        new int[] { KEYEVENTF_KEYDOWN, KEYEVENTF_KEYDOWN, 
                                    KEYEVENTF_KEYUP, KEYEVENTF_KEYUP }
                    );
                    break;
                case Event.ActionCenter:
                    Combination(
                        new byte[] { VK_LWIN, VK_A, VK_A, VK_LWIN },
                        new int[] { KEYEVENTF_KEYDOWN, KEYEVENTF_KEYDOWN, 
                                    KEYEVENTF_KEYUP, KEYEVENTF_KEYUP }
                    );
                    break;
                case Event.TaskPress:
                    Combination(
                        new byte[] { VK_MENU, VK_TAB, VK_TAB },
                        new int[] { KEYEVENTF_KEYDOWN, KEYEVENTF_KEYDOWN, KEYEVENTF_KEYUP }
                    );
                    break;
                case Event.TaskRelease:
                    Combination(
                        new byte[] { VK_MENU },
                        new int[] { KEYEVENTF_KEYUP }
                    );
                    break;
                case Event.TaskCenter:
                    Combination(
                        new byte[] { VK_LWIN, VK_TAB, VK_TAB, VK_LWIN },
                        new int[] { KEYEVENTF_KEYDOWN, KEYEVENTF_KEYDOWN, 
                                    KEYEVENTF_KEYUP, KEYEVENTF_KEYUP }
                    );
                    break;
                case Event.Desktop:
                    Combination(
                        new byte[] { VK_LWIN, VK_D, VK_D, VK_LWIN },
                        new int[] { KEYEVENTF_KEYDOWN, KEYEVENTF_KEYDOWN, 
                                    KEYEVENTF_KEYUP, KEYEVENTF_KEYUP }
                    );
                    break;
                case Event.VirtualDesktopLeft:
                    Combination(
                        new byte[] { VK_CONTROL, VK_LWIN, VK_LEFT, VK_LEFT, VK_LWIN, VK_CONTROL },
                        new int[] { KEYEVENTF_KEYDOWN, KEYEVENTF_KEYDOWN, KEYEVENTF_KEYDOWN,
                                    KEYEVENTF_KEYUP, KEYEVENTF_KEYUP, KEYEVENTF_KEYUP }
                    );
                    break;
                case Event.VirtualDesktopRight:
                    Combination(
                        new byte[] { VK_CONTROL, VK_LWIN, VK_RIGHT, VK_RIGHT, VK_LWIN, VK_CONTROL },
                        new int[] { KEYEVENTF_KEYDOWN, KEYEVENTF_KEYDOWN, KEYEVENTF_KEYDOWN,
                                    KEYEVENTF_KEYUP, KEYEVENTF_KEYUP, KEYEVENTF_KEYUP }
                    );
                    break;
                case Event.Back:
                    Combination(
                        new byte[] { VK_TAB, VK_TAB, VK_MENU, VK_LEFT, VK_LEFT, VK_MENU },
                        new int[] { KEYEVENTF_KEYDOWN, KEYEVENTF_KEYUP, KEYEVENTF_KEYDOWN,
                            KEYEVENTF_KEYDOWN, KEYEVENTF_KEYUP, KEYEVENTF_KEYUP }
                    );
                    break;
            }
        }
    }
}
