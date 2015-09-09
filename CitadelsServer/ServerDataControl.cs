using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CitadelsServer
{
    public static class ServerDataControl
    {
        public static string[] Ips2Strings(IPAddress[] ips)
        {
            string[] strs= new string[ips.Count()];
            for (int i = 0; i < ips.Count(); i++)
            {
                strs[i] = ips[i].ToString();
            }
            return strs;
        }
    }
}
