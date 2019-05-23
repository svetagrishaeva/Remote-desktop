using System;
using System.Windows.Forms;

namespace ScreenViewer.Server
{
    public partial class Form3 : Form
    {
        public Action <string> addAction { get; set; }
        public Form3()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var ip = textBox1.Text;
            addAction?.Invoke(ip);       
            textBox1.Text = "";
            this.Close();
        }
    }
}
