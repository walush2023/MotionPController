using System;
using System.Diagnostics;
using System.Threading;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.Xbox360;

namespace MotionPController
{
    public class MortorEventArgs : EventArgs
    {
        public MortorEventArgs(byte largeMotor, byte smallMotor)
        {
            LargeMotor = largeMotor;
            SmallMotor = smallMotor;
        }

        public byte LargeMotor { get; set; }

        public byte SmallMotor { get; set; }
    }

    class Gamepad
    {
        private static ViGEmClient client;
        private IXbox360Controller controller;
        public bool init()
        {
            try
            {
                if (client == null)
                {
                    // initializes the SDK instance
                    client = new ViGEmClient();
                }
                return true;
            }
            catch (Nefarius.ViGEm.Client.Exceptions.VigemBusNotFoundException e)
            {
                Debug.WriteLine(e);
                return false;
            }
        }

        public void open()
        {
            Debug.WriteLine("Gamepad: create");

            try
            {
                // prepares a new Xbox360 controller
                controller = client.CreateXbox360Controller();

                try
                {
                    controller.Connect();
                }
                catch (System.ComponentModel.Win32Exception e)
                {
                    Debug.WriteLine(e);
                }

                // gamepadThread = new Thread(handle);
                controller.FeedbackReceived += Xbox360FeedbackReceivedEventMotionPController;
            }
            catch (Nefarius.ViGEm.Client.Exceptions.VigemBusNotFoundException e)
            {
                Debug.WriteLine(e);
            }
        }

        private enum Event
        {
            // Button
            Up,
            Down,
            Left,
            Right,
            A,
            B,
            X,
            Y,
            LeftShoulder,
            LeftThumb,
            RightShoulder,
            RightThumb,
            Start,
            Back,
            Guide,

            // Axis
            LeftThumbAxis,
            RightThumbAxis,

            // Slider
            LeftTrigger,
            RightTrigger,

            Reset
        }

        public void Trigger(byte[] buf, int bufIndex)
        {

            // 5: event, ispress, null, null, null
            // 5: event, lx, hx, ly, hy
            Event gamepadEvent = (Event)buf[bufIndex];
            byte value = buf[bufIndex + 1];
            short x = BitConverter.ToInt16(new byte[] { buf[bufIndex + 2], buf[bufIndex + 1] }, 0);
            short y = BitConverter.ToInt16(new byte[] { buf[bufIndex + 4], buf[bufIndex + 3] }, 0);

            Debug.WriteLine(string.Format("Gamepad: {0}, {1}, {2}, {3}", (Event)gamepadEvent, value, x, y));
            switch ((Event)gamepadEvent)
            {
                case Event.Up:
                    controller.SetButtonState(Xbox360Button.Up, Convert.ToBoolean(value));
                    break;
                case Event.Down:
                    controller.SetButtonState(Xbox360Button.Down, Convert.ToBoolean(value));
                    break;
                case Event.Left:
                    controller.SetButtonState(Xbox360Button.Left, Convert.ToBoolean(value));
                    break;
                case Event.Right:
                    controller.SetButtonState(Xbox360Button.Right, Convert.ToBoolean(value));
                    break;
                case Event.A:
                    controller.SetButtonState(Xbox360Button.A, Convert.ToBoolean(value));
                    break;
                case Event.B:
                    controller.SetButtonState(Xbox360Button.B, Convert.ToBoolean(value));
                    break;
                case Event.X:
                    controller.SetButtonState(Xbox360Button.X, Convert.ToBoolean(value));
                    break;
                case Event.Y:
                    controller.SetButtonState(Xbox360Button.Y, Convert.ToBoolean(value));
                    break;
                case Event.LeftShoulder:
                    controller.SetButtonState(Xbox360Button.LeftShoulder, Convert.ToBoolean(value));
                    break;
                case Event.LeftThumb:
                    controller.SetButtonState(Xbox360Button.LeftThumb, Convert.ToBoolean(value));
                    break;
                case Event.RightShoulder:
                    controller.SetButtonState(Xbox360Button.RightShoulder, Convert.ToBoolean(value));
                    break;
                case Event.RightThumb:
                    controller.SetButtonState(Xbox360Button.RightThumb, Convert.ToBoolean(value));
                    break;
                case Event.Start:
                    controller.SetButtonState(Xbox360Button.Start, Convert.ToBoolean(value));
                    break;
                case Event.Back:
                    controller.SetButtonState(Xbox360Button.Back, Convert.ToBoolean(value));
                    break;
                case Event.Guide:
                    controller.SetButtonState(Xbox360Button.Guide, Convert.ToBoolean(value));
                    break;
                case Event.LeftThumbAxis:
                    controller.SetAxisValue(Xbox360Axis.LeftThumbX, x);
                    controller.SetAxisValue(Xbox360Axis.LeftThumbY, y);
                    break;
                case Event.RightThumbAxis:
                    controller.SetAxisValue(Xbox360Axis.RightThumbX, x);
                    controller.SetAxisValue(Xbox360Axis.RightThumbY, y);
                    break;

                case Event.LeftTrigger:
                    controller.SetSliderValue(Xbox360Slider.LeftTrigger, value);
                    break;
                case Event.RightTrigger:
                    controller.SetSliderValue(Xbox360Slider.RightTrigger, value);
                    break;

                case Event.Reset:
                    for (int i = 0; i < Enum.GetNames(typeof(Event)).Length - 1; i++)
                    {
                        Trigger(new byte[] { (byte)i, 0, 0, 0, 0 }, 0);
                    }
                    break;
            }
        }

        private void handle()
        {
            bool timeout=false;
            // recommended: run this in its own thread
            while (true) {
                try
                {
                    // blocks for 250ms to not burn CPU cycles if no report is available
                    // an overload is available that blocks indefinitely until the device is disposed, your choice!
                    //var buffer = controller.AwaitRawOutputReport(250, out timedOut);

                    if (timeout)
                    {
                        Debug.WriteLine("Timed out");
                        continue;
                    }

                    // you got a new report, parse it and do whatever you need to do :)
                    // here we simply hex-dump the contents
                    //Debug.WriteLine(string.Join(" ", buffer.Select(b => b.ToString("X2"))));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                    Thread.Sleep(1000);
                }
            }
        }

        public event GamepadMotorEventMotionPController FeedbackReceived;
        public delegate void GamepadMotorEventMotionPController(byte largeMotor, byte smallMotor);

        void Xbox360FeedbackReceivedEventMotionPController(object sender, Xbox360FeedbackReceivedEventArgs e)
        {
            if (FeedbackReceived != null)
            {
                FeedbackReceived.Invoke(e.LargeMotor, e.SmallMotor);
            }
        }

        public void close()
        {
            FeedbackReceived = null;
            try
            {
                controller.Disconnect();
            }
            catch(Nefarius.ViGEm.Client.Exceptions.VigemTargetNotPluggedInException e) {

            }
        }
    }
}
