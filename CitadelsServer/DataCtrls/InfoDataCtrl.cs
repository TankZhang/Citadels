using System;
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
            string[] strTemp = DataCtrl.SegmentData(infoStr);
            GameUser gameuser = new GameUser();
            switch (strTemp[0])
            {
                //收到注册数据
                case "0":
                    string[] infos = DataCtrl.SegmentData(infoStr.Substring(2));
                    if (mySQLCtrl.IsExistInDb(infos[0]))
                    { gameuser.Status = "0|0|0|注册邮箱已存在|"; return gameuser; }
                    if (mySQLCtrl.IsNickNameExistInDb(infos[1]))
                    { gameuser.Status = "0|0|0|注册昵称已存在|"; return gameuser; }
                    RegisterDeal(mySQLCtrl, socket, infos, out gameuser); 
                    return gameuser;
                //收到登陆数据
                case "1":
                    string[] loadInfo = DataCtrl.SegmentData(infoStr.Substring(2));
                    if (!mySQLCtrl.IsExistInDb(loadInfo[0]))
                    { gameuser.Status = "0|1|0|登录邮箱未注册|"; return gameuser; }
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
                gameuser.Status = "0|0|1|";
                return true;
            }
            gameuser = null;
            gameuser.Status = "0|0|0|注册失败，请重试|";
            return false;
        }
        //处理登陆数据
        public static bool SignDeal(MySQLCtrl mySQLCtrl, Socket socket, string[] landInfos, out GameUser gameuser)
        {
            gameuser = mySQLCtrl.SelecInDb(landInfos[0]);
            if (landInfos[1] == gameuser.Pwd)
            {
                gameuser.Status = "0|1|1|";
                return true;
            }
            else
            {
                gameuser.Status = "0|1|0|登陆失败，请检查密码或重试|";
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
            string[] strTemp = DataCtrl.SegmentData(roomStr);
            switch (strTemp[0])
            {
                //创建房间的信息，房间号++，建立socket的list，加入当前socket，将当前list加入dic，如果对应的房间号的list包含此socket,加入对应的playerdata，返回true，否则，返回false。
                case "0":
                    int roomNum = 1;
                    while(gameDataCenter.RoomDataDic.Keys.Contains(roomNum))
                    {
                        roomNum++;
                    }
                    gameDataCenter.RoomNum= roomNum;
                    List<Socket> l = new List<Socket>();
                    l.Add(socket);
                    gameDataCenter.RoomSeatSockets.Add(gameDataCenter.RoomNum, l);
                    if (gameDataCenter.RoomSeatSockets[gameDataCenter.RoomNum].Contains(socket))
                    {
                        GameRoomData gameRoomData = new GameRoomData();
                        PlayerData playerData = new PlayerData(1, gameDataCenter.MailNickDic[strTemp[1]],strTemp[1],socket);
                        playerData.IsKing = true;
                        gameRoomData.PlayerDataDic.Add(1,playerData);
                        gameDataCenter.RoomDataDic.Add(gameDataCenter.RoomNum, gameRoomData);
                        return "1|0|1|" + gameDataCenter.RoomNum.ToString() + "|" + gameDataCenter.RoomSeatSockets[gameDataCenter.RoomNum].Count.ToString() + "|";
                    }
                    else
                    {
                       
                        gameDataCenter.RoomNum--;
                        return "1|0|0|创建失败|";
                    }
                //加入房间的信息，先取得房间号i,判断房间号i存在否，存在则将此socket加入list，如果此list含此socket，返回true。
                case "1":
                    int i;
                    int.TryParse(strTemp[1], out i);
                    if (!gameDataCenter.RoomSeatSockets.Keys.Contains(i)) { return "1|1|0|加入失败|"; }
                    gameDataCenter.RoomSeatSockets[i].Add(socket);
                    if (gameDataCenter.RoomSeatSockets[i].Contains(socket))
                    {
                        PlayerData playerData = new PlayerData(gameDataCenter.RoomSeatSockets[i].Count, gameDataCenter.MailNickDic[strTemp[1]], strTemp[1],socket);
                        gameDataCenter.RoomDataDic[i].PlayerDataDic.Add(gameDataCenter.RoomDataDic[i].PlayerDataDic.Count+1, playerData);
                        return "1|1|1|" + i + "|" + gameDataCenter.RoomSeatSockets[i].Count.ToString() + "|"; }
                    else
                    { return "1|1|0|加入失败|"; }
                //更新房间信息
                case "2":
                    string strRoomDataUpdate = RoomDataUpdate(gameDataCenter, socket, strTemp);
                    return strRoomDataUpdate;

                    //将这个socket加入到大厅玩家列表中
                    gameDataCenter.LobbySocketList.Add(socket);
                    string str = "1|4|";
                    foreach (var item in gameDataCenter.RoomDataDic)
                    {
                        str += (item.Key + "|");
                        str += (item.Value.PlayerDataDic.Count + "|");
                        str += (item.Value.PlayerDataDic[1].NickName + "|");
                    }
                    return str;
                default: return "未知错误";

            }
        }
        //处理更新房间信息的请求
        private static string RoomDataUpdate(GameDataCenter gameDataCenter, Socket socket, string[] strTemp)
        {
            switch(strTemp[1])
            {
                case "0":
                    gameDataCenter.LobbySocketList.Add(socket);
                    string str = "1|4|";
                    foreach (var item in gameDataCenter.RoomDataDic)
                    {
                        str += (item.Key + "|");
                        str += (item.Value.PlayerDataDic.Count + "|");
                        str += (item.Value.PlayerDataDic[1].NickName + "|");
                    }
                    return str;
                default:return;
            }
        }
    }
}
