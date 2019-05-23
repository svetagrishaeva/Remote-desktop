using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace ScreenViewer.Client
{
    public partial class Shell : Form
    {
        public Shell()
        {
            InitializeComponent();
            this.KeyUp += new KeyEventHandler(pressEnter);
        }

        void pressEnter(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.ToString() == "Return")
                new Thread(DoWork).Start();
        }

        public static void change_text(string message)
        {
            richTextBox1.Invoke(new MethodInvoker(() => {
                richTextBox1.Text += message + "\n";
            }), null);
        }
        void DoWork()
        {           
            if (textBox1.Text == String.Empty)
            {
                MessageBox.Show("Введите команду!");
                return;
            }
            Thread.Sleep(500);
            SynchronousSocketClient.sendCmd(textBox1.Text);          
        }

        private void button1_Click(object sender, EventArgs e)
        {
            new Thread(DoWork).Start();
        }
    }
}
