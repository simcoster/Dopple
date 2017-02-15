using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Utility
{
    public class Class1
    {

        static public int TestRec(int[] arr, int left, int right)
        {
            var lefttemp = left + 5;
            var rightTemp = 4;
            TestRec(arr, lefttemp, rightTemp);
            TestRec(new int[4], lefttemp, rightTemp);
            if (left >5)
            {
                return left;
            }
            return right;
        }

        public static int SumInSingleStep(int a, int b, int c)
        {
            return a + b + c;
        }
    }
}