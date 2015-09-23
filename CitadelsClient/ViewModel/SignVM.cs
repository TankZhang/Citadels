using CitadelsClient.Model;
using CitadelsClient.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CitadelsClient.ViewModel
{
   public  class SignVM:NotificationObject
    {

        UserInfo _userInfo;
        public UserInfo UserInfo
        {
            get
            {
                return _userInfo;
            }

            set
            {
                _userInfo = value;
                RaisePropertyChanged("UserInfo");
            }
        }
        
        ICommand _btnSignCmd;
        public ICommand BtnSignCmd
        {
            get
            {
                return _btnSignCmd;
            }

            set
            {
                _btnSignCmd = value;
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

        

        public void Sign(object obj)
        {
            UserInfo userInfoTemp = obj as UserInfo;
            App.clientNetControl.Send("0|0|" + userInfoTemp.Mail + "|" + userInfoTemp.NickName + "|" + userInfoTemp.Pwd + "|" + userInfoTemp.RealName + "|");
            Thread thReceive = new Thread(ReceiveSocket);
            thReceive.IsBackground = true;
            thReceive.Start(App.clientNetControl.SocketClient);
        }
        //构造函数
        public SignVM()
        {
            IsEnable = true;
            UserInfo = new UserInfo();
            BtnSignCmd = new RelayCommand(new Action<object>(Sign));
        }
        //对socket进行接收
        public void ReceiveSocket(object obj)
        {
            Socket s = obj as Socket;
            while (true)
            {
                byte[] buffer = new byte[1024 * 1024 * 5];
                try
                {
                    int r = s.Receive(buffer);
                    if (r == 0)
                    {
                        break;
                    }
                    string str = Encoding.UTF8.GetString(buffer, 0, r);
                    if(str=="0|0|1|")
                    {
                        MessageBox.Show("注册成功");
                        IsEnable = false;
                        return;
                    }
                    MessageBox.Show(str.Substring(6));
                    Console.WriteLine("注册界面:"+ str.Substring(6));
                    return;
                }//try结束
                catch (Exception ex)
                {
                    //抛出异常信息
                    Console.WriteLine(ex.Message.ToString());
                }
            }
        }
    }
}
