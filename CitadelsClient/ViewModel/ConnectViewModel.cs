using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CitadelsClient.Model;
using System.Windows;
using CitadelsClient.View;

namespace CitadelsClient.ViewModel
{
    public class ConnectViewModel:NotificationObject
    {
        IPAddr _ip;
        public IPAddr Ip
        {
            get
            {
                return _ip;
            }

            set
            {
                _ip = value;
                RaisePropertyChanged("Ip");
            }
        }

        ICommand _btnConnectCmd;
        public ICommand BtnConnectCmd
        {
            get
            {
                return _btnConnectCmd;
            }

            set
            {
                _btnConnectCmd = value;
            }
        }

        bool _isEnable;
        public bool IsEnable
        {
            get
            {
                return _isEnable;
            }

            set
            {
                _isEnable = value;
                RaisePropertyChanged("IsEnable");
            }
        }

        public ConnectViewModel()
        {
            Ip = new IPAddr();
            Ip.Ip = "192.168.1.102";
            Ip.Port = "31313";
            BtnConnectCmd = new RelayCommand(new Action<object>(Connect));
            IsEnable = true;
        }


        public void Connect(object obj)
        {
            IPAddr ipTemp = obj as IPAddr;
            App.clientNetControl = new ClientNetControl(ipTemp.Ip, ipTemp.Port);
            App.clientNetControl.Send("hi");
            if(App.clientNetControl.SocketClient.Connected)
            {
                Login loginWindow = new Login();
                loginWindow.Show();
                IsEnable = false;
            }
            else
            {
                MessageBox.Show("连接失败");
            }
        }
    }
}
