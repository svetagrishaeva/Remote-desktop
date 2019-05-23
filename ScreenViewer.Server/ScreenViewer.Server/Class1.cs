using System;
using System.Net;
using System.Net.Sockets;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using Message = CommonLibrary.Message;
using System.Diagnostics;
using System.Management;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Runtime.Serialization;

namespace ScreenViewer.Server
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
    // State object for reading client data asynchronously  
    public class StateObject
    {
        // Client  socket.  
        public Socket workSocket = null;
        // Size of receive buffer.  
        public const int BufferSize = 20000000;
        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize];
        // Received data string.  
        public MemoryStream stream = new MemoryStream();
    }

    public struct User
    {
        public string ip;
        public bool[] rights;
    }

    public class SynchronousSocketListener
    {
        public static GeoInformation GeoInfo;
        public static string port;
        public static IPEndPoint localEndPoint;
        public static IPAddress ipAddress;

        public static  User[] user;
        public static User currentUser;
        // ManualResetEvent instances signal completion.  
        private static ManualResetEvent connectDone =
            new ManualResetEvent(false);
        private static ManualResetEvent sendDone =
            new ManualResetEvent(false);
        private static ManualResetEvent receiveDone =
            new ManualResetEvent(false);
        // The response from the remote device.
        public static byte[] response;

        public static BinaryFormatter bin = new BinaryFormatter();
        public static StateObject state;

        // Data buffer for incoming data.  
        public static byte[] bytes = new Byte[20000000];
        // Output data to client.  
        public static byte[] data = new Byte[20000000];
        // Program is suspended while waiting for an incoming connection.  
        public static Socket handler;
        private static Message mess;
        public static string remoteHost;
        // сингал потока.  
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        public static Socket listener;
        public static Point p;

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
       
        public static byte[] GetScreenShot()
        {     
            Bitmap b = GetScreenBitmap();
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(b, typeof(byte[]));
        }

        public static Bitmap GetScreenBitmap()
        {
            Bitmap screenShot = new Bitmap(SystemInformation.VirtualScreen.Width,
                                         SystemInformation.VirtualScreen.Height,
                                         System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Graphics screenGraph = Graphics.FromImage(screenShot);
            screenGraph.CopyFromScreen(SystemInformation.VirtualScreen.X,
                                       SystemInformation.VirtualScreen.Y,
                                       0,
                                       0,
                                       SystemInformation.VirtualScreen.Size,
                                       CopyPixelOperation.SourceCopy);
            var icon = new Icon("cursor.ico");
            screenGraph.DrawIcon(icon, Cursor.Position.X, Cursor.Position.Y);
            return screenShot;
        }

        public static void Start(string port, IPEndPoint localEndPoint, IPAddress ipAddress)
        {
            // Create a TCP/IP socket.  
            listener = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and   
            // listen for incoming connections.  
            try
            {
                listener.Bind(localEndPoint);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static void Listening(string port, IPEndPoint localEndPoint, IPAddress ipAddress)
        {
            try
            {
                listener.Listen(100);
                listener.BeginAccept(
                    new AsyncCallback(AcceptCallback),
                    listener);
                Console.WriteLine("Waiting for a connection..." + localEndPoint);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.  
            allDone.Set();

            // Get the socket that handles the client request.  
            Socket listener = (Socket)ar.AsyncState;
            handler = listener.EndAccept(ar);
            Console.WriteLine("Connection established with " + handler.RemoteEndPoint.ToString());
            remoteHost = ((IPEndPoint)handler.RemoteEndPoint).Address.ToString();

            user = Form2.ReadXml();
            if (user != null)
                foreach (var s in user)
                {
                    if (s.ip == remoteHost)
                    {
                        currentUser.ip = s.ip;
                        currentUser.rights = new bool[8];
                        currentUser.rights[0] = s.rights[0];
                        currentUser.rights[1] = s.rights[1];
                        currentUser.rights[2] = s.rights[2];
                        currentUser.rights[3] = s.rights[3];
                        currentUser.rights[4] = s.rights[4];
                        currentUser.rights[5] = s.rights[5];
                        currentUser.rights[6] = s.rights[6];
                        currentUser.rights[7] = s.rights[7];
                    }
                }

            if (currentUser.ip == null)
            {
                Console.WriteLine("The server refused the connection. The connected user is not in the security list.");
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
                Listening(port, localEndPoint, ipAddress);
                return;
            }
            sendInfo();

            Thread myThread = new Thread(sendScreenShotAsync); //Создаем новый объект потока (Thread)
            myThread.Start();
            // Create the state object.  
            StateObject state = new StateObject();
            state.workSocket = handler;
            Receive(handler);
        }

        private static void Receive(Socket client)
        {
            try
            {
                // Create the state object.
                StateObject state = new StateObject();
                state.workSocket = client;
                // Begin receiving the data from the remote device.
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                if (e.ToString() == "Удаленный хост принудительно разорвал существующее подключение")
                {
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                    Listening(port, localEndPoint, ipAddress);
                    Console.WriteLine("Удаленный хост принудительно разорвал существующее подключение");
                    return;
                }
            }
        }

        private static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client socket 
                // from the asynchronous state object.
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;
                // Read data from the remote device.
                int bytesRead = client.EndReceive(ar);
                if (bytesRead > 0)
                {
                    // There might be more data, so store the data received so far.
                    state.stream.Write(state.buffer, 0, bytesRead);
                    //  state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
                    // Get the rest of the data.
                    try
                    {
                        Console.WriteLine($"Receive bytes {state.stream.Length}");
                        state.stream.Seek(0, 0); //переместили указатель на начало потока
                        var bin = new BinaryFormatter();

                        var deserialize = bin.Deserialize(state.stream);
                        var mess = (Message)deserialize;
                        Receive(mess);
                        state.stream = new MemoryStream();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        if (e.ToString() == "Удаленный хост принудительно разорвал существующее подключение")
                        {
                            handler.Shutdown(SocketShutdown.Both);
                            handler.Close();
                            Listening(port, localEndPoint, ipAddress);
                            Console.WriteLine("Удаленный хост принудительно разорвал существующее подключение");
                            return;
                        }

                    }

                    client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), state);
                }
                else
                {

                    // All the data has arrived; put it in response.
                    if (state.stream.Length > 1)
                    {
                        //    response = Encoding.ASCII.GetBytes(state.sb.ToString());
                    }
                    // Signal that all bytes have been received.
                    receiveDone.Set();

                    //TODO
                    state.stream.Seek(0, 0); //переместили указатель на начало потока
                    var deserialize = bin.Deserialize(state.stream);

                    mess = (Message)deserialize;
                    Receive(mess);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                string s = "Удаленный хост принудительно разорвал существующее подключение";
                if (String.Equals(e.ToString(), s))
                {
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                    Listening(port, localEndPoint, ipAddress);
                    Console.WriteLine("Удаленный хост принудительно разорвал существующее подключение");
                    return;
                }
            }
        }

        private static void Send(Socket client, byte[] byteData)
        {
            try
            {
                // Begin sending the data to the remote device.  
                client.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), client);
            }
            catch (Exception e)
            {
                if (e.ToString() == "Удаленный хост принудительно разорвал существующее подключение")
                {
                    client.Shutdown(SocketShutdown.Both);
                    client.Close();
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
            //Console.WriteLine("Sent {0} bytes to server.", bytesSent);

            // Signal that all bytes have been sent.  
            sendDone.Set();
        }


        public static void sendScreenShotAsync()
        {
            var index = 0;
            while (true)
            {
                sendScreenShot(index++);
                Thread.Sleep(500);
                
            }
        }

        private static void Receive(Message mess)
        {
            Console.WriteLine("Recive Message " + mess.messageType);
            //проверяем какое сообщение пришло от клиента
            /*
                if (mess.messageType == 1) //если положение курсора, то меняем позицию курсора на экране
                {
                    var remotePosition = mess.cursor;
                    var remoteRemoteSize = mess.screenSize;
                    Console.WriteLine("Receive Cursor " + remotePosition.X + " " + remotePosition.Y);
                    //var cursorPostition = new Point(cursor.X-5,cursor.Y-5);

                    var virtualScreenSize = SystemInformation.VirtualScreen.Size;
                    var localX = (remotePosition.X * virtualScreenSize.Width / remoteRemoteSize.Width);
                    var localY = (remotePosition.Y * virtualScreenSize.Height / remoteRemoteSize.Height);
                    Cursor.Position = new Point(localX, localY);
                    mess.messageType = -1;
                }*/
            if (mess.messageType == 1)
            {
                if (currentUser.rights[2])
                {
                    var Key = mess.key;
                    Console.WriteLine("Receive PresKey " + Key);
                    SendKeys.SendWait(Key);
                    mess.messageType = -1;
                }
                else sendMessage("Нет разрешения на запись в файлы.");
            }
            if (mess.messageType == 2) //если команда запуска файла, то делаем двойной щелчок мыши
            {
                if (currentUser.rights[1])
                {
                    var remotePosition = mess.cursor;
                    var remoteRemoteSize = mess.screenSize;
                    Console.WriteLine("Receive Cursor " + remotePosition.X + " " + remotePosition.Y);
                    var virtualScreenSize = SystemInformation.VirtualScreen.Size;
                    var localX = (remotePosition.X * virtualScreenSize.Width / remoteRemoteSize.Width);
                    var localY = (remotePosition.Y * virtualScreenSize.Height / remoteRemoteSize.Height);
                    Cursor.Position = new Point(localX, localY);
                    Point p = GetCursorPosition();
                    Console.WriteLine(p.X + ";" + p.Y);
                    pressMouse(p);
                    mess.messageType = -1;
                }
                else
                    sendMessage("Нет разрешения на запуск файлов.");
            }
            if (mess.messageType == 3)
            {
                if (currentUser.rights[4])
                {
                    Console.WriteLine("Receive cmd " + mess.key);
                    Process proc = new Process()
                    {
                        StartInfo = new ProcessStartInfo("cmd.exe", mess.key)
                        {
                            RedirectStandardOutput = true,
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            WindowStyle = ProcessWindowStyle.Hidden
                        }
                    };

                    proc.Start();

                    if (!proc.StartInfo.RedirectStandardOutput)
                        return;

                    string line = "", output = mess.key + "\n";
                    StreamReader sr = proc.StandardOutput;
                    while (!sr.EndOfStream)
                    {
                        line = sr.ReadLine();
                        output += line + "\n";
                    }
                    Thread myThread = new Thread(new ParameterizedThreadStart(sendOutput)); //Создаем новый объект потока (Thread)
                    myThread.Start(output);
                }
                else
                    sendMessage("Нет разрешения на вызов командной строки.");
            }
            if (mess.messageType == 4) //если - 4, то собираем системную информацию и отправляем её.
            {
                if (currentUser.rights[3])
                {
                    string info = getSystemInfo();
                    info += getNextSystemInfo();
                    Thread myThread = new Thread(new ParameterizedThreadStart(sendInfo)); //Создаем новый объект потока (Thread)
                    myThread.Start(info);
                    mess.messageType = -1;
                }
                else
                    sendMessage("Нет разрешения на получение системной информации.");
            }
            if (mess.messageType == 5) //если - 5, то собираем системную информацию о процессах и отправляем её.
            {
                if (currentUser.rights[6])
                {
                    Thread myThread = new Thread(sendProcess);
                    myThread.Start();
                    mess.messageType = -1;
                }
                else
                    sendMessage("Нет разрешения на получение информации об процессах.");
            }
            if (mess.messageType == 7) //если - 7, то  убиваем процесс
            {
                Process p = Process.GetProcessById(int.Parse(mess.key));
                p.Kill();              
                //оправляем обновленный список процессов
                Thread myThread = new Thread(sendProcess);
                myThread.Start();
                mess.messageType = -1;
            }
            if (mess.messageType == 8) //если - 8, то  показывем окно с присланным сообщением
            {
                if (currentUser.rights[7])
                {
                    if (mess.key != "")
                        MessageBox.Show(mess.key);
                    mess.messageType = -1;
                }
                else
                {
                    Thread.Sleep(10);
                    sendMessage("Нет разрешения отправлять сообщения.");
                }
            }
            if (mess.messageType == 9) //если - 9, то получаем список дисков и отправляем его
            {
                if (currentUser.rights[5])
                {
                    String[] LogicalDrives = Environment.GetLogicalDrives();
                    sendDir(LogicalDrives);
                    mess.messageType = -1;
                }
                else
                    sendMessage("Нет доступа к файловому менеджеру.");
            }
            if (mess.messageType == 10) //если - 10, то получаем список подкаталогов и файлов
            {
                string[] dirs = Directory.GetDirectories(mess.key);
                string[] files = Directory.GetFiles(mess.key);
                sendSubdirAndFiles(dirs, files); 
                mess.messageType = -1;
            }
            /*
            if (mess.messageType == 11) //если - 11, то получаем список файлов текущем каталоге
            {
                string[] files = Directory.GetFiles(mess.key);
                sendFiles(files); //Создаем новый объект потока (Thread)
                mess.messageType = -1;
            }*/
            if (mess.messageType == 12) //если команда запуска файла, то делаем нажатие правой кнопки мыши
            {
                var remotePosition = mess.cursor;
                var remoteRemoteSize = mess.screenSize;
                Console.WriteLine("Receive Cursor " + remotePosition.X + " " + remotePosition.Y);
                var virtualScreenSize = SystemInformation.VirtualScreen.Size;
                var localX = (remotePosition.X * virtualScreenSize.Width / remoteRemoteSize.Width);
                var localY = (remotePosition.Y * virtualScreenSize.Height / remoteRemoteSize.Height);
                Cursor.Position = new Point(localX, localY);
                Point p = GetCursorPosition();
                Console.WriteLine(p.X + ";" + p.Y);
                pressMouseRight(p);
                mess.messageType = -1;
            }
            if (mess.messageType == 13) //если - 13, то читаем байты из файла и отправляем
            {
                byte[] a = File.ReadAllBytes(mess.key);
                sendFile(a);
                mess.messageType = -1;
            }
            if (mess.messageType == 14) //если - 14, то создаем файл
            {
                File.WriteAllBytes(mess.key, mess.bytes);
                mess.messageType = -1;
            }
            if (mess.messageType == 15) //если - 15, то удаляем файл
            {
                string path = mess.key.Remove(0, 1);
                int ImageIndex = mess.key[0] - '0';
                if (ImageIndex == 1)
                    Directory.Delete(path, true); //удалить дирректорию
                else
                    File.Delete(path); //удалить файл
                mess.messageType = -1;
            }
            if (mess.messageType == 16) //если - 16, то создаем каталог
            {
                Directory.CreateDirectory(mess.key);
                mess.messageType = -1;
            }
            if (mess.messageType == 17) //если - 17, то создаем файл
            {
                File.Create(mess.key);
                mess.messageType = -1;
            }
            if (mess.messageType == 18) 
            {
                if (currentUser.rights[0])
                    sendMessage("Okey.");
                else
                    sendMessage("Нет доступа к отображению экрана рабочего стола.");
            }
        }

        public static string getUser()
        {
            ManagementObjectSearcher searcher5 = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_OperatingSystem");
            foreach (ManagementObject queryObj in searcher5.Get())
            {
                return queryObj["CSName"].ToString();
            }
            return "";
        }

        public static string getOS()
        {
            ManagementObjectSearcher searcher5 = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_OperatingSystem");
            foreach (ManagementObject queryObj in searcher5.Get())
            {
                 return queryObj["Caption"].ToString();
            }
            return "";
        }
        public static string getIP()
        {
            return Form1.IP;
        }

        private static string getNextSystemInfo()
        {
            ManagementObjectSearcher searcher1 =
            new ManagementObjectSearcher("root\\CIMV2",
            "SELECT * FROM Win32_Processor");

            string res = "";
            foreach (ManagementObject queryObj in searcher1.Get())
            {
                res += "\nИНФОРМАЦИЯ О ПРОЦЕССОРЕ\n";
                res += "\nНазваниe: \t\t" + queryObj["Name"];
                res += "\nКоличество ядер: \t" + queryObj["NumberOfCores"];
            }

            ManagementObjectSearcher searcher5 = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_OperatingSystem");
            foreach (ManagementObject queryObj in searcher5.Get())
            {
                res += "\n\nИНФОРМАЦИЯ ОБ ОПЕРАЦИОННОЙ СИСТЕМЕ";
                res += "\nИмя узла: \t\t" + queryObj["CSName"];
                res += "\nНазвание ОС: \t\t" + queryObj["Caption"];
                res += "\nАрхитектура ОС: \t" + queryObj["OSArchitecture"];
                res += "\nВерсия ОС: \t\t" + queryObj["Version"];
                res += "\nИзготовитель ОС: \t" + queryObj["Manufacturer"];
                res += "\nЗарег. владелец: \t" + queryObj["RegisteredUser"];
                res += "\nПапка Windows: \t\t" + queryObj["WindowsDirectory"];
                res += "\nСистемный диск: \t" + queryObj["SystemDrive"];
                res += "\nСистемное устройство: \t" + queryObj["SystemDevice"];
                res += "\nЗагруз. устройство: \t" + queryObj["BootDevice"];
                res += "\nНомер сборки: \t\t" + queryObj["BuildNumber"];
                res += "\nТип сборки: \t\t" + queryObj["BuildType"];
                res += "\nСвободная физическая память (байт): " + queryObj["FreePhysicalMemory"];
                res += "\nСвободная виртуальная память (байт): " + queryObj["FreeVirtualMemory"];
                res += "\nОбщий размер виртуал. памяти (байт): " + queryObj["TotalVirtualMemorySize"];
                res += "\nОбщий размер видимой памяти (байт): " + queryObj["TotalVisibleMemorySize"];
            }
            
            ManagementObjectSearcher searcher11 =
    new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_VideoController");

            foreach (ManagementObject queryObj in searcher11.Get())
            {
                res += "\n\nИНФОРМАЦИЯ О ВИДЕОКАРТЕ";
                res += "\nАдаптер RAM: " + queryObj["AdapterRAM"];
                res += "\nНазвание: " + queryObj["Caption"];
                res += "\nОписание: " + queryObj["Description"];
                res += "\nВидеопроцессор: " + queryObj["VideoProcessor"];
            }
            return res;
        }

        public static string getSystemInfo()
        {
            string res = "ОБЩАЯ ИНФОРМАЦИЯ";
            res += "\nОперационная система (номер версии):  " + Environment.OSVersion;
            res += "\nРазрядность процессора:  " + Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");
            res += "\nМодель процессора:  " + Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER");
            res += "\nПуть к системному каталогу:  " + Environment.SystemDirectory;
            res += "\nЧисло процессоров:  " + Environment.ProcessorCount;
            res += "\nИмя пользователя: " + Environment.UserName;

            res += "\nЛокальные диски: ";
            foreach (DriveInfo dI in DriveInfo.GetDrives())
            {
                if (!dI.IsReady) continue;
                res += "\n\tДиск: " + dI.Name + "\n\t" +
                      "Формат диска: " + dI.DriveFormat + "\n\t" +
                      "Размер диска (ГБ): " + (double)dI.TotalSize / 1024 / 1024 / 1024 + "\n\t" +
                      "Доступное свободное место (ГБ): " + (double)dI.AvailableFreeSpace / 1024 / 1024 / 1024 + "\n";
            }
            return res;
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

        [DllImport("user32.dll", SetLastError = true)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, int dwExtraInfo);

        public static void pressMouse(Point p) //имитация двойного нажания мыши на экране в указанной точке
        {
            mouse_event((uint)MouseEventFlags.LEFTDOWN | (uint)MouseEventFlags.LEFTUP, (uint)p.X, (uint)p.Y, 0, 0);
            //Thread.Sleep(150);
            mouse_event((uint)MouseEventFlags.LEFTDOWN | (uint)MouseEventFlags.LEFTUP, (uint)p.X, (uint)p.Y, 0, 0);
        }
        public static void pressMouseRight(Point p) //имитация двойного нажания мыши на экране в указанной точке
        {
            mouse_event((uint)MouseEventFlags.RIGHTDOWN | (uint)MouseEventFlags.RIGHTUP, (uint)p.X, (uint)p.Y, 0, 0);
        }
        [Flags]
        public enum MouseEventFlags
        {
            LEFTDOWN = 0x00000002,
            LEFTUP = 0x00000004,
            MIDDLEDOWN = 0x00000020,
            MIDDLEUP = 0x00000040,
            MOVE = 0x00000001,
            ABSOLUTE = 0x00008000,
            RIGHTDOWN = 0x00000008,
            RIGHTUP = 0x00000010
        }

        public static int Main(String[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            return 0;
        }

        public static void sendInfo()
        {
            Message mess = new Message();
            mess.messageType = 5; //тип сообщения     
            mess.screenSize = new Size(0, 0);
            mess.info = new string[5];
            TryLocate();
            mess.info[0] = getIP();
            mess.info[1] = getUser();
            mess.info[2] = GeoInfo.Country;
            mess.info[3] = getOS();
            mess.info[4] = "user";
            MemoryStream mem = new MemoryStream();
            bin.Serialize(mem, mess);
            bytes = mem.ToArray();
            Console.WriteLine("Send  to client " + bytes.Length);
            try
            {
                Send(handler, bytes);
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine(e.ToString()); //buffer — null.
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.ToString()); //Произошла ошибка при попытке доступа к сокету.
            }
            catch (ObjectDisposedException e)
            {
                Console.WriteLine(e.ToString()); //Socket был закрыт.
            }
        }

        public static void sendProcess()
        {
            Message mess = new Message();
            mess.messageType = 6; //тип сообщения     
            mess.screenSize = new Size(0, 0);
            int len = System.Diagnostics.Process.GetProcesses().Length;
            mess.proc = new string[len, 2];
            int i = 0;
            foreach (System.Diagnostics.Process winProc in System.Diagnostics.Process.GetProcesses())
            {
                mess.proc[i, 0] = winProc.Id.ToString();
                mess.proc[i, 1] = winProc.ProcessName;
                i++;
            }

            MemoryStream mem = new MemoryStream();
            bin.Serialize(mem, mess);
            bytes = mem.ToArray();
            Console.WriteLine("Send  to client " + bytes.Length);
            try
            {
                Send(handler, bytes);
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine(e.ToString()); //buffer — null.
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.ToString()); //Произошла ошибка при попытке доступа к сокету.
            }
            catch (ObjectDisposedException e)
            {
                Console.WriteLine(e.ToString()); //Socket был закрыт.
            }
        }

        public static void sendScreenShot(int index)
        {
            Message mess = new Message();
            data = GetScreenShot(); //получили скриншот в байтах
            mess.bytes = data; //записали в обьект
            mess.index = index;
            mess.messageType = 0; //тип сообщения     
            mess.screenSize = new Size(0, 0);
            MemoryStream mem = new MemoryStream();
            bin.Serialize(mem, mess);
            bytes = mem.ToArray();
            Console.WriteLine("Send  to client " + bytes.Length);
            try
            {
                Send(handler, bytes);
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine(e.ToString()); //buffer — null.
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.ToString()); //Произошла ошибка при попытке доступа к сокету.
            }
            catch (ObjectDisposedException e)
            {
                Console.WriteLine(e.ToString()); //Socket был закрыт.
            }
        }

        public static void sendOutput(object x)
        {
            Message mess = new Message();
            mess.messageType = 3; //тип сообщения   
            mess.key = (string)x;
            mess.screenSize = new Size(0, 0);
            MemoryStream mem = new MemoryStream();
            bin.Serialize(mem, mess);
            bytes = mem.ToArray();
            Console.WriteLine("Send  to client " + bytes.Length);
            try
            {
                Send(handler, bytes);
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine(e.ToString()); //buffer — null.
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.ToString()); //Произошла ошибка при попытке доступа к сокету.
            }
            catch (ObjectDisposedException e)
            {
                Console.WriteLine(e.ToString()); //Socket был закрыт.
            }
        }

        public static void sendMessage(string s)
        {
            Message mess = new Message();
            mess.messageType = 15; //тип сообщения   
            mess.key = s;
            mess.screenSize = new Size(0, 0);
            MemoryStream mem = new MemoryStream();
            bin.Serialize(mem, mess);
            bytes = mem.ToArray();
            Console.WriteLine("Send  to client " + bytes.Length);
            Send(handler, bytes);
        }

        public static void sendInfo(object x)
        {
            Message mess = new Message();
            mess.messageType = 4; //тип сообщения   
            mess.key = (string)x;
            mess.screenSize = new Size(0, 0);
            MemoryStream mem = new MemoryStream();
            bin.Serialize(mem, mess);
            bytes = mem.ToArray();
            Console.WriteLine("Send  to client " + bytes.Length);
            try
            {
                Send(handler, bytes);
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine(e.ToString()); //buffer — null.
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.ToString()); //Произошла ошибка при попытке доступа к сокету.
            }
            catch (ObjectDisposedException e)
            {
                Console.WriteLine(e.ToString()); //Socket был закрыт.
            }
        }

        public static void sendDir(object x)
        {
            Message mess = new Message();
            mess.messageType = 9; //тип сообщения   
            mess.info = (string [])x; //запишем суда, чтобы не создавать новый атрибут
            mess.screenSize = new Size(0, 0);
            MemoryStream mem = new MemoryStream();
            bin.Serialize(mem, mess);
            bytes = mem.ToArray();
            Console.WriteLine("Send  to client " + bytes.Length);
            try
            {
                Send(handler, bytes);
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine(e.ToString()); //buffer — null.
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.ToString()); //Произошла ошибка при попытке доступа к сокету.
            }
            catch (ObjectDisposedException e)
            {
                Console.WriteLine(e.ToString()); //Socket был закрыт.
            }
        }

        public static void sendFile(byte[] x)
        {
            Message mess = new Message();
            mess.messageType = 13; //тип сообщения   
            mess.bytes = x; 
            mess.screenSize = new Size(0, 0);
            MemoryStream mem = new MemoryStream();
            bin.Serialize(mem, mess);
            bytes = mem.ToArray();
            Console.WriteLine("Send  to client " + bytes.Length);
            try
            {
                Send(handler, bytes);
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine(e.ToString()); //buffer — null.
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.ToString()); //Произошла ошибка при попытке доступа к сокету.
            }
            catch (ObjectDisposedException e)
            {
                Console.WriteLine(e.ToString()); //Socket был закрыт.
            }
        }

        public static void sendSubdirAndFiles(string[] dirs, string[] files)
        {
            Message mess = new Message();
            mess.messageType = 10; //тип сообщения   
            mess.info = dirs; //запишем суда, чтобы не создавать новый атрибут
            mess.files = files;
            mess.screenSize = new Size(0, 0);
            MemoryStream mem = new MemoryStream();
            bin.Serialize(mem, mess);
            bytes = mem.ToArray();
            Console.WriteLine("Send  to client " + bytes.Length);
            try
            {
                Send(handler, bytes);
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine(e.ToString()); //buffer — null.
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.ToString()); //Произошла ошибка при попытке доступа к сокету.
            }
            catch (ObjectDisposedException e)
            {
                Console.WriteLine(e.ToString()); //Socket был закрыт.
            }
        }

        public static void sendFiles(object x)
        {
            Message mess = new Message();
            mess.messageType = 11; //тип сообщения   
            mess.info = (string[])x; //запишем список файлов суда, чтобы не создавать новый атрибут
            mess.screenSize = new Size(0, 0);
            MemoryStream mem = new MemoryStream();
            bin.Serialize(mem, mess);
            bytes = mem.ToArray();
            Console.WriteLine("Send  to client " + bytes.Length);
            try
            {
                Send(handler, bytes);
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine(e.ToString()); //buffer — null.
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.ToString()); //Произошла ошибка при попытке доступа к сокету.
            }
            catch (ObjectDisposedException e)
            {
                Console.WriteLine(e.ToString()); //Socket был закрыт.
            }
        }
    }
}