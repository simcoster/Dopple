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
        //public static int SumMe(int[] array)
        //{
        //    return Enumerable.Sum(array,x => x ++);
        //}
        public static int RegularSum(Helper helper)
        {
            var a = 6;
            helper.Number = a;
            a = helper.Number;
            return a;
        }

        public static int Bigger(int a, int b)
        {
            if (a > b)
            {
                return a;
            }
            else
            {
                return b;
            }
        }
    }

    public class Helper
    {
        public int Number;
        public string Text;
        public string Text2;
    }
}