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
            string returnString = "";
            int[] meep = { 4, 5, 6, 4, 4 };
            for (int i=0; i<5; i++)
            {
                returnString += meep[i];
            }
            return returnString;
        }

        public static string ForeachLoop()
        {
            string returnString = "";
            foreach (var blah in new []{ 4,5,6,4,4})
            {
                returnString += blah;
            }
            return returnString;
        }

        public static string LinqStatement()
        {
            return (new[] { 2, 4, 5, 6, 4 }).Select(x => x.ToString()).Aggregate((x,y) => x + y);
        }
    }
}
