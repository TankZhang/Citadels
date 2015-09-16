using CitadelsServer.Buildings;
using CitadelsServer.Heros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitadelsServer.Datas
{
  public   class PlayerData
    {
        private int _seat;
        public int Seat
        {
            get
            {
                return _seat;
            }

            set
            {
                _seat = value;
            }
        }

        private bool _isKill;
        public bool IsKill
        {
            get
            {
                return _isKill;
            }

            set
            {
                _isKill = value;
            }
        }
        
        private bool _isStole;
        public bool IsStole
        {
            get
            {
                return _isStole;
            }

            set
            {
                _isStole = value;
            }
        }

        private bool _isKing;
        public bool IsKing
        {
            get
            {
                return _isKing;
            }

            set
            {
                _isKing = value;
            }
        }

        private bool _isFirstFinished;
        public bool IsFirstFinished
        {
            get
            {
                return _isFirstFinished;
            }

            set
            {
                _isFirstFinished = value;
            }
        }

        private bool _isSecondFinished;
        public bool IsSecondFinished
        {
            get
            {
                return _isSecondFinished;
            }

            set
            {
                _isSecondFinished = value;
            }
        }

        int _money;
        public int Money
        {
            get
            {
                return _money;
            }

            set
            {
                _money = value;
            }
        }
        
        public int Score
        {
            get
            {
                int sco = 0;
                bool a = false;
                bool b= false;
                bool c = false;
                bool d = false;
                bool e = false;
                foreach (var item in TableBuilding)
                {
                    sco += item.Price;
                    switch(item.Type)
                    {
                        case FunctionType.commercial:a = true;break;
                        case FunctionType.magic:b = true;break;
                        case FunctionType.noble:c = true; break;
                        case FunctionType.religious: d = true;break;
                        case FunctionType.warlord:e = true;break;
                        default:break;
                    }
                }
                if(a&&b&&c&&d&&e)
                { sco += 3; }
                if (IsFirstFinished) { sco += 4; }
                if (IsSecondFinished) { sco += 2; }
                return sco;
            }
        }

        List<Building> _pocketBuildings;
        public List<Building> PocketBuildings
        {
            get
            {
                return _pocketBuildings;
            }

            set
            {
                _pocketBuildings = value;
            }
        }

        List<Building> _tableBuilding;
        public List<Building> TableBuilding
        {
            get
            {
                return _tableBuilding;
            }

            set
            {
                _tableBuilding = value;
            }
        }

        List<Hero> _role;
        public List<Hero> Role
        {
            get
            {
                return _role;
            }

            set
            {
                _role = value;
            }
        }


        public PlayerData(int seat)
        {
            Seat = seat;
            IsKill = false;
            IsKing = false;
            IsStole = false;
            IsFirstFinished = false;
            IsSecondFinished = false;
            PocketBuildings = new List<Building>();
            TableBuilding = new List<Building>();
            Role = new List<Hero>();
        }
    }
}
