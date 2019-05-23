using System;
using System.Windows.Forms;
using System.Threading;

namespace ScreenViewer.Client
{
    public partial class TaskManager : Form
    {
        public TaskManager()
        {
            InitializeComponent();
        }

        private void DoChange(ListViewItem item)
        {
            listView1.Items.Add(item);
        }
        private void run(object inf)
        {
            this.Show();
            ListViewItem item;
            var temp = (string[,])inf;
            for (int i = 0; i < temp.Length / 2; i++)
            {
                item = new ListViewItem(new string[] { temp[i, 0], temp[i, 1] });
                DoChange(item);
            }
            Application.Run(this);
        }

        public void addInformation(string[,] info)
        {
            Thread myThread = new Thread(new ParameterizedThreadStart(run));
            myThread.Start(info);
        }

        private void killProcessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var id = listView1.SelectedItems[0].Text;
            SynchronousSocketClient.sendKillProcess(id);
        }
    }
}
