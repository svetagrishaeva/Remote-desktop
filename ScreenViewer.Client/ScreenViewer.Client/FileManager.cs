using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Microsoft.VisualBasic;
using System.Threading;
using Message = CommonLibrary.Message;

namespace ScreenViewer.Client
{
    public partial class FileManager : Form
    {
        public FileManager()
        {
            InitializeComponent();          
        }
        string FPath;
        string backPath;
        public static List<string> ls = new List<string>();

        public SynchronousSocketClient Client { get; internal set; }

        public void onMessage(Message message)
        {
            switch(message.messageType)
            {
                case 9:
                    addLocDir(message.info);
                    break;
                case 10:
                    listView1.Invoke((MethodInvoker)delegate
                    {
                        this.listView1.Items.Clear();
                    });
                    addSubdir(message.info);
                    addFiles(message.files);
                    Thread.Sleep(200);
                    break;
                case 13:
                    uploadFile(message.bytes);
                    break;
            }
        }

        private void uploadFile(byte[] bytes)
        {
            listView1.Invoke((MethodInvoker)delegate
            {
                string p;
                if (path == string.Empty) return;
                ListViewItem item = listView1.SelectedItems[0];
                if (path != string.Empty && path[path.Length - 1] != '\\')
                    path += "\\";
                p = path + item.Text.ToString();
                File.WriteAllBytes(p, bytes);
            });
    
        }

        private void CopyDir(DirectoryInfo soursDir, DirectoryInfo destDir)
        {
            while (true)
            {
                CreateDir(soursDir, destDir);

                //теперь проверяем наличие в ней папок
                DirectoryInfo[] dirs = soursDir.GetDirectories();
                if (dirs.Length > 0)
                {
                    foreach (DirectoryInfo di in dirs)
                    {
                        DirectoryInfo dir = new DirectoryInfo(destDir.FullName.ToString() + "\\" + di.Name.ToString());
                        CopyDir(di, dir);
                    }
                    break;
                }
                else break;
            }
        }

        //создаем папку
        private void CreateDir(DirectoryInfo soursDir, DirectoryInfo destDir)
        {
            if (!destDir.Exists) destDir.Create();

            //проверяем наличие файлов
            FileInfo[] fls = soursDir.GetFiles();
            if (fls.Length > 0) //копируем если есть
                foreach (FileInfo fi in fls)
                    fi.CopyTo(destDir.FullName.ToString() + "\\" + fi.Name.ToString(), true);
        }

        public void addLocDir(string[] LogicalDrives)
        {

            listView1.Invoke((MethodInvoker)delegate
            {
                this.listView1.Items.Clear();
                this.Text = "Мой компьютер";
                foreach (string s in LogicalDrives)
                {
                    listView1.Items.Add(s, 1);
                    ls.Add(s);
                }
            });
        }

        public void GetLocDir()
        {
            var forms = Application.OpenForms[0];
            Console.WriteLine(forms);
            string[] LogicalDrives = Environment.GetLogicalDrives();
            this.listView1.Items.Clear();
            this.Text = "Мой компьютер";
            foreach (string s in LogicalDrives)
            {
                listView1.Items.Add(s, 1);
                ls.Add(s);
            }
            Application.Run(this);
        }

        public void addSubdir(object dirs)
        {
            listView1.Invoke((MethodInvoker)delegate {


                this.listView1.Items.Clear();
                var temp = (string[])dirs;
                foreach (string s in temp)
                {
                    string dirname = System.IO.Path.GetFileName(s);
                    this.listView1.Items.Add(dirname, 1);
                    ls.Add(s);
                }
                this.button1.Refresh();
                this.button2.Refresh();
                this.listView1.Refresh();


            });
        }

        public void addFiles(object files)
        {
            listView1.Invoke((MethodInvoker) delegate
            {
                var temp = (string[]) files;
                foreach (string s in temp)
                {
                    string filename = System.IO.Path.GetFileName(s);
                    this.listView1.Items.Add(filename, 0);
                }

                this.button1.Refresh();
                this.button2.Refresh();
                this.listView1.Refresh();
            });
        }

        void GetFiles()
        {
            this.listView1.BeginUpdate();

            try
            {
                SynchronousSocketClient.sendGetSubdirAndFiles(FPath);
                Thread.Sleep(100);            
                ls.Clear();
                backPath = FPath;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            this.listView1.EndUpdate();
        }

        private void listView1_ItemActivate(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
                return;

            ListViewItem item = listView1.SelectedItems[0];
            if (item.ImageIndex == 1)
            {
                string it = item.Text;
                string title = "";
                foreach (string s in ls)
                {
                    try
                    {
                        if (s.Substring(s.Length - it.Length, it.Length) == it)
                        {
                            FPath = s;
                            title = s;
                        }
                    }
                    catch { }
                }
                try
                {
                    this.Text = title;
                    GetFiles();
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }

            else if (item.ImageIndex == 0)
            {
                string start = this.Text + "\\" + item.Text;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            GetLocDir();
        }

        public string path; //путь до каталога в который выгружаются файлы
        private void копироватьToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (listView1.SelectedItems[0] == null)
            {
                MessageBox.Show("Выберите файл или папку.");
                return;
            }

            path = Microsoft.VisualBasic.Interaction.InputBox("Введите в поле путь для копирования",
                "Укажите путь", "D:\\", 10, 10);
            
            ListViewItem item = listView1.SelectedItems[0];
            string copy = this.Text + "\\" + item.Text;
            try
            {
                if (item.ImageIndex == 1) //пока что копирует только файлы
                {
                    /*DirectoryInfo soursDir = new DirectoryInfo(copy); //папка из которой копировать
                    DirectoryInfo destDir = new DirectoryInfo(path + "new" + item.Text); //куда копируешь
                    System.Threading.Thread MyThread1 =
                    new System.Threading.Thread(delegate () { CopyDir(soursDir, destDir); });
                    MyThread1.Start();*/
                }
                else
                {
                    SynchronousSocketClient.sendGetCopyFile(copy);
                    /*System.Threading.Thread MyThread1 =
                       new System.Threading.Thread(delegate () { File.Copy(copy, path + "new" + item.Text); });
                    MyThread1.Start(); */


                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            GetFiles();
        }

        private void удалитьToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (listView1.SelectedItems[0] == null)
            {
                MessageBox.Show("Выберите файл или папку.");
                return;
            }
            ListViewItem item = listView1.SelectedItems[0];
            string message = "Вы действительно хотите удалить " + item.Text + "?";
            string caption = "Удаление файла";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result;

            try
            {
                result = MessageBox.Show(message, caption, buttons);

                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    string delete = this.Text + "\\" + item.Text;
                    SynchronousSocketClient.sendDeleteFile(delete, item.ImageIndex);
                }

            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            GetFiles();
        }

        private void папкуToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            try
            {
                string name, path;
                if (this.Text == "Мой компьютер")
                {
                    MessageBox.Show("Выберите диск.");
                    return;
                }
                name = Microsoft.VisualBasic.Interaction.InputBox("Введите в поле имя файла",
                  "Введите данные", "", 10, 10);
                if (this.Text[this.Text.Length-1] !='\\')
                    path = this.Text + "\\" + name;
                else
                    path = this.Text + name;
                SynchronousSocketClient.sendCreateDir(path);

            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            GetFiles();
        }

        private void файлToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            try
            {
                string name, path;
                if (this.Text == "Мой компьютер")
                {
                    MessageBox.Show("Выберите диск.");
                    return;
                }
                name = Microsoft.VisualBasic.Interaction.InputBox("Введите в поле путь и название файла",
                   "Введите данные", "", 10, 10);
                if (this.Text[this.Text.Length - 1] != '\\')
                    path = this.Text + "\\" + name;
                else
                    path = this.Text + name;
                SynchronousSocketClient.sendCreateFile(path);
                //File.Create(name);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            GetFiles();
        }

        private void переместитьToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            string path = Microsoft.VisualBasic.Interaction.InputBox("Введите в поле путь для перемещения",
                "Укажите путь", "D:\\", 10, 10);
            ListViewItem item = listView1.SelectedItems[0];
            string move = this.Text + "\\" + item.Text;
            try
            {
                if (item.ImageIndex == 1)
                {
                    DirectoryInfo soursDir = new DirectoryInfo(move); //папка из которой копировать
                    DirectoryInfo destDir = new DirectoryInfo(path + item.Text); //куда копируешь                    
                    CopyDir(soursDir, destDir);
                    Directory.Delete(move, true);
                }
                else
                {
                    File.Move(move, path + item.Text);

                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            GetFiles();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            bool b = false;
            String[] LogicalDrives = Environment.GetLogicalDrives(); /////
            foreach (string s in LogicalDrives)
            {
                if (backPath == s && FPath == s) b = true;
            } 
            if (backPath != null && !b)
            {
                this.Text = backPath;
                FPath = backPath;
                while(FPath[FPath.Length - 1] != '\\'  )
                {
                    FPath = FPath.Remove(FPath.Length - 1);
                }
                GetFiles();
            }
            else
            {
               Client.sendGetLocDir();
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            GetFiles();
        }

        private void выгрузитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView1.Invoke((MethodInvoker)delegate
            {
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    byte[] bt = File.ReadAllBytes(openFileDialog1.FileName);
                    string path = openFileDialog1.FileName, name = "";
                    while (path[path.Length - 1] != '\\')
                    {
                        name += path[path.Length - 1];
                        path = path.Remove(path.Length - 1);
                    }
                    string temp = name;
                    name = "";
                    for (int i = temp.Length - 1; i >= 0; i--)
                        name  += temp[i];
                    string dir = this.Text;
                    SynchronousSocketClient.sendFile(name, bt, dir);
                }
            });
        }
    }
}
