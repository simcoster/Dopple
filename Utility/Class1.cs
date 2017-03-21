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
        public static void RegularSum(Helper helper)
        {
            helper.Text = "blah";
            helper.Text2 = helper.Text;
            helper.Text2 = "beep";
            int a = 6;
            a = a * 5;
            helper.Number = a;
        }
    }

    public class Helper
    {
        public int Number;
        public string Text;
        public string Text2;
    }
}