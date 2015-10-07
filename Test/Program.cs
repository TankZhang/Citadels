using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> sss = new List<string>();
            sss.Insert(3, "3");
            foreach (var item in sss)
            {
                Console.WriteLine(item);
            }
            Console.ReadKey();
        }
    }
}
