using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using CitadelsServer.Datas;
using CitadelsServer.Buildings;

namespace CitadelsServer.DataCtrls
{
    public static class DataCtrl
    {
        //处理客户端传过来的所有信息
        public static void DealData(GameDataCenter gameDataCenter, Socket socket, string str)
        {
            string[] strTemp = SegmentData(str);
            switch (strTemp[0])
            {
                //处理登陆注册信息
                case "0":
                    GameUser gameuser = InfoDataCtrl.InfoDataDeal(App.viewModel.MySQLCtrl, socket, str.Substring(2));
                    NetCtrl.Send(socket, gameuser.Status);
                    gameDataCenter.MailNickDic.Add(gameuser.Mail, gameuser.NickName);
                    Console.WriteLine(gameuser.Status);
                    break;
                //处理房间座位信息
                case "1":
                    string roomStatus = InfoDataCtrl.RoomDataDeal(gameDataCenter, socket, str.Substring(2));
                    NetCtrl.Send(socket, roomStatus);
                    Console.WriteLine(roomStatus);
                    int roomToStart = 0;
                    if (IsStartEnable(gameDataCenter, socket, roomStatus, ref roomToStart))
                    {
                        NetCtrl.Send(gameDataCenter.RoomSeatSockets[roomToStart][0], "1|2|");
                        Console.WriteLine(roomToStart + "号房间可以开始了");
                    }
                    break;
                //处理游戏中的数据
                case "2":
                    DealGameData(gameDataCenter, str.Substring(2));
                    break;
                default: Console.WriteLine("接收到错误信息"); break;
            }
        }

        //所有的游戏数据的处理
        private static void DealGameData(GameDataCenter gameDataCenter, string str)
        {
            string[] gameStrs = SegmentData(str);
            switch (gameStrs[2])
            {
                //对开始游戏的处理
                case "0": DealStartGame(gameDataCenter, gameStrs); break;
                //对选择角色的处理
                case "1": DealSelectHero(gameDataCenter, gameStrs); break;
                //对选择玩家的处理
                case "2": DealSelectPlayer(gameDataCenter, gameStrs); break;
                //对选择建筑的处理
                case "3": DealSelectBuilding(gameDataCenter, gameStrs); break;
                default: return;
            }
        }

        //对选择建筑的处理
        private static void DealSelectBuilding(GameDataCenter gameDataCenter, string[] gameStrs)
        {
            switch (gameStrs[3])
            {
                //对纳入囊中的处理
                case "0": DealSelectBuildingToMe(gameDataCenter, gameStrs); break;
                //对进行建筑的处理
                case "1": DealSelectBuildingToBuild(gameDataCenter, gameStrs); break;
                //对拆牌建筑的处理
                case "2": DealSelectBuildingToBreak(gameDataCenter, gameStrs); break;
                default: return;
            }
        }
        //将某个对象的某张牌拆掉
        private static void DealSelectBuildingToBreak(GameDataCenter gameDataCenter, string[] gameStrs)
        {
            int roomNum = Convert.ToInt32(gameStrs[0]);
            int seatNum = Convert.ToInt32(gameStrs[1]);
            int targetSeatNum = Convert.ToInt32(gameStrs[4]);
            int id = Convert.ToInt32(gameStrs[5]);
            foreach (var item in gameDataCenter.RoomDataDic[roomNum].PlayerDataDic[targetSeatNum].TableBuilding)
            {
                if (item.ID == id)
                {
                    gameDataCenter.RoomDataDic[roomNum].BackBuildings.Add(item);
                    gameDataCenter.RoomDataDic[roomNum].PlayerDataDic[targetSeatNum].TableBuilding.Remove(item);
                }
            }
        }

        //选择建筑牌进行建筑
        private static void DealSelectBuildingToBuild(GameDataCenter gameDataCenter, string[] gameStrs)
        {
            int roomNum = Convert.ToInt32(gameStrs[0]);
            int seatNum = Convert.ToInt32(gameStrs[1]);
            switch (gameStrs[4])
            {
                //建筑1张牌的时候
                case "0":
                    int id = Convert.ToInt32(gameStrs[5]);
                    foreach (var item in gameDataCenter.RoomDataDic[roomNum].PlayerDataDic[seatNum].PocketBuildings)
                    {
                        if (item.ID == id)
                        {
                            gameDataCenter.RoomDataDic[roomNum].PlayerDataDic[seatNum].TableBuilding.Add(item);
                            gameDataCenter.RoomDataDic[roomNum].PlayerDataDic[seatNum].PocketBuildings.Remove(item);
                        }
                    }
                    break;
                //建筑多张牌的时候
                case "1":
                    int bNum = gameStrs.Length - 5;
                    List<int> idList = new List<int>();
                    for (int i = 0; i < bNum; i++)
                    {
                        idList.Add(Convert.ToInt32(gameStrs[i + 5]));
                    }
                    foreach (var item in gameDataCenter.RoomDataDic[roomNum].PlayerDataDic[seatNum].PocketBuildings)
                    {
                        if (idList.Contains(item.ID))
                        {
                            gameDataCenter.RoomDataDic[roomNum].PlayerDataDic[seatNum].TableBuilding.Add(item);
                            gameDataCenter.RoomDataDic[roomNum].PlayerDataDic[seatNum].PocketBuildings.Remove(item);
                        }
                    }
                    break;
                default: return;
            }
        }

        //选择建筑牌拿到自己的手中
        private static void DealSelectBuildingToMe(GameDataCenter gameDataCenter, string[] gameStrs)
        {
            int roomNum = Convert.ToInt32(gameStrs[0]);
            int seatNum = Convert.ToInt32(gameStrs[1]);
            switch (gameStrs[4])
            {
                case "0":
                    int id = Convert.ToInt32(gameStrs[5]);
                    foreach (var item in gameDataCenter.RoomDataDic[roomNum].TableBuildings)
                    {
                        if (item.ID == id)
                        {
                            gameDataCenter.RoomDataDic[roomNum].PlayerDataDic[seatNum].PocketBuildings.Add(item);
                            gameDataCenter.RoomDataDic[roomNum].TableBuildings.Remove(item);
                            gameDataCenter.RoomDataDic[roomNum].BackBuildings.AddRange(gameDataCenter.RoomDataDic[roomNum].TableBuildings);
                            gameDataCenter.RoomDataDic[roomNum].TableBuildings.Clear();
                        }
                    }
                    break;
                case "1":
                    int id1 = Convert.ToInt32(gameStrs[5]);
                    int id2 = Convert.ToInt32(gameStrs[6]);
                    foreach (var item in gameDataCenter.RoomDataDic[roomNum].TableBuildings)
                    {
                        if (item.ID == id1)
                        {
                            gameDataCenter.RoomDataDic[roomNum].PlayerDataDic[seatNum].PocketBuildings.Add(item);
                            gameDataCenter.RoomDataDic[roomNum].TableBuildings.Remove(item);
                        }
                        if (item.ID == id2)
                        {
                            gameDataCenter.RoomDataDic[roomNum].PlayerDataDic[seatNum].PocketBuildings.Add(item);
                            gameDataCenter.RoomDataDic[roomNum].TableBuildings.Remove(item);
                        }
                    }
                    gameDataCenter.RoomDataDic[roomNum].BackBuildings.AddRange(gameDataCenter.RoomDataDic[roomNum].TableBuildings);
                    gameDataCenter.RoomDataDic[roomNum].TableBuildings.Clear();
                    break;
                default: return;
            }
        }

        //对选择玩家的处理
        private static void DealSelectPlayer(GameDataCenter gameDataCenter, string[] gameStrs)
        {
            int roomNum = Convert.ToInt32(gameStrs[0]);
            int targetSeatNum = Convert.ToInt32(gameStrs[4]);
            int mySeatNum = Convert.ToInt32(gameStrs[1]);
            List<Building> listTemp = new List<Building>();
            listTemp.AddRange(gameDataCenter.RoomDataDic[roomNum].PlayerDataDic[targetSeatNum].PocketBuildings);
            gameDataCenter.RoomDataDic[roomNum].PlayerDataDic[targetSeatNum].PocketBuildings.Clear();
            gameDataCenter.RoomDataDic[roomNum].PlayerDataDic[targetSeatNum].PocketBuildings.AddRange(gameDataCenter.RoomDataDic[roomNum].PlayerDataDic[mySeatNum].PocketBuildings);
            gameDataCenter.RoomDataDic[roomNum].PlayerDataDic[mySeatNum].PocketBuildings.Clear();
            gameDataCenter.RoomDataDic[roomNum].PlayerDataDic[mySeatNum].PocketBuildings.AddRange(listTemp);
        }

        //对选择角色的请求
        private static void DealSelectHero(GameDataCenter gameDataCenter, string[] gameStrs)
        {
            switch (gameStrs[3])
            {
                case "0": DealSelectHeroToMe(gameDataCenter, gameStrs); break;
                case "1": DealSelectHeroToBack(gameDataCenter, gameStrs); break;
                case "2": DealSelectHeroToKill(gameDataCenter, gameStrs); break;
                case "3": DealSelectHeroToStole(gameDataCenter, gameStrs); break;
                default: return;
            }
        }
        //选择某个英雄被偷窃
        private static void DealSelectHeroToStole(GameDataCenter gameDataCenter, string[] gameStrs)
        {

            int roomNum = Convert.ToInt32(gameStrs[0]);
            int seatNum = Convert.ToInt32(gameStrs[1]);
            int ID = Convert.ToInt32(gameStrs[4]);
            foreach (var item in gameDataCenter.RoomDataDic[roomNum].PlayerDataDic)
            {
                foreach (var role in item.Value.Role)
                {
                    if (role.ID == ID) { item.Value.IsStole = true; }
                }
            }
        }

        //选择某个英雄被杀害
        private static void DealSelectHeroToKill(GameDataCenter gameDataCenter, string[] gameStrs)
        {
            int roomNum = Convert.ToInt32(gameStrs[0]);
            int seatNum = Convert.ToInt32(gameStrs[1]);
            int ID = Convert.ToInt32(gameStrs[4]);
            foreach (var item in gameDataCenter.RoomDataDic[roomNum].PlayerDataDic)
            {
                foreach (var role in item.Value.Role)
                {
                    if (role.ID == ID) { item.Value.IsKill = true; }
                }
            }

        }

        //选择英雄到背面
        private static void DealSelectHeroToBack(GameDataCenter gameDataCenter, string[] gameStrs)
        {
            int roomNum = Convert.ToInt32(gameStrs[0]);
            int seatNum = Convert.ToInt32(gameStrs[1]);
            int ID = Convert.ToInt32(gameStrs[4]);
            foreach (var item in gameDataCenter.RoomDataDic[roomNum].TableHeros)
            {
                if (item.ID == ID)
                {
                    gameDataCenter.RoomDataDic[roomNum].BackHeros.Add(item);
                    gameDataCenter.RoomDataDic[roomNum].TableHeros.Remove(item);
                }
            }
            SendHerosToNext(gameDataCenter, roomNum, seatNum);
        }

        //选择英雄到自己的名下
        private static void DealSelectHeroToMe(GameDataCenter gameDataCenter, string[] gameStrs)
        {
            int roomNum = Convert.ToInt32(gameStrs[0]);
            int seatNum = Convert.ToInt32(gameStrs[1]);
            int ID = Convert.ToInt32(gameStrs[4]);
            int nums = gameDataCenter.RoomDataDic[roomNum].PlayerDataDic.Count;
            foreach (var item in gameDataCenter.RoomDataDic[roomNum].TableHeros)
            {
                if (item.ID == ID)
                {
                    gameDataCenter.RoomDataDic[roomNum].PlayerDataDic[seatNum].Role.Add(item);
                    gameDataCenter.RoomDataDic[roomNum].HeroToPlayer.Add(item.ID, seatNum);
                    gameDataCenter.RoomDataDic[roomNum].TableHeros.Remove(item);
                }
            }
            //如果不是king还要盖住一个牌
            if (!gameDataCenter.RoomDataDic[roomNum].PlayerDataDic[seatNum].IsKing)
            {
                NetCtrl.Send(gameDataCenter.RoomDataDic[roomNum].PlayerDataDic[seatNum].Socket, "2|4|1|" + GetTableHeroString(gameDataCenter, roomNum));
            }
            else
            {
                SendHerosToNext(gameDataCenter, roomNum, seatNum);
            }
        }
        //将Heros发给下一家进行选牌
        private static void SendHerosToNext(GameDataCenter gameDataCenter, int roomNum, int seatNum)
        {
            int playerNum = gameDataCenter.RoomDataDic[roomNum].PlayerDataDic.Count;
            int nextPlayerNum;
            if (seatNum != playerNum) { nextPlayerNum = seatNum + 1; }
            else { nextPlayerNum = 1; }
            //如果下家有皇冠，游戏开始；否则，下家继续选择角色牌
            if (gameDataCenter.RoomDataDic[roomNum].PlayerDataDic[nextPlayerNum].IsKing)
            {
                int i = 0;
                while (!gameDataCenter.RoomDataDic[roomNum].HeroToPlayer.Keys.Contains(i))
                { i++; }
                int startSeat = gameDataCenter.RoomDataDic[roomNum].HeroToPlayer[i];
                NetCtrl.Send(gameDataCenter.RoomDataDic[roomNum].PlayerDataDic[startSeat].Socket, "2|5|");
            }
            else
            {
                NetCtrl.Send(gameDataCenter.RoomDataDic[roomNum].PlayerDataDic[nextPlayerNum].Socket, "2|4|0|" + GetTableHeroString(gameDataCenter, roomNum));
            }
        }

        //对开始游戏的处理,房间号|座号|开始游戏
        private static void DealStartGame(GameDataCenter gameDataCenter, string[] gameStrs)
        {
            int roomNum = Convert.ToInt32(gameStrs[0]);
            //游戏正式开始
            GroupSend(gameDataCenter, roomNum, "1|3|");
            foreach (var item in gameDataCenter.RoomDataDic[roomNum].PlayerDataDic)
            {
                item.Value.PocketBuildings.Add(gameDataCenter.RoomDataDic[roomNum].BackBuildings[0]);
                gameDataCenter.RoomDataDic[roomNum].BackBuildings.RemoveAt(0);
                item.Value.PocketBuildings.Add(gameDataCenter.RoomDataDic[roomNum].BackBuildings[0]);
                gameDataCenter.RoomDataDic[roomNum].BackBuildings.RemoveAt(0);
                item.Value.Money = 2;
                //发给特定的人告诉他的手牌和钱
                NetCtrl.Send(item.Value.Socket, "2|0|" + item.Value.PocketBuildings[0].ID + "|" + item.Value.PocketBuildings[1].ID + "|" + "2|");
                //群发消息更改特定人的钱
                GroupSend(gameDataCenter, roomNum, "2|1|" + item.Value.Seat + "|2|");
            }
            foreach (var item in gameDataCenter.RoomDataDic[roomNum].PlayerDataDic)
            {
                if (item.Value.IsKing)
                {
                    gameDataCenter.RoomDataDic[roomNum].TableHeros = gameDataCenter.RoomDataDic[roomNum].BackHeros;
                    gameDataCenter.RoomDataDic[roomNum].BackHeros.Clear();
                    gameDataCenter.RoomDataDic[roomNum].BackHeros.Add(gameDataCenter.RoomDataDic[roomNum].TableHeros[0]);
                    gameDataCenter.RoomDataDic[roomNum].TableHeros.RemoveAt(0);
                    string s = "2|4|0|";
                    foreach (var item1 in gameDataCenter.RoomDataDic[roomNum].TableHeros)
                    {
                        s += (item1.ID + "|");
                    }
                    NetCtrl.Send(item.Value.Socket, s);
                }
            }
        }

        //得到桌子上的角色牌的string
        private static string GetTableHeroString(GameDataCenter gameDataCenter, int roomNum)
        {
            string s = "";
            foreach (var item in gameDataCenter.RoomDataDic[roomNum].TableHeros)
            {
                s += (item.ID + "|");

            }
            return s;
        }
        //判断是否可以开始游戏，如果可以开始，返回给对应的房间的创始人
        private static bool IsStartEnable(GameDataCenter gameDataCenter, Socket socket, string roomStatus, ref int roomToStart)
        {
            string[] status = SegmentData(roomStatus);
            if (status[2] == "0") return false;
            if (status[4] == "2")
            {
                int.TryParse(status[3], out roomToStart);
                return true;
            }
            return false;
        }

        //分割字符串
        public static string[] SegmentData(string str)
        {
            string[] sss = str.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            return sss;
        }

        //打乱整型数组的顺序
        public static void RandOrder(ref int[] ints)
        {
            int num = ints.Length;
            int[] intss = new int[num];
            Random rand = new Random();
            int randInt = rand.Next(1, num);
            for (int i = 0; i < num; i++)
            {
                while (intss.Contains(randInt))
                {
                    randInt = rand.Next(1, num + 1);
                }
                intss[i] = randInt;
            }
            int[] result = new int[num];
            for (int i = 0; i < num; i++)
            {
                result[i] = ints[intss[i] - 1];
            }
            ints = result;
        }

        //第几个房间群发
        public static void GroupSend(GameDataCenter gameDataCenter, int roomNum, string str)
        {
            foreach (var item in gameDataCenter.RoomSeatSockets[roomNum])
            {
                NetCtrl.Send(item, str);
            }
        }
    }
}
