using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CitadelsServer
{
    public class ServerViewModel: NotificationObject
    {

        public ServerMySQLControl serverMySQLControl;
        public ServerNetControl serverNetControl;
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

        public ServerViewModel()
        {
            this.serverNetControl = new ServerNetControl();
            this.serverMySQLControl = new ServerMySQLControl();
            this.Ips = new List<ServerIp>();
            foreach (var item in ServerNetControl.GetLocalIP())
            {
                if (!item.ToString().Contains("%"))
                {
                    Ips.Add(new ServerIp(item));
                }
            }
        }

    }
}
