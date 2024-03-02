

namespace MotionPController
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (!new Gamepad().init())
            {
                if (MessageBox.Show("Please install ViGEm Bus before execute.\n\nVisit download page ?", "Error",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Hand) == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start("https://github.com/ViGEm/ViGEmBus/releases/latest");
                }
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }
}