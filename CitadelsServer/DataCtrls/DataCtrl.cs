using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitadelsServer.DataCtrls
{
    public static class DataCtrl
    {

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
