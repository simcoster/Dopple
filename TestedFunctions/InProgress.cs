using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestedFunctions
{
    public class InProgress
    {
        public static int SumMe(int[] array)
        {
            return Enumerable.Sum(array, x => x++);
        }
    }
}
