using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitadelsServer
{
    public class ViewModel
    {
        public MySQLCtrl mySQLCtrl;
        public NetCtrl netCtrl;
        private ObservableCollection<ServerIp> _ips;
        public ObservableCollection<ServerIp> Ips
        {
            get
            {
                return _ips;
            }

            set
            {
                _ips = value;
            }
        }
        public ViewModel()
        {
            netCtrl = new NetCtrl();
            mySQLCtrl = new MySQLCtrl();
            Ips = new ObservableCollection<ServerIp>();
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
