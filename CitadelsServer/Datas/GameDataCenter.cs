using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CitadelsServer.Datas
{
    public  class GameDataCenter
    { 
        //生成房间座位socket字典
        Dictionary<int, List<Socket>> _roomSeatSockets;
        public Dictionary<int, List<Socket>> RoomSeatSockets
        {
            get
            {
                return _roomSeatSockets;
            }

            set
            {
                _roomSeatSockets = value;
            }
        }

        //生成房间号
        int _roomNum;
        public int RoomNum
        {
            get
            {
                return _roomNum;
            }

            set
            {
                _roomNum = value;
            }
        }
        //生成房间号与游戏数据的对应
        Dictionary<int, GameRoomData> _roomDataDic;
        public Dictionary<int, GameRoomData> RoomDataDic
        {
            get
            {
                return _roomDataDic;
            }

            set
            {
                _roomDataDic = value;
            }
        }

        Dictionary<string, string> _mailNickDic;
        public Dictionary<string, string> MailNickDic
        {
            get
            {
                return _mailNickDic;
            }

            set
            {
                _mailNickDic = value;
            }
        }

        public GameDataCenter()
        {
            RoomNum = 0;
            RoomSeatSockets = new Dictionary<int, List<Socket>>();
            RoomDataDic = new Dictionary<int, GameRoomData>();
            MailNickDic = new Dictionary<string, string>();
        }
    }
}
