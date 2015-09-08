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
    public  class ServerNetControl
    {
        private Socket _socketWatch;
        //生成服务器监听的socket
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

        private Dictionary<string, Socket> _dicSocket;
        //生成与每个客户交流的socket字典
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

        /// <summary>
        /// 返回本机IP
        /// </summary>
        /// <returns>本机的IP的数组</returns>
        static IPAddress[] GetLocalIP()
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
                    switch (buffer[0])
                    {
                        //将远程连接的客户端的别名和Socket存入集合中
                        case 0:
                            DicSocket.Add(Encoding.UTF8.GetString(buffer, 1, r - 1), socketSend);
                            break;
                        //收到你好时候的反应
                        case 1:
                            string str = Encoding.UTF8.GetString(buffer, 1, r - 1);
                            Console.WriteLine("我收到" + str);
                            //由于调用线程无法访问此对象,因为另一个线程拥有该对象，因此使用这个函数来调用UI对象
                            break;
                    }//switch结束
                }
                catch (Exception ex)
                {
                    //抛出异常信息
                    Console.WriteLine(ex.Message.ToString()); 
                }
            }
        }
    }
}
