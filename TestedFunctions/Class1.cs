using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestedFunctions
{
    public class Class1
    {
        public static int RegularSum(int[] array)
        {
            int sum = 0;
            foreach (int a in array)
            {
                sum += a;
            }
            return sum;
        }
    }
}
