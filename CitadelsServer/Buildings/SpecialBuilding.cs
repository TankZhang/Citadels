using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitadelsServer.Heros;
namespace CitadelsServer.Buildings
{
    public class SpecialBuilding:Building
    {
        public SpecialBuilding()
        { }

        public SpecialBuilding(int id, int price, string name)
            : base(id, price,name, FunctionType.magic)
        { }
    }
}
