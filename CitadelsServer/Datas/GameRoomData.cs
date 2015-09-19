using CitadelsServer.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitadelsServer.Heros;
namespace CitadelsServer.Datas
{
   public class GameRoomData
    {
        private List<Building> _tableBuildings;
        public List<Building> TableBuildings
        {
            get
            {
                return _tableBuildings;
            }

            set
            {
                _tableBuildings = value;
            }
        }

        private List<Building> _backBuildings;
        public List<Building> BackBuildings
        {
            get
            {
                return _backBuildings;
            }

            set
            {
                _backBuildings = value;
            }
        }
        
        private List<Hero> _tableHeros;
        public List<Hero> TableHeros
        {
            get
            {
                return _tableHeros;
            }

            set
            {
                _tableHeros = value;
            }
        }

        private List<Hero> _backHeros;
        public  List<Hero> BackHeros
        {
            get
            {
                return _backHeros;
            }

            set
            {
                _backHeros = value;
            }
        }
        
        Dictionary<int, int> _heroToPlayer;
        public Dictionary<int, int> HeroToPlayer
        {
            get
            {
                return _heroToPlayer;
            }

            set
            {
                _heroToPlayer = value;
            }
        }

        Dictionary<int,PlayerData> _playerDataDic;
        public Dictionary<int,PlayerData> PlayerDataDic
        {
            get
            {
                return _playerDataDic;
            }

            set
            {
                _playerDataDic = value;
            }
        }

        public GameRoomData()
        {
            CardRes cardRes = new CardRes();
            BackBuildings = cardRes.Buildings;
            TableBuildings = new List<Building>();
            BackHeros = cardRes.Heros;
            TableHeros = new List<Hero>();
            HeroToPlayer = new Dictionary<int, int>();
            PlayerDataDic = new Dictionary<int, PlayerData>();
        }
    }
}
