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
        public static string ForLoop()
        {
            int returnInt = 0;
            int[] meep = { 4, 5, 6, 4, 4 };
            int a = 0;
            for (int i=0; i<5; i++)
            {
                returnString += meep[i];
                a++;
            }
            return returnString;
        }

        public static int ForeachLoop()
        {
            int returnInt = 0;
            foreach (var blah in new []{ 4,5,6,4,4})
            {
                returnInt += blah;
            }
            return returnInt;
        }


        public static int ForeachLoopDifferent()
        {
            int returnInt = 0;
            foreach (var blah in new[] { 4, 5, 6, 4, 4 })
            {
                returnInt -= blah;
            }
            return returnInt;
        }

        public static int LinqStatement()
        {
            return (new[] { 2, 4, 5, 6, 4 }).Aggregate((x,y) => x + y);
        }
    }
}
