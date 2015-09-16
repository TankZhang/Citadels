using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitadelsServer.Heros;
namespace CitadelsServer.Buildings
{
    public  class MilitaryBuilding:Building
    {

        public MilitaryBuilding()
        { }

        public MilitaryBuilding(int id,int price,string name)
            : base(id, price,name, FunctionType.warlord)
        { }
    }
}
