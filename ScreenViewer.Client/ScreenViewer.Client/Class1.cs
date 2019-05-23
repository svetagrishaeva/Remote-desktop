using System;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Message  = CommonLibrary.Message;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Runtime.Serialization;

namespace ScreenViewer.Client
{
    [DataContract]
    public class GeoInformation
    {
        [DataMember(Name = "as")]
        public string As { get; set; }
        [DataMember(Name = "city")]
        public string City { get; set; }
        [DataMember(Name = "country")]
        public string Country { get; set; }
        [DataMember(Name = "countryCode")]
        public string CountryCode { get; set; }
        [DataMember(Name = "isp")]
        public string Isp { get; set; }
        [DataMember(Name = "lat")]
        public double Lat { get; set; }
        [DataMember(Name = "lon")]
        public double Lon { get; set; }
        [DataMember(Name = "org")]
        public string Org { get; set; }
        [DataMember(Name = "query")]
        public string Ip { get; set; }
        [DataMember(Name = "region")]
        public string Region { get; set; }
        [DataMember(Name = "regionName")]
        public string RegionName { get; set; }
        [DataMember(Name = "status")]
        public string Status { get; set; }
        [DataMember(Name = "timezone")]
        public string Timezone { get; set; }
        [DataMember(Name = "zip")]
        public string Zip { get; set; }
    }
    // State object for receiving data from remote device.  
    public class StateObject
    {
        // Client socket.  
        public Socket workSocket = null;
        // Size of receive buffer.  
        public const int BufferSize = 20000000;
        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize];
        // Received data string.  
        public MemoryStream stream = new MemoryStream();
        public SynchronousSocketClient client;
    }

    public class SynchronousSocketClient
    {
        // ManualResetEvent instances signal completion.  
        private static ManualResetEvent connectDone =
            new ManualResetEvent(false);
        private static ManualResetEvent sendDone =
            new ManualResetEvent(false);
        private static ManualResetEvent receiveDone =
            new ManualResetEvent(false);
        public Action<Message> onMessage;
        private Action<Message> _onFrmMessage;
     
        public static byte[] response; // The response from the remote device.
        public static bool connect = false;
        public static Message mes;
        public static Socket sender; 
        public static IPHostEntry ipHostInfo;
        public static IPAddress ipAddress;
        public static IPEndPoint remoteEP;
        static Mutex mutexObj = new Mutex();
        public static bool show = false;

        public Action<Message> onFrmMessage
        {
            get { return _onFrmMessage;  }
            set {
                this._onFrmMessage = value; }
        }
        public Action<Message> onMainMessage;
        public SynchronousSocketClient()
        {

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

        public static Point GetCursorPosition()
        {
            POINT lpPoint;
            GetCursorPos(out lpPoint);
            return lpPoint;
        }

        public void StartClient(string ip, string port)
        {
            // Connect to a remote device.  
            try
            {
                ipAddress = IPAddress.Parse(ip);
                remoteEP = new IPEndPoint(ipAddress, int.Parse(port));
                // Create a TCP/IP  socket.  
                sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                StateObject state = new StateObject();
                state.client = this;
                state.workSocket = sender;
                sender.BeginConnect(remoteEP, ConnectCallback, state);
                connectDone.WaitOne();
                connect = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void ConnectCallback(IAsyncResult ar)
        {
            StateObject state = (StateObject)ar.AsyncState;
            // Retrieve the socket from the state object.  
            Socket client = state.workSocket;

            // Complete the connection.  
            client.EndConnect(ar);

            Console.WriteLine("Socket connected to {0}",
                client.RemoteEndPoint.ToString());

            // Signal that the connection has been made.  
            connectDone.Set();
            try
            {
                Receive(state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());

                if (e.ToString() == "Удаленный хост принудительно разорвал существующее подключение")
                {
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();
                    Console.WriteLine("Удаленный хост принудительно разорвал существующее подключение");
                }
            }
        }

        public static void Receive(StateObject connectionState)
        {
            // Create the state object.  
            StateObject state = new StateObject();
            Socket client = connectionState.workSocket;
            state.workSocket = client;
            state.client = connectionState.client;
            try
            {
                // Begin receiving the data from the remote device.  
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                ReceiveCallback, state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                if (e.ToString() == "Удаленный хост принудительно разорвал существующее подключение")
                {
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();
                    Console.WriteLine("Удаленный хост принудительно разорвал существующее подключение");
                }
            }
        }

        private static void ReceiveCallback(IAsyncResult ar)
        {
            // Retrieve the state object and the client socket   
            // from the asynchronous state object.  
            StateObject state = (StateObject)ar.AsyncState;
            Socket soket = state.workSocket;

            // Read data from the remote device.  
            int bytesRead = soket.EndReceive(ar);

            if (bytesRead > 0)
            {
                // There might be more data, so store the data received so far.  

                state.stream.Write(state.buffer, 0, bytesRead);

                try
                {
                    state.stream.Seek(0, 0); //переместили указатель на начало потока
                    var bin = new BinaryFormatter();

                    var deserialize = bin.Deserialize(state.stream);
                    var mess = (Message)deserialize;
                    state.client.Receive(mess);
                    state.stream = new MemoryStream();
                }
                catch (Exception e)
                {
                    state.stream = new MemoryStream();
                    Console.WriteLine(e);

                }

                soket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), state);

                receiveDone.Set();

            }
            else
            {
                Console.WriteLine($"Receive bytes {state.stream.Length}");
                // All the data has arrived; put it in response.  
                try
                {
                    //Console.WriteLine($"Receive bytes {state.stream.Length}");
                    state.stream.Seek(0, 0); //переместили указатель на начало потока
                    var bin = new BinaryFormatter();

                    var deserialize = bin.Deserialize(state.stream);
                    var mess = (Message)deserialize;
                    state.client.Receive(mess);
                    state.stream = new MemoryStream();
                }
                catch (Exception e)
                {
                    state.stream = new MemoryStream();
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();
                    Console.WriteLine("The remote host forcibly broke the existing connection.");
                    return;
                }

                soket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), state);

                // Signal that all bytes have been received.  
                receiveDone.Set();
            }
        }

        public void Receive(Message message)
        {
            Console.WriteLine($"Receive {message.messageType} {message.index}");

            switch (message.messageType)
            {
                case 0:
                    mes = message;
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    Shell.change_text(message.key + "\n");
                    break;
                case 4:
                    SystemInformation.change_text(message.key + "\n");
                    break;
                case 5:
                    Console.WriteLine("Message 5");
                    break;
                case 6:
                    var test = new TaskManager();
                    test.addInformation(message.proc);

                    break;
                case 15:
                    if (message.key != "Okey.")
                        MessageBox.Show(message.key);
                    else if (message.key == "Okey.")
                        show = true;
                    break;
            }

            onMainMessage?.Invoke(message);
            onMessage?.Invoke(message);
            onFrmMessage?.Invoke(message);
        }

        private static void Send(Socket client, byte[] byteData)
        {
            try
            {
                // Begin sending the data to the remote device.  
                client.BeginSend(byteData, 0, byteData.Length, 0,
                    SendCallback, client);
            }
            catch (Exception e)
            {

                if (e.ToString() == "Удаленный хост принудительно разорвал существующее подключение")
                {
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();
                    Console.WriteLine("Удаленный хост принудительно разорвал существующее подключение");
                }
            }
        }

        private static void SendCallback(IAsyncResult ar)
        {
            // Retrieve the socket from the state object.  
            Socket client = (Socket)ar.AsyncState;

            // Complete sending the data to the remote device.  
            int bytesSent = client.EndSend(ar);
            Console.WriteLine("Sent {0} bytes to server.", bytesSent);

            // Signal that all bytes have been sent.  
            sendDone.Set();
        }

        private static void TryLocate()
        {
            try
            {
                DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(GeoInformation));

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://ip-api.com/json/");
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.3; rv:48.0) Gecko/20100101 Firefox/48.0";
                request.Proxy = null;
                request.Timeout = 10000;
                GeoInformation GeoInfo;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(dataStream))
                        {
                            string responseString = reader.ReadToEnd();

                            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(responseString)))
                            {
                                GeoInfo = (GeoInformation)jsonSerializer.ReadObject(ms);
                            }
                        }
                    }
                }
                //Console.WriteLine("Country " + GeoInfo.Country);
            }
            catch
            {
            }
        }

        public static void sendKillProcess(string cmd)
        {
            BinaryFormatter bin = new BinaryFormatter();
            MemoryStream mem = new MemoryStream();
            byte[] bytes = new Byte[10000000];
            var mes = new Message();
            mes.key = cmd; //!!!чтобы не создавать новый атрибут, запишем id процесса в  key
            mes.screenSize = new Size(0, 0);
            mes.messageType = 7; //тип собщения - убить процесс        
            bin.Serialize(mem, mes);
            bytes = mem.ToArray();
            if (sender != null)
                Send(sender, bytes);
        }

        public static void sendCmd(string cmd)
        {
            BinaryFormatter bin = new BinaryFormatter();
            MemoryStream mem = new MemoryStream();
            byte[] bytes = new Byte[10000000];
            var mes = new Message();
            mes.key = cmd; //!!!чтобы не создавать новый атрибут, запишем команду в  key
            mes.screenSize = new Size(0, 0);
            mes.messageType = 3; //тип собщения         
            bin.Serialize(mem, mes);
            bytes = mem.ToArray();
            if (sender != null)
                Send(sender, bytes);
        }

        public static void sendGetListProcess()
        {
            BinaryFormatter bin = new BinaryFormatter();
            MemoryStream mem = new MemoryStream();
            byte[] bytes = new Byte[10000000];
            var mes = new Message();
            mes.screenSize = new Size(0, 0);
            mes.messageType = 5;       
            bin.Serialize(mem, mes);
            bytes = mem.ToArray();
            if (sender != null)
                Send(sender, bytes);
        }

        public static void sendGetInfo()
        {
            BinaryFormatter bin = new BinaryFormatter();
            MemoryStream mem = new MemoryStream();
            byte[] bytes = new Byte[10000000];
            var mes = new Message();
            mes.screenSize = new Size(0, 0);
            mes.messageType = 4; //тип собщения         
            bin.Serialize(mem, mes);
            bytes = mem.ToArray();
            if (sender != null)
                Send(sender, bytes);
        }

        public static void sendPressKey(string key)
        {
            BinaryFormatter bin = new BinaryFormatter();
            MemoryStream mem = new MemoryStream();
            byte[] bytes = new Byte[10000000];
            var mes = new Message();
            mes.key = key;
            mes.screenSize = new Size(0, 0);
            mes.messageType = 1; //тип собщения - команада нажатия клавиши          
            bin.Serialize(mem, mes);
            bytes = mem.ToArray();
            if (sender != null)
                Send(sender, bytes);
        }
        
        public static void requestRermission()
        {
            BinaryFormatter bin = new BinaryFormatter();
            MemoryStream mem = new MemoryStream();
            byte[] bytes = new Byte[10000000];
            var mes = new Message();
            mes.screenSize = new Size(0, 0);
            mes.messageType = 18;           
            bin.Serialize(mem, mes);
            bytes = mem.ToArray();
            Send(sender, bytes);
        }

        internal static void sendPosCursur(Point p, Size size)
        {
            BinaryFormatter bin = new BinaryFormatter();
            MemoryStream mem = new MemoryStream();
            //p = GetCursorPosition();
            var mes = new Message();
            mes.messageType = 1;
            mes.cursor = p; //положение курсора
            mes.screenSize = size;
            bin.Serialize(mem, mes);
            byte[] bytes = new Byte[10000000];
            bytes = mem.ToArray();
            if (sender != null)
                Send(sender, bytes);
        }

        public static void sendExecute(Point p, Size size)
        {
            BinaryFormatter bin = new BinaryFormatter();
            MemoryStream mem = new MemoryStream();
            byte[] bytes = new Byte[10000000];
            var mes = new Message();
            mes.cursor = p; //положение курсора
            mes.screenSize = size;
            mes.messageType = 2; //тип собщения - команада выполнения файла          
            bin.Serialize(mem, mes);
            bytes = mem.ToArray();
            Send(sender, bytes);
        }

        public static void sendExecute2(Point p, Size size)
        {
            BinaryFormatter bin = new BinaryFormatter();
            MemoryStream mem = new MemoryStream();
            byte[] bytes = new Byte[10000000];
            var mes = new Message();
            mes.cursor = p; //положение курсора
            mes.screenSize = size;
            mes.messageType = 12; //тип собщения - нажатие левой кнопки мыши          
            bin.Serialize(mem, mes);
            bytes = mem.ToArray();
            Send(sender, bytes);
        }

        public static void sendMessage(string message)
        {
            BinaryFormatter bin = new BinaryFormatter();
            MemoryStream mem = new MemoryStream();
            byte[] bytes = new Byte[10000000];
            var mes = new Message();
            mes.screenSize = new Size(0,0);
            mes.messageType = 8;
            mes.key = message;
            bin.Serialize(mem, mes);
            bytes = mem.ToArray();
            Send(sender, bytes);
        }

        public void sendGetLocDir()
        {
            BinaryFormatter bin = new BinaryFormatter();
            MemoryStream mem = new MemoryStream();
            byte[] bytes = new Byte[10000000];
            var mes = new Message();
            mes.screenSize = new Size(0, 0);
            mes.messageType = 9;
            bin.Serialize(mem, mes);
            bytes = mem.ToArray();
            Send(sender, bytes);
        }

        public static void sendGetSubdirAndFiles(string fpath)
        {
            BinaryFormatter bin = new BinaryFormatter();
            MemoryStream mem = new MemoryStream();
            byte[] bytes = new Byte[10000000];
            var mes = new Message();
            mes.screenSize = new Size(0, 0);
            mes.messageType = 10;
            mes.key = fpath; //опять путь до файла запишем суда, чтобы не создавать новый атрибут
            bin.Serialize(mem, mes);
            bytes = mem.ToArray();
            Send(sender, bytes);
        }

        public static void sendGetCopyFile(string copy)
        {
            BinaryFormatter bin = new BinaryFormatter();
            MemoryStream mem = new MemoryStream();
            byte[] bytes = new Byte[10000000];
            var mes = new Message();
            mes.screenSize = new Size(0, 0);
            mes.messageType = 13;
            mes.key = copy; //опять путь до файла запишем суда, чтобы не создавать новый атрибут
            bin.Serialize(mem, mes);
            bytes = mem.ToArray();
            Send(sender, bytes);
        }

        public static void sendDeleteFile(string delete, int i)
        {
            BinaryFormatter bin = new BinaryFormatter();
            MemoryStream mem = new MemoryStream();
            byte[] bytes = new Byte[10000000];
            var mes = new Message();
            mes.screenSize = new Size(0, 0);
            mes.messageType = 15;
            mes.key = i + delete; //опять путь до файла запишем суда, чтобы не создавать новый атрибут
            bin.Serialize(mem, mes);
            bytes = mem.ToArray();
            Send(sender, bytes);
        }

        public static void sendFile(string file, byte[] b, string dir)
        {
            BinaryFormatter bin = new BinaryFormatter();
            MemoryStream mem = new MemoryStream();
            byte[] bytes = new Byte[10000000];
            var mes = new Message();
            mes.screenSize = new Size(0, 0);
            mes.messageType = 14;
            if (dir[dir.Length - 1] != '\\')
                mes.key = dir + "\\" + file; //опять путь до файла запишем суда, чтобы не создавать новый атрибут
            else
                mes.key = dir + file;
            mes.bytes = b;
            bin.Serialize(mem, mes);
            bytes = mem.ToArray();
            Send(sender, bytes);
        }

        public static void sendCreateDir(string path)
        {
            BinaryFormatter bin = new BinaryFormatter();
            MemoryStream mem = new MemoryStream();
            byte[] bytes = new Byte[10000000];
            var mes = new Message();
            mes.screenSize = new Size(0, 0);
            mes.messageType = 16;
            mes.key = path;
            bin.Serialize(mem, mes);
            bytes = mem.ToArray();
            Send(sender, bytes);
        }

        public static void sendCreateFile(string path)
        {
            BinaryFormatter bin = new BinaryFormatter();
            MemoryStream mem = new MemoryStream();
            byte[] bytes = new Byte[10000000];
            var mes = new Message();
            mes.screenSize = new Size(0, 0);
            mes.messageType = 17;
            mes.key = path;
            bin.Serialize(mem, mes);
            bytes = mem.ToArray();
            Send(sender, bytes);
        }

        [STAThreadAttribute]
        public static int Main(String[] args)
        {
            Main form4 = new Main();
            Application.EnableVisualStyles();
            var hWnd = form4.Handle; 
            Application.Run(form4);
            return 0;
        }      
    }
}
