using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CitadelsServer
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

        public GameDataCenter()
        {
            RoomSeatSockets = new Dictionary<int, List<Socket>>();
        }
    }
}
