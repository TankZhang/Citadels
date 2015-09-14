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
    }
}
