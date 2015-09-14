using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitadelsServer
{
    public class ViewModel: NotificationObject
    {
        public MySQLCtrl mySQLCtrl;
        public NetCtrl netCtrl;
        private List<ServerIp> _ips;
        public List<ServerIp> Ips
        {
            get
            {
                return _ips;
            }

            set
            {
                _ips = value;
                this.RaisePropertyChanged("Str");
            }
        }
        public ViewModel()
        {
            netCtrl = new NetCtrl();
            mySQLCtrl = new MySQLCtrl();
            Ips = new List<ServerIp>();
            foreach (var item in NetCtrl.GetLocalIP())
            {
                if (!item.ToString().Contains("%"))
                {
                    Ips.Add(new ServerIp(item));
                }
            }
        }
    }
}
