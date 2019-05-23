using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Message = CommonLibrary.Message;

namespace ScreenViewer.Client
{
    public partial class Connection : Form
    {
        public SynchronousSocketClient Client; 
        public Connection()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string ip = textBox1.Text;
            string port = textBox2.Text;
            Client.StartClient(ip, port);
            if (SynchronousSocketClient.connect) label3.Text = "Статус: cоединение установлено!";
        }
    }
}
