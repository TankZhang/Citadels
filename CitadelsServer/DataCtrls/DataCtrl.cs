using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using CitadelsServer.Datas;
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
                        Console.WriteLine(roomToStart+"号房间可以开始了");
                    }
                    break;
                //处理游戏中的数据
                case "2":
                    break;
                default: Console.WriteLine("接收到错误信息"); break;
            }
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
        public static void GroupSend(GameDataCenter gameDataCenter,int roomNum,string str)
        {
            foreach (var item in gameDataCenter.RoomSeatSockets[roomNum])
            {
                NetCtrl.Send(item, str);
            }
        }
    }
}
