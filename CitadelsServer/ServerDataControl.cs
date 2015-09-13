using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CitadelsServer
{
    public static class ServerDataControl
    {
        public static string[] Ips2Strings(IPAddress[] ips)
        {
            string[] strs = new string[ips.Count()];
            for (int i = 0; i < ips.Count(); i++)
            {
                strs[i] = ips[i].ToString();
            }
            return strs;
        }
        //分割字符串
        public static string[] SegmentData(string str)
        {
            string[] sss = str.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            return sss;
        }
        //处理服务器收到的信息数据,返回gameuser信息
        public static GameUser InfoDataDeal(ServerMySQLControl mySQLControl, Socket socket, string infoStr)
        {
            GameUser gameuser = new GameUser();
            switch (infoStr[0])
            {
                //收到注册数据
                case '0': if(!RegisterDeal(mySQLControl,socket, infoStr.Substring(1), out gameuser))
                    { Console.WriteLine("注册失败"); return null; }
                    return gameuser;
                //收到登陆数据
                case '1':
                    if (!SignDeal(mySQLControl, socket, infoStr.Substring(1), out gameuser))
                    { Console.WriteLine("登陆失败"); return null; }
                    return gameuser;
                default: return null;
            }
        }
        //处理注册数据
        private static bool RegisterDeal(ServerMySQLControl mySQLControl, Socket socket, string str, out GameUser gameuser)
        {
            string[] info = SegmentData(str);
            if (mySQLControl.IsExistInDb(info[0]))
            {
                gameuser = null;
                return false;
            }
            if( mySQLControl.InsertToDb(info[0], info[1], info[2], info[3]))
            {
                gameuser = new GameUser(info[0], info[1], info[2], info[3], "0");
                Console.WriteLine("注册成功");
                return true;
            }
            gameuser = null;
            return false;

        }

        //处理登陆数据
        public static bool SignDeal(ServerMySQLControl mySQLControl, Socket socket, string landStr, out GameUser gameuser)
        {
            string[] info = SegmentData(landStr);
            gameuser = mySQLControl.SelecInDb(info[0]);
            if (gameuser.Mail == "null")
            { Console.WriteLine("无此邮箱"); return false; }
            if (info[1] == gameuser.Pwd)
            {
                Console.WriteLine("登陆成功");
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 创建房间数据处理        
        /// </summary>
        /// <param name="roomSeatSockets">要操作的socket字典</param>
        /// <param name="roomNum">要操作的房间号</param>
        /// <param name="socket">对应的客户端socket</param>
        /// <param name="roomStr">对应的接收到的房间数据</param>
        /// <returns>成功返回true，否则返回false</returns>
        public static bool RoomDataDeal(ref Dictionary<int, List<Socket>> roomSeatSockets, ref int roomNum, Socket socket, string roomStr)
        {
            switch (roomStr[0])
            {
                //创建房间的信息，房间号++，建立socket的list，加入当前socket，将当前list加入dic，如果对应的房间号的list包含此socket，返回true，否则，返回false。
                case '0':
                    roomNum++;
                    List<Socket> l = new List<Socket>();
                    l.Add(socket);
                    roomSeatSockets.Add(roomNum, l);
                    if (roomSeatSockets[roomNum].Contains(socket))
                    {
                        Console.WriteLine("创建房间成功"); return true;
                    }
                    else
                    {
                        Console.WriteLine("创建房间失败");
                        roomNum--;
                        return false;
                    }
                //加入房间的信息，先取得房间号i,判断房间号i存在否，存在则将此socket加入list，如果此list含此socket，返回true。
                case '1':
                    int i;
                    int.TryParse(roomStr.Substring(1), out i);
                    if (!roomSeatSockets.Keys.Contains(i)) { Console.WriteLine("不存在此房间"); return false; }
                    roomSeatSockets[i].Add(socket);
                    if (roomSeatSockets[i].Contains(socket))
                    { return true; }
                    else
                    { return false; }
                default: break;
            }
            return false;
        }
    }
}
