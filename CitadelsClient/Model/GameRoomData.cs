using CitadelsClient.Model.Buildings;
using CitadelsClient.Model.Heros;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitadelsClient.Model
{
   public  class GameRoomData
    {
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

        int _seatNum;
        public int SeatNum
        {
            get
            {
                return _seatNum;
            }

            set
            {
                _seatNum = value;
            }
        }
        
        UserInfo _userInfoData;
        public UserInfo UserInfoData
        {
            get
            {
                return _userInfoData;
            }

            set
            {
                _userInfoData = value;
            }
        }

        public List<Building> TableBuildings;
        public List<Hero> TableHeros;


    }
}
