using CitadelsClient.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace CitadelsClient.ViewModel
{
    public class LobbyVM : NotificationObject
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

        private ObservableCollection<RoomInfo> _roomInfoList;
        public ObservableCollection<RoomInfo> RoomInfoList
        {
            get
            {
                return _roomInfoList;
            }

            set
            {
                _roomInfoList = value;
                RaisePropertyChanged("RoomInfoList");
            }
        }

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
                try
                {
                    if (RoomInfoList[value].NickNameList.Count < 4) { CanJoin = true; } else { CanJoin = false; }
                }
                catch { }
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
            App.clientNetControl.Send("1|1|" + RoomInfoList[Index].Id + "|" + UserInfo.Mail + "|");
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
                    Application.Current.Dispatcher.Invoke(del, str);
                    Console.WriteLine("Lobby收到了：" + str);
                }//try结束
                catch (Exception ex)
                {
                    //抛出异常信息
                    Console.WriteLine(ex.Message.ToString());
                }
            }
        }

        //预处理，防止收到两个连续的数据包
        void DealReceivePre(string s)
        {
            string[] ss = DataCtrl.SegmentDataStar(s);
            foreach (var item in ss)
            {
                DealReceive(item);
            }
        }

        //处理lobby界面收到的值
        public void DealReceive(string s)
        {
            string[] receStrs = DataCtrl.SegmentData(s);
            if (receStrs[0] != "1") { return; }
            //此处0为创建确认，1为加入确认，2为可以开始游戏，3为游戏正式开始，4为房间信息
            switch (receStrs[1])
            {
                case "0":
                    break;
                case "1":
                    break;
                case "2":
                    break;
                case "3":
                    break;
                case "4":
                    DealReceiveRoomInfo(receStrs);
                    break;
                default: break;
            }
        }

        //处理收到的房间具体的信息
        private void DealReceiveRoomInfo(string[] s)
        {
            switch (s[2])
            {
                //错误
                case "-1": return;
                //粗略信息
                case "0":
                    DealRoughRoomData(s); break;
                //详细信息
                case "1":
                    DealDetailRoomData(s); break;
                default:
                    return;
            }

        }

        //处理详细的房间信息
        private void DealDetailRoomData(string[] s)
        {
            foreach (var item in RoomInfoList)
            {
                if (item.Id == (int.Parse(s[3])))
                {
                    item.NickNameList = new ObservableCollection<string>();
                    for (int i = 0; i < (s.Length / 2 - 2); i++)
                    {
                        for (int j = 0; j < (s.Length / 2 - 2); j++)
                        {
                            if (int.Parse(s[4 + j * 2]) == (i + 1))
                            {
                                item.NickNameList.Add(s[4 + j * 2 + 1]);
                            }
                        }
                    }
                    item.Num = 0;
                }
            }
        }

        //处理粗略的房间信息
        private void DealRoughRoomData(string[] s)
        {
            RoomInfoList = new ObservableCollection<RoomInfo>();
            for (int i = 1; i < s.Length / 3; i++)
            {
                RoomInfo r = new RoomInfo();
                r.Id = int.Parse(s[3 * i]);
                r.Num = int.Parse(s[3 * i + 1]);
                r.NickNameList.Add(s[3 * i + 2]);
                RoomInfoList.Add(r);
            }
        }

        public delegate void Del(string a);
        Del del;

        ICommand _btnTest;
        public ICommand BtnTest
        {
            get
            {
                return _btnTest;
            }

            set
            {
                _btnTest = value;
            }
        }
        public void Test()
        {
            RoomInfo r2 = new RoomInfo();
            r2.Id = 1;
            r2.NickNameList.Add("r2s1");
            r2.Num = 1;
            r2.NickNameList.Add("r2s2");
            r2.Num = 1;
            RoomInfoList.Add(r2);
            RoomInfoList[0].NickNameList.Add("ha");
            RoomInfoList[0].Num = 0;

        }

        public LobbyVM()
        {
            del = new Del(DealReceivePre);
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
            BtnTest = new RelayCommand(new Action(Test));
        }
    }
}
