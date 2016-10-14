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
        public static int Caller(int[] a)
        {
            int sum = 0;
            for(int i=0; i<a.Length; i++)
            {
                sum = Callee(a, i, sum);
            }
            return sum;
        }

        private static int Callee(int[] a, int i, int sum)
        {
            return (sum + a[i]);
        }

        public static int Inlined(int[] a)
        {
            int sum = 0;
            for (int i = 0; i < a.Length; i++)
            {
                sum = (sum + a[i]);
            }
            return sum;
        }
    }
}
