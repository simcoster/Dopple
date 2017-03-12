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
            return Enumerable.Sum(array);
        }
        public static int RegularSum(int[] array)
        {
            int sum = 0;
            foreach(int a in array)
            {
                sum += a;
            }
            return sum;
        }
    }

    public class Helper
    {
        public int Number;
    }
}