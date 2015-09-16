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
        public static void DealData(GameDataCenter gameDataCenter,Socket socket,string str)
        {
            switch (str[0])
            {
                //处理登陆注册信息
                case '0':
                    GameUser gameuser = InfoDataCtrl.InfoDataDeal(App.viewModel.MySQLCtrl, socket, str.Substring(1));
                    Console.WriteLine(gameuser.Status);
                    break;
                //处理房间座位信息
                case '1':
                    string roomStatus = InfoDataCtrl.RoomDataDeal(gameDataCenter, socket, str.Substring(1));
                    Console.WriteLine(roomStatus);
                    break;
                default: Console.WriteLine("接收到错误信息"); break;
            }
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

    }
}
