using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ScreenViewer.Client
{
    public partial class SystemInformation : Form
    {
        public SystemInformation()
        {
            InitializeComponent();
            SynchronousSocketClient.sendGetInfo();
        }

        public static void change_text(string message)
        {
            richTextBox1.Invoke(new MethodInvoker(() => {
                richTextBox1.Text += message + "\n";
            }), null);
        }
    }
}
