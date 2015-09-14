using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using CitadelsServer.DataCtrls;

namespace CitadelsServer.DataCtrls
{
   public static class InfoDataCtrl
    {
        //处理服务器收到的信息数据,返回gameuser信息
        public static GameUser InfoDataDeal(MySQLCtrl mySQLCtrl, Socket socket, string infoStr)
        {
            GameUser gameuser = new GameUser();
            switch (infoStr[0])
            {
                //收到注册数据
                case '0':
                    string[] infos = DataCtrl.SegmentData(infoStr.Substring(1));
                    if (mySQLCtrl.IsExistInDb(infos[0]))
                    { gameuser.Status = "注册邮箱已存在";Console.WriteLine(gameuser.Status); return gameuser; }
                     RegisterDeal(mySQLCtrl, socket, infos, out gameuser); 
                    return gameuser;
                //收到登陆数据
                case '1':
                    string[] loadInfo = DataCtrl.SegmentData(infoStr.Substring(1));
                    if (!mySQLCtrl.IsExistInDb(loadInfo[0]))
                    { gameuser.Status = "注册邮箱不存在"; Console.WriteLine(gameuser.Status); return gameuser; }
                    SignDeal(mySQLCtrl, socket, loadInfo, out gameuser);
                    return gameuser;
                default: return null;
            }

        }

        //处理注册数据
        private static bool RegisterDeal(MySQLCtrl mySQLCtrl, Socket socket, string[] infos, out GameUser gameuser)
        {
            if (mySQLCtrl.InsertToDb(infos[0], infos[1], infos[2], infos[3]))
            {
                gameuser = new GameUser(infos[0], infos[1], infos[2], infos[3], "0");
                gameuser.Status = "注册成功";
                return true;
            }
            gameuser = null;
            gameuser.Status = "注册失败，请重试";
            return false;
        }
        //处理登陆数据
        public static bool SignDeal(MySQLCtrl mySQLCtrl, Socket socket, string[] landInfos, out GameUser gameuser)
        {
            gameuser = mySQLCtrl.SelecInDb(landInfos[0]);
            if (landInfos[1] == gameuser.Pwd)
            {
                gameuser.Status = "登陆成功";
                return true;
            }
            else
            {
                gameuser.Status = "登陆失败，请检查密码或重试";
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
