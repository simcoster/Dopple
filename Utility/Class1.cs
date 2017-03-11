using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Utility
{
    public abstract class Class1
    {
        //public static int SumMe(Helper[] array)
        public static int SumMe(int[] array)
        {
            return array.Sum(x => x);
        }
    }

    public class Helper
    {
        public int Number;
    }
}