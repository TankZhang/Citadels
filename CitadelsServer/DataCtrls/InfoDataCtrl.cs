﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using CitadelsServer.DataCtrls;
using CitadelsServer.Datas;
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
                    { gameuser.Status = "注册邮箱已存在"; return gameuser; }
                     RegisterDeal(mySQLCtrl, socket, infos, out gameuser); 
                    return gameuser;
                //收到登陆数据
                case '1':
                    string[] loadInfo = DataCtrl.SegmentData(infoStr.Substring(1));
                    if (!mySQLCtrl.IsExistInDb(loadInfo[0]))
                    { gameuser.Status = "登录邮箱未注册"; return gameuser; }
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
        public static string RoomDataDeal(GameDataCenter gameDataCenter, Socket socket, string roomStr)
        {
            switch (roomStr[0])
            {
                //创建房间的信息，房间号++，建立socket的list，加入当前socket，将当前list加入dic，如果对应的房间号的list包含此socket，返回true，否则，返回false。
                case '0':
                    gameDataCenter.RoomNum++;
                    List<Socket> l = new List<Socket>();
                    l.Add(socket);
                    gameDataCenter.RoomSeatSockets.Add(gameDataCenter.RoomNum, l);
                    if (gameDataCenter.RoomSeatSockets[gameDataCenter.RoomNum].Contains(socket))
                    {
                        return "10" + gameDataCenter.RoomNum.ToString() + "|" + gameDataCenter.RoomSeatSockets[gameDataCenter.RoomNum].Count.ToString();
                    }
                    else
                    {
                       
                        gameDataCenter.RoomNum--;
                        return "创建失败";
                    }
                //加入房间的信息，先取得房间号i,判断房间号i存在否，存在则将此socket加入list，如果此list含此socket，返回true。
                case '1':
                    int i;
                    int.TryParse(roomStr.Substring(1), out i);
                    if (!gameDataCenter.RoomSeatSockets.Keys.Contains(i)) { return "加入失败"; }
                    gameDataCenter.RoomSeatSockets[i].Add(socket);
                    if (gameDataCenter.RoomSeatSockets[i].Contains(socket))
                    { return "10" + gameDataCenter.RoomNum.ToString() + "|" + gameDataCenter.RoomSeatSockets[gameDataCenter.RoomNum].Count.ToString(); }
                    else
                    { return "加入失败"; }
                default: return "未知错误";
            }
        }
    }
}
