using System;
using System.Windows.Forms;
using Message = CommonLibrary.Message;
namespace ScreenViewer.Client
{
    public partial class Main : Form
    {
        private SynchronousSocketClient Client = new SynchronousSocketClient();

        public string[] Info { get; private set; }

        public Main()
        {
            InitializeComponent();
            Client.onMainMessage = this.onMessage ;
        }

        private void onMessage(Message obj)
        {

            switch (obj.messageType)
            {
                case 5:
                    this.addInformation(obj.info);
                    break;
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Connection ifrm = new Connection();
            ifrm.Client = Client;
            ifrm.Show(); // отображаем Form5
        }

        private void remoteShellToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form ifrm = new Shell();
            ifrm.Show(); // отображаем Form2
        }

        private void systemInformationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form ifrm = new SystemInformation();
            ifrm.Show(); // отображаем Form3
        }

        private void remoteDesktopToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            Form ifrm = new Desktop();
            ifrm.Show(); // отображаем Form1
        }

        private void DoChangeTicks(ListViewItem item)
        {
            listView1.Items.Add(item);
        }

        public void addInformation(string[] info)
        {
            this.Info = info;
            this.addInfoToList();
        }

        public void addInfoToList()
        {
            var info = this.Info;
            ListViewItem item = new ListViewItem(new string[] { info[0], info[1], info[2], info[3], info[4] });
            if (this.IsHandleCreated)
            {
                this.CreateControl();
            }
            listView1.Invoke((MethodInvoker)delegate {
                // Running on the UI thread
                if (listView1.Items.Count > 0)
                    listView1.Items.RemoveAt(listView1.Items.Count - 1);
                listView1.Items.Add(item);
            });
        }

        public void delFromList()
        {
            listView1.Invoke((MethodInvoker)delegate {
                // Running on the UI thread
                listView1.Items.RemoveAt(listView1.Items.Count - 1);
            });
        }
      
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void taskManagerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SynchronousSocketClient.sendGetListProcess();
        }

        private void showMessageboxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form ifrm = new Messenger();
            ifrm.Show(); // отображаем Form3
        }

        private void fileManagerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var fr = new FileManager();
            fr.Client = Client;
            Client.onFrmMessage = fr.onMessage;
            fr.Show();
            Client.sendGetLocDir();
        }

        private void Main_Load(object sender, EventArgs e)
        {

        }
    }
}
