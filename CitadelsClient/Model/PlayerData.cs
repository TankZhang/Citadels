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
   public  class PlayerData
    {
        int _pocketBuildingNum;
        public int PocketBuildingNum
        {
            get
            {
                return _pocketBuildingNum;
            }

            set
            {
                _pocketBuildingNum = value;
            }
        }
        
        int _score;
        public int Score
        {
            get
            {
                return _score;
            }

            set
            {
                _score = value;
            }
        }
        
        public List<Building> TableBuildings;
        
        public List<Hero> Roles;
        
    }
}
