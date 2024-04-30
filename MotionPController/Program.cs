using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using QRCoder;

namespace MotionPController
{
    internal static class Program
    {
        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (!new Gamepad().init())
            {
                if (MessageBox.Show("Please install ViGEm Bus before execute.\n\nVisit download page ?", "Error",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Hand) == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start("explorer", "https://github.com/ViGEm/ViGEmBus/releases/latest");
                }
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
