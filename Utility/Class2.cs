using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Utility
{
    public class Class2
    {
        public static void ForLoop()
        {
            int[] meep = new[] { 4, 5, 6, 4, 4 };
            for (int i=0; i<5; i++)
            {
                Console.WriteLine(meep[0]);
            }
        }

        public static void ForeachLoop()
        {
            foreach(var blah in new []{ 4,5,6,4,4})
            {
                Console.WriteLine(blah);
            }
        }

        public static void LinqStatement()
        {
            (new[] { 2, 4, 5, 6, 4 }).Select(x => x +2);
        }
    }
}
