using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestedFunctions
{
    public class Class1
    {
        public static int SimpleAdd(int a, int b)
        {
            return a + b;
        }
        public static int SimpleForLoop(int a)
        {
            int sum = 0;
            for (int i = 0; i < a; i++)
            {
                sum += a;
            }
            return sum;
        }
        public int SimpleWhile(int a)
        {
            int b = 0;
            while (a > 7)
            {
                b++;
            }
            return b;
        }
        public int SimpleForeach(int[] array)
        {
            int sum = 0;
            foreach (int a in array)
            {
                sum += a;
            }
            return sum;
        }
        public static int ConpoundConditions(int a, int b)
        {
            int c = 0;
            int d = 0;
            while (a == b)
            {
                if (a > b)
                {
                    c = 3;
                    d = 5;
                }
                else
                {
                    c = 1;
                    d = 2;
                    if (b < a)
                    {
                        return d;
                    }
                }
            }
            return c + d;
        }
    }
}
