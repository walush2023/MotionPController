using Microsoft.Win32;
using System.Diagnostics;
using System.Net;
using QRCoder;

namespace MotionPController
{
    public partial class Form1 : Form
    {
        SocketHandler socket;
        bool runQrcodeThread = true;
        bool duplicateProc = false;

        public Form1()
        {
            InitializeComponent();

            foreach (Process proc in Process.GetProcesses())
            {
                if (proc.ProcessName.Equals(Process.GetCurrentProcess().ProcessName) && proc.Id != Process.GetCurrentProcess().Id)
                {
                    duplicateProc = true;
                    proc.Kill();
                }
            }

            new Thread(qrcodeThreadCreate).Start();
        }
        private void qrcodeThreadCreate()
        {
            socket = new SocketHandler();

            while (runQrcodeThread)
            {

                QRCodeGenerator qrGenerator = new QRCodeGenerator();

                // Retrieve host ip
                IPAddress[] ipList = SocketHandler.getHostIP();
                if (ipList == null)
                    this.pictureBox1.Image = null;
                else
                {

                    String data = "#";
                    foreach (IPAddress ip in ipList)
                    {
                        if (!socket.tcpExist(ip))
                        {
                            new Thread(() => socket.tcpThreadCreate(ip)).Start();
                        }
                        data += "," + ip.ToString();
                    }

                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(encodeData(data), QRCodeGenerator.ECCLevel.Q);
                    QRCode qrCode = new QRCode(qrCodeData);
                    Bitmap qrCodeImage = qrCode.GetGraphic(20);
                    Bitmap resizedImage = new Bitmap(qrCodeImage, new Size(this.Width, this.Height - 23));
                    this.pictureBox1.Image = resizedImage;
                }

                Thread.Sleep(1000);
            }

            Debug.WriteLine("QRcode thread dispose");
        }

        private string encodeData(string data)
        {
            byte[] array = System.Text.Encoding.UTF8.GetBytes(data);
            for (int i = 0; i < array.Length; i++)
            {
                array[i] ^= 0x23;
            }
            return System.Text.Encoding.UTF8.GetString(array); ;
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                this.notifyIcon1.Visible = true;
            }
            else
            {
                this.notifyIcon1.Visible = false;

                if (this.Width == this.Height - 23) return;

                if (this.Width > this.Height - 23)
                {
                    this.Height = this.Width + 23;
                }
                else
                {
                    this.Width = this.Height - 23;
                }
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Debug.WriteLine("Closing");
            runQrcodeThread = false;
            socket.close();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Close the form, which closes the application.
            this.Close();
        }

        private void autoStartToolStripMenuItem_Click(object sender, EventArgs e)
        {

            this.autoStartToolStripMenuItem.Checked = !this.autoStartToolStripMenuItem.Checked;

            RegistryKey rk = Registry.CurrentUser.OpenSubKey
                ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (this.autoStartToolStripMenuItem.Checked)
                rk.SetValue("MotionPController", Application.ExecutablePath);
            else
                rk.DeleteValue("MotionPController", false);
        }
        private void Form1_Shown(object sender, EventArgs e)
        {
            if (duplicateProc)
                return;

            RegistryKey rk = Registry.CurrentUser.OpenSubKey
                ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (rk.GetValue("MotionPController") != null)
            {
                this.autoStartToolStripMenuItem.Checked = true;
                this.WindowState = FormWindowState.Minimized;
            }
        }
    }
}
