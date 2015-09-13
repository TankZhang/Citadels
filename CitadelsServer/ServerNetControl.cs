using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CitadelsServer
{
    public class ServerNetControl
    {
        //生成服务器监听的socket
        private Socket _socketWatch;
        public Socket SocketWatch
        {
            get
            {
                return _socketWatch;
            }

            set
            {
                _socketWatch = value;
            }
        }
        //生成与每个客户交流的socket字典
        private Dictionary<string, Socket> _dicSocket;
        public Dictionary<string, Socket> DicSocket
        {
            get
            {
                return _dicSocket;
            }

            set
            {
                _dicSocket = value;
            }
        }
        //生成房间号
        private int _roomNum;
        public int RoomNum
        {
            get
            {
                return _roomNum;
            }

            set
            {
                _roomNum = value;
            }
        }

        //生成房间座位socket字典
        private Dictionary<int, List<Socket>> _roomSeatSocket;
        public Dictionary<int, List<Socket>> RoomSeatSocket
        {
            get
            {
                return _roomSeatSocket;
            }

            set
            {
                _roomSeatSocket = value;
            }
        }


        /// <summary>
        /// 返回本机IP
        /// </summary>
        /// <returns>本机的IP的数组</returns>
        public static IPAddress[] GetLocalIP()
        {
            string hostname = Dns.GetHostName();
            IPHostEntry localhost = Dns.GetHostEntry(hostname);
            return localhost.AddressList;
        }
        /// <summary>
        /// 监听socket开始监听
        /// </summary>
        /// <param name="o">监听socket</param>
        void Listen(object o)
        {
            Socket socketTemp = o as Socket;
            while (true)
            {
                Socket socketSend;
                try
                {
                    //负责与客户端通信的socket
                    socketSend = socketTemp.Accept();
                    //连接成功
                    Console.WriteLine(socketSend.RemoteEndPoint.ToString() + ":" + "连接成功");
                    //开启一个新的线程不断地接收客户端发过来的消息
                    Thread th = new Thread(Receive);
                    th.IsBackground = true;
                    th.Start(socketSend);
                }//try结束
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message.ToString()); //抛出异常信息
                }
            }
        }
        /// <summary>
        /// 接收信息
        /// </summary>
        /// <param name="o"></param>
        void Receive(object o)
        {
            Socket socketSend = o as Socket;
            while (true)
            {
                try
                {
                    //客户端连接成功后，服务器应该接收客户端发来的消息
                    byte[] buffer = new byte[1024 * 1024 * 2];
                    int r = socketSend.Receive(buffer);
                    if (r == 0)
                    { break; }
                    string str = Encoding.UTF8.GetString(buffer, 0, r);
                    Console.WriteLine(str);
                    switch (str[0])
                    {
                        //处理登陆注册信息
                        case '0':
                            GameUser gameuser = ServerDataControl.InfoDataDeal(App.serverViewModel.serverMySQLControl, socketSend, str.Substring(1));
                            if (gameuser != null)
                            {
                                DicSocket.Add(gameuser.Mail, socketSend);
                                Console.WriteLine("登陆成功");
                            }
                            else
                            {
                                Send(socketSend, "9登陆失败");
                            }
                            break;
                        //处理房间座位信息
                        case '1':
                            if (ServerDataControl.RoomDataDeal(ref _roomSeatSocket, ref _roomNum, socketSend, str.Substring(1)))
                            {
                                //返回给客户端自己的房间号和座位号
                                Send(socketSend, "10"+ RoomNum.ToString() + "|"+RoomSeatSocket[RoomNum].Count.ToString());
                                Console.WriteLine("创建房间成功");
                            }
                            else
                            {
                                Send(socketSend, "9创建房间失败");
                            }
                            break;
                        default: Console.WriteLine("接收到错误信息"); break;
                    }
                }
                catch (Exception ex)
                {
                    //抛出异常信息
                    Console.WriteLine(ex.Message.ToString());
                }
            }
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="str"></param>
        void Send(Socket socket, string str)
        {
            try
            {
                byte[] buffer = Encoding.UTF8.GetBytes(str);
                socket.Send(buffer);
            }
            catch (Exception ex)
            {
                //抛出异常信息
                Console.WriteLine(ex.Message.ToString());
            }
        }
        //构造函数
        public ServerNetControl()
        { }
        public ServerNetControl(string ipAddr, string port)
        {
            SocketWatch = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ip = IPAddress.Parse(ipAddr);
            IPEndPoint point = new IPEndPoint(ip, Convert.ToInt32(port));
            DicSocket = new Dictionary<string, Socket>();
            RoomSeatSocket = new Dictionary<int, List<Socket>>();
            SocketWatch.Bind(point);
            SocketWatch.Listen(10);
            Thread th = new Thread(Listen);
            th.IsBackground = true;
            th.Start(SocketWatch);
            RoomNum = 0;
            Console.WriteLine("监听成功");
        }
    }
}
