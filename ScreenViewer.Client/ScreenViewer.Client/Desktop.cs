using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.ServiceModel;
using System.Runtime.InteropServices;
using Message = CommonLibrary.Message;
using System.Runtime.Serialization.Formatters.Binary;
//using System.Threading;
using System.Diagnostics;

namespace ScreenViewer.Client {
    public partial class Desktop : Form
    {     
        Timer t = null;
        public static Message mes;
        public Desktop() {
            InitializeComponent();
            //событие нажатия клавиши
            this.KeyUp += new KeyEventHandler(OKP);
        }
        private bool qwerty(string c)
        {
            if (c == "Q" || c == "W" || c == "E" || c == "R" || c == "T" || c == "Y"
                || c == "U" || c == "I" || c == "O" || c == "P" || c == "A" || c == "S"
                || c == "D" || c == "F" || c == "G" || c == "H" || c == "J" || c == "K"
                || c == "L" || c == "Z" || c == "X" || c == "C" || c == "V" || c == "B"
                || c == "N" || c == "M")
                return true;
            return false;
        }
        private string keyRecognition(string c)
        {
            switch (c)
            {
                case "Return":
                    return "{ENTER}";
                case "Home":
                    return "{HOME}";
                case "Insert":
                    return "{INS}";
                case "End":
                    return "{END}";
                case "Space":
                    return " ";
                case "Up":
                    return "{UP}";
                case "Down":
                    return "{DOWN}";
                case "Right":
                    return "{RIGHT}";
                case "Left":
                    return "{LEFT}";
                case "F1":
                    return "{F1}";
                case "F2":
                    return "{F2}";
                case "F3":
                    return "{F3}";
                case "F4":
                    return "{F4}";
                case "F5":
                    return "{F5}";
                case "F6":
                    return "{F6}";
                case "F7":
                    return "{F7}";
                case "F8":
                    return "{F8}";
                case "F9":
                    return "{F9}";
                case "F10":
                    return "{F10}";
                case "F11":
                    return "{F11}";
                case "F12":
                    return "{F12}";
                case "Tab":
                    return "{TAB}";
                case "Capital":
                    return "{CAPSLOCK}";
                case "Back":
                    return "{BS}";
                case "ShiftKey":
                    return "+";
                case "ControlKey": //CTRL
                    return "^";
                case "Menu": //ALT
                    return "%";
                case "Escape":
                    return "{ESC}";
                case "D1":
                    return "1";
                case "D2":
                    return "2";
                case "D3":
                    return "3";
                case "D4":
                    return "4";
                case "D5":
                    return "5";
                case "D6":
                    return "6";
                case "D7":
                    return "7";
                case "D8":
                    return "8";
                case "D9":
                    return "9";
                case "D0":
                    return "0";
            }
            return "";
        }
        private void OKP(object sender, KeyEventArgs e)
        {
            string c;
            Console.WriteLine("Pressed  " + e.KeyCode.ToString());
            if (qwerty(e.KeyCode.ToString()))
            {
                SynchronousSocketClient.sendPressKey(e.KeyCode.ToString());
            }
            else
            {
                c = keyRecognition(e.KeyCode.ToString());
                if (c != "")
                    SynchronousSocketClient.sendPressKey(c);
            }
        }

        /// <summary>
        /// Struct representing a point.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public static implicit operator Point(POINT point)
            {
                return new Point(point.X, point.Y);
            }
        }

        /// <summary>
        /// Retrieves the cursor position, in screen coordinates.
        /// </summary>
        /// <see>See MSDN documentation for further information.</see>
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);
        public static Point p;
        public static Point GetCursorPosition()
        {
            POINT lpPoint;
            GetCursorPos(out lpPoint);
            return lpPoint;
        }

        public static BinaryFormatter bin = new BinaryFormatter();
        public static MemoryStream mem = new MemoryStream();
      
        void t_Tick(object sender, EventArgs e) {
            if (!SynchronousSocketClient.show) return;
                if (SynchronousSocketClient.mes.messageType == 0  && SynchronousSocketClient.mes.bytes!=null) //если скрин экрана
            { //показ изображения
                pictureBox2.Image = Image.FromStream(new MemoryStream(SynchronousSocketClient.mes.bytes));
                pictureBox2.Refresh();
                SynchronousSocketClient.mes.messageType = -1;
            }                      
        }

        void ProcessScreening(OperationType operation) {
        
            switch (operation) {
                case OperationType.START:
                    btnOperate.Text = "STOP";
                    if (t != null && t.Enabled) {
                        t.Stop();
                        t.Dispose();
                        //proxy = null;
                    }
                    StartScreen();
                    break;
                case OperationType.STOP:
                    btnOperate.Text = "START";
                    if (t != null && t.Enabled ) {
                        t.Stop();
                        t.Dispose();
                    }
                    pictureBox2.Image = null;
                    break;
            }
        }
        void StartScreen()
        {
            t = new Timer();
            t.Interval = 5;
            t.Tick += new EventHandler(t_Tick);
            t.Start();
        }
        
        private void btnOperate_Click_1(object sender, EventArgs e)
        {
            SynchronousSocketClient.requestRermission();
            OperationType operation = (OperationType)Enum.Parse(typeof(OperationType), (sender as Button).Text);
            ProcessScreening(operation);
        }
        
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
        private static bool double_click = false;

        //метод вызваеся при движении мышкой
        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            //Control control = (Control)sender; //обьект здесь - это pictureBox, вроде
            //Point p = control.PointToScreen(new Point(e.X+40, e.Y-100)); //преоразуем к экранным координатам, координаты курсора относительно pictureBox
            //SynchronousSocketClient.sendPosCursur(e.Location, pictureBox2.Size); //почему нет доступа из-за уровня защиты ??
            //Thread.Sleep(10);
        }

        private void pictureBox2_Click(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (double_click) {                
                    //то нужно запустить программу
                    SynchronousSocketClient.sendExecute(e.Location, pictureBox2.Size);
                }
                else double_click = true;

            }
            if (e.Button == MouseButtons.Right)
                SynchronousSocketClient.sendExecute2(e.Location, pictureBox2.Size);
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }


        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
    [ServiceContract(SessionMode = SessionMode.Required)]
    public interface IScreen {
        [OperationContract]
        byte[] GetScreenShot();
    }
    enum OperationType {
        START,
        STOP
    }
}
