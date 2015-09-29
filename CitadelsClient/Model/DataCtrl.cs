using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CitadelsClient.Model
{
    public static class DataCtrl
    {
       public  static string Encryption(string s)
        {
            MD5 md5 = MD5.Create();
            byte[] buffer = Encoding.Default.GetBytes(s);
            byte[] MD5Buffer = md5.ComputeHash(buffer);
            string st = "";
            for (int i = 0; i < MD5Buffer.Length; i++)
            {
                st += MD5Buffer[i].ToString("x2");
            }
            return st;
        }
        public static string[] SegmentData(string str)
        {
            string[] sss = str.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            return sss;
        }
    }
}
