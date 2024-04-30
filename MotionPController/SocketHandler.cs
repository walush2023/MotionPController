using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.Collections.Concurrent;
using System;
using System.Threading;
using System.Linq;
using System.Collections.Generic;

namespace MotionPController
{
    class SocketHandler
    {
        private byte[] machineName = Encoding.UTF8.GetBytes(Environment.MachineName);
        private const int tcpPort = 7063;
        private const int udpPort = 7064;

        private const int MAX_CONNECTIONS = 4;
        private ConcurrentDictionary<IPAddress, Socket> tcpSocket = new ConcurrentDictionary<IPAddress, Socket>();
        private ConcurrentDictionary<IPAddress, Gamepad> ipGamepad = new ConcurrentDictionary<IPAddress, Gamepad>();


        private UdpClient udpSocket;
        private ConcurrentDictionary<IPAddress, Socket> clientSocket = new ConcurrentDictionary<IPAddress, Socket>();


        private bool runThread = true;

        public SocketHandler()
        {
            new Thread(udpThreadCreate).Start();
        }
        public static Byte[] SubArray(Byte[] data, int index, int length)
        {
            Byte[] result = new Byte[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }
        public bool tcpExist(IPAddress ip)
        {
            if (tcpSocket.ContainsKey(ip))
            {
                return true;
            }
            return false;
        }

        public void tcpThreadCreate(IPAddress ip)
        {
            Debug.WriteLine("Socket: server create " + ip);

            try
            {
                // Server start
                tcpSocket.TryAdd(ip, new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp));
                tcpSocket[ip].Bind(new IPEndPoint(ip, tcpPort));
                tcpSocket[ip].Listen(0);
            }
            catch (SocketException e)
            {
                Debug.WriteLine("Socket Error: can not create socket server" + ip);
                tcpSocket.TryRemove(ip, out _);
                return;
            }

            while (runThread && SocketConnected(tcpSocket[ip]))
            {
                try
                {
                    // Server listen
                    Socket client = tcpSocket[ip].Accept();
                    new Thread(() => clientThreadCreate(client)).Start();
                }
                catch (SocketException e)
                {
                    Debug.WriteLine("Socket Error: accept error");
                    break;
                }
                Thread.Sleep(100);
            }

            Debug.WriteLine("Socket close: tcp " + ip);
            tcpSocket.TryRemove(ip, out _);
        }

        public enum Target
        {
            System,
            Mouse,
            Keyboard,
            Gamepad,
        }

        private void clientThreadCreate(Socket client)
        {
            byte[] dateBuffer = new byte[64];
            try
            {
                client.Receive(dateBuffer);

                if (dateBuffer[0] != 0x23)
                {
                    Debug.WriteLine("Socket error: client pair error");
                    return;
                }
            }
            catch (SocketException e)
            {
                Debug.WriteLine("Socket error: clientThreadCreate");
                return;
            }

            IPAddress ip = getRemoteIP(client);
            clientSocket.TryAdd(ip, client);

            if (ipGamepad.ContainsKey(ip))
            {
                Debug.WriteLine("Socket already connected: Client " + ip);
                ipGamepad[ip].close();
                ipGamepad.TryRemove(ip, out _);
            }
            Debug.WriteLine("Socket: Client " + ip);

            ipGamepad.TryAdd(ip, new Gamepad());
            ipGamepad[ip].open();

            if (dateBuffer[1] == 1)
            {
                ipGamepad[ip].FeedbackReceived += (byte largeMotor, byte smallMotor) =>
                {
                    try
                    {
                        // 0 ~ 255
                        if (client != null && SocketConnected(client))
                        {
                            Debug.WriteLine(String.Format("Gamepad {0}: moter {1} {2}", ip.ToString(), largeMotor, smallMotor));
                            client.Send(new byte[] { 0x03, largeMotor, smallMotor });
                        }
                        else
                            Debug.WriteLine(String.Format("Gamepad {0} error: socket connect", ip.ToString()));
                    }
                    catch (SocketException e)
                    {

                    }
                };
            }

            byte[] data = new byte[] { 0x00, (byte)ipGamepad.Count, (byte)machineName.Length }.Concat(machineName).ToArray();
            client.Send(data);

            while (runThread)
            {
                try
                {
                    if (!SocketConnected(client))
                        break;

                    if (client.Receive(dateBuffer) == 0)
                        break;
                }
                catch (SocketException e)
                {
                    break;
                }
                Thread.Sleep(1000);
            }


            Debug.WriteLine("Socket: client {0} disconnect", ip);
            if (ipGamepad.ContainsKey(ip))
            {
                ipGamepad[ip].close();
            }

            clientSocket.TryRemove(ip, out _);
            ipGamepad.TryRemove(ip, out _);
        }

        private void udpThreadCreate()
        {
            Debug.WriteLine("Socket create: udp");
            //Creates a UdpClient for reading incoming data.
            udpSocket = new UdpClient(udpPort);

            //Creates an IPEndPoint to record the IP Address and port number of the sender.
            // The IPEndPoint will allow you to read datagrams sent from any source.
            IPEndPoint remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
            DateTime myDate = DateTime.Now;
            while (runThread)
            {
                try
                {
                    // Blocks until a message returns on this socket from a remote host.
                    Byte[] receiveBytes = udpSocket.Receive(ref remoteIpEndPoint);
                    IPAddress remoteIp = remoteIpEndPoint.Address;

                    if (!clientSocket.ContainsKey(remoteIp))
                    {
                        continue;
                    }

                    Debug.WriteLine(String.Format("Socket receive: {0} {1}",
                        remoteIp.ToString(),
                        string.Join(" ", receiveBytes.Select(x => x.ToString("X2")))));

                    int i = 0;
                    int len = receiveBytes.Length;

                    while (runThread && i < len)
                    {
                        TimeSpan ts = DateTime.Now - myDate;

                        Debug.WriteLine(String.Format("time: {0}", ts.TotalMilliseconds.ToString()));
                        switch ((Target)receiveBytes[i])
                        {
                            case Target.System:
                                switch (receiveBytes[i + 1])
                                {
                                    case 1:
                                        Debug.WriteLine(String.Format("System process start: {0}", receiveBytes[i + 2]));
                                        switch (receiveBytes[i + 2])
                                        {
                                            case 0:
                                                Process.Start("explorer", "https://www.youtube.com/");
                                                break;
                                            case 1:
                                                // Process.Start("netflix://") == null)
                                                Process.Start("explorer", "https://www.netflix.com/tw/");
                                                break;
                                            case 2:
                                                Process.Start("explorer", "https://www.disneyplus.com/");
                                                break;
                                        }
                                        i += 3;
                                        break;
                                    case 2:
                                        String text = System.Text.Encoding.UTF8.GetString(SubArray(receiveBytes, i + 3, receiveBytes[i + 2]));
                                        Debug.WriteLine(String.Format("System text: {0}", text));

                                        System.Windows.Forms.SendKeys.SendWait(text);
                                        i += 3 + receiveBytes[i + 2];
                                        break;
                                }
                                break;
                            case Target.Mouse:
                                if (i + 6 <= len)
                                    Mouse.Trigger(receiveBytes, i + 1);
                                i += 6;
                                break;
                            case Target.Keyboard:
                                if (i + 3 <= len)
                                    Keyboard.Trigger(receiveBytes, i + 1);
                                i += 3;
                                break;
                            case Target.Gamepad:
                                if (i + 6 <= len)
                                    ipGamepad[remoteIp].Trigger(receiveBytes, i + 1);
                                i += 6;
                                break;
                            default:
                                Debug.WriteLine(String.Format("Socket Error: invalid command {0}", receiveBytes[i]));
                                i = len;
                                break;
                        }

                        myDate = DateTime.Now;
                    }
                }
                catch (Exception e)
                {
                }
            }


            Debug.WriteLine("Udp socket thread dispose");
        }

        private static bool SocketConnected(Socket s)
        {
            bool part1 = s.Poll(1000, SelectMode.SelectRead);
            bool part2 = (s.Available == 0);
            if (part1 && part2)
                return false;
            else
                return true;
        }

        static public IPAddress[] getHostIP()
        {
            IPAddress[] host = Dns.GetHostAddresses(Dns.GetHostName());
            List<IPAddress> hostList = new List<IPAddress>();
            foreach (IPAddress ip in host)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    hostList.Add(ip);
                }
            }
            return hostList.ToArray();
        }

        private IPAddress getLocalIP(Socket socket)
        {
            IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
            return endPoint.Address;
        }

        private IPAddress getRemoteIP(Socket socket)
        {
            IPEndPoint endPoint = socket.RemoteEndPoint as IPEndPoint;
            return endPoint.Address;
        }

        public void close()
        {
            runThread = false;

            foreach (Socket socket in tcpSocket.Values)
            {
                socket.Close();
            }
            udpSocket.Close();
            foreach (Socket socket in clientSocket.Values)
            {
                socket.Close();
            }
        }
    }
}
