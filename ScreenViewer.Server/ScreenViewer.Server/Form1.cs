using System;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace ScreenViewer.Server
{
    public partial class Form1 : Form
    {
        public string port;
        public static string IP;
        public Form1()
        {
            InitializeComponent();
            port = textBox1.Text;
            // Получение имени компьютера.
            String host = System.Net.Dns.GetHostName();
            // Получение ip-адреса.
            int n = System.Net.Dns.GetHostByName(host).AddressList.Length; //количество локальных адресов

            for (int i = 0; i < n; i++)
                Form1.addIp(System.Net.Dns.GetHostByName(host).AddressList[i]);
        }
        public const string start = "Начать слушать";
        public const string stop = "Остановить прослушивание";
        public int flag=0;
        public void button1_Click(object sender, EventArgs e)
        {
            switch (button1.Text)
            {
                case start:
                    button1.Text = "Остановить прослушивание";
                    if (listBox1.SelectedItem == null)
                    {
                        MessageBox.Show("Выберите ip-адрес.");
                        return;
                    }
                    IP = listBox1.SelectedItem.ToString();
                    IPAddress ipAddress = (IPAddress)listBox1.SelectedItem;
                    IPEndPoint localEndPoint = new IPEndPoint(ipAddress, int.Parse(port));

                    SynchronousSocketListener.port = port;
                    SynchronousSocketListener.localEndPoint = localEndPoint;
                    SynchronousSocketListener.ipAddress = ipAddress;
                    if (flag == 0)
                    {
                        SynchronousSocketListener.Start(port, localEndPoint, ipAddress);
                        SynchronousSocketListener.Listening(port, localEndPoint, ipAddress);
                    }
                    else
                        SynchronousSocketListener.Listening(port, localEndPoint, ipAddress);
                    flag = 1;
                    break;
                case stop:
                    button1.Text = "Начать слушать";
                    Console.WriteLine("It is stopped listening.");
                    SynchronousSocketListener.handler.Shutdown(SocketShutdown.Both);
                    SynchronousSocketListener.handler.Close();
                    break;
            }
        }
        public static void addIp(IPAddress adress)
        {
            listBox1.Items.Add(adress);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form ifrm = new Form2();
            ifrm.Show();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form ifrm = new Form2();
            ifrm.Show();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (SynchronousSocketListener.handler != null)
            {
                SynchronousSocketListener.handler.Shutdown(SocketShutdown.Both);
                SynchronousSocketListener.handler.Close();
            }
        }
    }
}
