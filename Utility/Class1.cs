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
        public int SumSystemFunc(int[] array)
        {
            return array.Sum();
        }

        public int SumMyImlement(IEnumerable<int> array)
        {
            int sum = 0;
            foreach(int a in array)
            {
                sum += a;
            }
            return sum;
        }

    }
}