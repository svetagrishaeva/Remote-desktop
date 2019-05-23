using System;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;
using System.Collections.Generic;

namespace ScreenViewer.Server
{
    public partial class Form2 : Form
    {
        Dictionary<string, ActionPermission> Permissions = new Dictionary<string, ActionPermission>();
        public Form2()
        {
            InitializeComponent();
            User[] user = ReadXml();
            ListViewItem item;

            if (user != null)
            {
                for (int i = 0; i < user.Length; i++)
                {
                    var ip = user[i].ip;
                    item = new ListViewItem(new string[] { Convert.ToString(i), ip });
                    var permissions = new ActionPermission();
                    permissions.value1 = user[i].rights[0];
                    permissions.value2 = user[i].rights[1];
                    permissions.value3 = user[i].rights[2];
                    permissions.value4 = user[i].rights[3];
                    permissions.value5 = user[i].rights[4];
                    permissions.value6 = user[i].rights[5];
                    permissions.value7 = user[i].rights[6];
                    permissions.value8 = user[i].rights[7];
                    Permissions.Add(ip, permissions);
                    item.Tag = permissions;
                    listView1.Items.Add(item);
                }
            }
        }

        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView1.SelectedItems)
            {
                listView1.Items.Remove(item);
            }
            int i = 0;
            foreach (ListViewItem item in listView1.Items)
            {
                listView1.Items[i].SubItems[0].Text = Convert.ToString(i);
                i++;
            }
        }
        public class ActionPermission
        {
            public bool value1;
            public bool value2;
            public bool value3;
            public bool value4;
            public bool value5;
            public bool value6;
            public bool value7;
            public bool value8;
        }

        public void addip(string ip)
        {
            var id = listView1.Items.Count;
            ListViewItem item = new ListViewItem(new string[] { Convert.ToString(id), ip });
            var permisions = new ActionPermission();
            item.Tag = permisions;
            listView1.Items.Add(item);
            this.Permissions.Add(ip, permisions);
            showPermissions(permisions);
        }

        public void showPermissions(ActionPermission p)
        {
            checkBox1.Checked = p.value1;
            checkBox2.Checked = p.value2;
            checkBox3.Checked = p.value3;
            checkBox4.Checked = p.value4;
            checkBox5.Checked = p.value5;
            checkBox6.Checked = p.value6;
            checkBox7.Checked = p.value7;
            checkBox8.Checked = p.value8;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form3 ifrm = new Form3();
            ifrm.addAction = addip;
            ifrm.Show();
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            checkBox1.Visible = true;
            checkBox2.Visible = true;
            checkBox3.Visible = true;
            checkBox4.Visible = true;
            checkBox5.Visible = true;
            checkBox6.Visible = true;
            checkBox7.Visible = true;
            checkBox8.Visible = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SynchronousSocketListener.user = new User[listView1.Items.Count];           
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                var item = listView1.Items[i];
                var ip = item.SubItems[1].Text;
                SynchronousSocketListener.user[i].ip = ip;
                SynchronousSocketListener.user[i].rights = new bool[8];
                var permissions = item.Tag as ActionPermission;
                SynchronousSocketListener.user[i].rights[0] = permissions.value1;
                SynchronousSocketListener.user[i].rights[1] = permissions.value2;
                SynchronousSocketListener.user[i].rights[2] = permissions.value3;
                SynchronousSocketListener.user[i].rights[3] = permissions.value4;
                SynchronousSocketListener.user[i].rights[4] = permissions.value5;
                SynchronousSocketListener.user[i].rights[5] = permissions.value6;
                SynchronousSocketListener.user[i].rights[6] = permissions.value7;
                SynchronousSocketListener.user[i].rights[7] = permissions.value8;
            }
            WriteXml(SynchronousSocketListener.user);
            this.Close();
        }

        public static  String XMLFileName = Environment.CurrentDirectory + "\\settings.xml";
        //Запись настроек в файл
        public void WriteXml(User[] users)
        {
            XmlSerializer ser = new XmlSerializer(typeof(User[]));
            TextWriter writer = new StreamWriter(XMLFileName);
            ser.Serialize(writer, users);
            writer.Close();
        }

        //Чтение насроек из файла
        public static User[] ReadXml()
        {
            User[] users = null;
            if (File.Exists(XMLFileName))
            {
                XmlSerializer ser = new XmlSerializer(typeof(User[]));
                TextReader reader = new StreamReader(XMLFileName);
                users = ser.Deserialize(reader) as User[];
                reader.Close();
            }
            else
            {
                //если файла не существует
            }
            return users;
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                var selectedItem = listView1.SelectedItems[0];
                var permissions = selectedItem.Tag as ActionPermission;
                showPermissions(permissions);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                var selectedItem = listView1.SelectedItems[0];
                var permissions = selectedItem.Tag as ActionPermission;
                permissions.value1 = checkBox1.Checked;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                var selectedItem = listView1.SelectedItems[0];
                var permissions = selectedItem.Tag as ActionPermission;
                permissions.value2 = checkBox2.Checked;
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                var selectedItem = listView1.SelectedItems[0];
                var permissions = selectedItem.Tag as ActionPermission;
                permissions.value3 = checkBox3.Checked;
            }
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                var selectedItem = listView1.SelectedItems[0];
                var permissions = selectedItem.Tag as ActionPermission;
                permissions.value4 = checkBox4.Checked;
            }
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                var selectedItem = listView1.SelectedItems[0];
                var permissions = selectedItem.Tag as ActionPermission;
                permissions.value5 = checkBox5.Checked;
            }
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                var selectedItem = listView1.SelectedItems[0];
                var permissions = selectedItem.Tag as ActionPermission;
                permissions.value6 = checkBox6.Checked;
            }
        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                var selectedItem = listView1.SelectedItems[0];
                var permissions = selectedItem.Tag as ActionPermission;
                permissions.value7 = checkBox7.Checked;
            }
        }

        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                var selectedItem = listView1.SelectedItems[0];
                var permissions = selectedItem.Tag as ActionPermission;
                permissions.value8 = checkBox8.Checked;
            }
        }
    }
}
