using CitadelsClient.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CitadelsClient.View;
using System.Threading;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Threading;

namespace CitadelsClient.ViewModel
{
    public  class LoginViewModel: NotificationObject
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

        ICommand _btnLoginCmd;
        public ICommand BtnLoginCmd
        {
            get
            {
                return _btnLoginCmd;
            }

            set
            {
                _btnLoginCmd = value;
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

        Thread thReceive;
        //构造函数
        public LoginViewModel()
        {
            IsEnable = true;
            UserInfo = new UserInfo();
            BtnLoginCmd = new RelayCommand(new Action<object>(Login));
            BtnSignCmd = new RelayCommand(new Action<object>(Sign));
        }
        //点击登陆，进行登陆
        public void Login(object obj)
        {
            thReceive = new Thread(ReceiveSocket);
            thReceive.IsBackground = true;
            thReceive.Start(App.clientNetControl.SocketClient);
            UserInfo userInfo = obj as UserInfo;
            App.clientNetControl.Send("0|1|"+userInfo.Mail+"|"+userInfo.Pwd+"|");
        }
        //点击注册，进入注册界面
        public void Sign(object obj)
        {
            string mailTemp = obj as string;
            Sign signWindow = new Sign();
            signWindow.ShowDialog();
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
                    //如果收到登陆成功的消息，关闭本窗口，并return该线程
                    if (str == "0|1|1|")
                    {
                        App.userInfo = new UserInfo();
                        App.userInfo = UserInfo;
                        IsEnable = false;
                        return;
                    }
                    MessageBox.Show(str.Substring(6));
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
