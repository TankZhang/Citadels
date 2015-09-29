using CitadelsClient.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CitadelsClient.ViewModel
{
    public  class LobbyVM:NotificationObject
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

        public ObservableCollection<RoomInfo> RoomInfoList { set; get; }
        
        bool _CanJoin;
        public bool CanJoin
        {
            get
            {
                return _CanJoin;
            }

            set
            {
                _CanJoin = value;
                RaisePropertyChanged("CanJoin");
            }
        }

        int _index;
        public int Index
        {
            get
            {
                return _index;
            }

            set
            {
                if (RoomInfoList[value].NickNameList.Count < 4) { CanJoin = true; } else { CanJoin = false; }
                _index = value;
                RaisePropertyChanged("Index");
            }
        }

        ICommand _btnJoin;
        public ICommand BtnJoin
        {
            get
            {
                return _btnJoin;
            }

            set
            {
                _btnJoin = value;
            }
        }

        ICommand _btnCreate;
        public ICommand BtnCreate
        {
            get
            {
                return _btnCreate;
            }

            set
            {
                _btnCreate = value;
            }
        }
        
        public void Join()
        {
            App.clientNetControl.Send("1|1|" + RoomInfoList[Index].Id+"|"+ UserInfo.Mail + "|");
        }

        public void Create()
        {
            App.clientNetControl.Send("1|0|" + UserInfo.Mail + "|");
        }

        Thread thReceive;

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
                    Console.WriteLine("Lobby收到了："+str);
                    DealReceive(str.Substring(2));
                }//try结束
                catch (Exception ex)
                {
                    //抛出异常信息
                    Console.WriteLine(ex.Message.ToString());
                }
            }
        }
        //处理lobby界面收到的值
        public void DealReceive(string s)
        {
            switch (s[0])
            {
                case '0':
                    break;
                case '1':
                    break;
                case '2':
                    break;
                case '3':
                    break;
                case '4':
                    RoomInfoList = new ObservableCollection<RoomInfo>();
                    if (s == "4|") { return; }
                    DealReceiveRoomInfo(s.Substring(2));
                    break;
                default:break;
            }
        }
        //处理收到的房间具体的信息
        private void DealReceiveRoomInfo(string s)
        {
            string[] ss = DataCtrl.SegmentData(s);
            for (int i = 0; i < ss.Length/3; i++)
            {
                RoomInfo r = new RoomInfo();
                r.Id = int.Parse(ss[3 * i]);
                r.Num = int.Parse(ss[3 * i + 1]);
                r.NickNameList.Add(ss[3 * i + 2]);
                RoomInfoList.Add(r);
            }
        }

        public LobbyVM()
        {
            CanJoin = true;
            RoomInfoList = new ObservableCollection<RoomInfo>();
            UserInfo = App.userInfo;
            thReceive = new Thread(ReceiveSocket);
            thReceive.IsBackground = true;
            thReceive.Start(App.clientNetControl.SocketClient);
            //首先请求更新信息
            App.clientNetControl.Send("1|2|0|");
            BtnJoin = new RelayCommand(new Action(Join));
            BtnCreate = new RelayCommand(new Action(Create));
        }

    }
}
