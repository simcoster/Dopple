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
        public static int TwoVarsPath(int a, int b)
        {
            int counter = 0;
            int sum = 0;
            for (int i = 0; i < 5; i++)
            {
                counter++;
                for (int j = 0; j < 5; j++)
                {
                    sum += counter;
                }
            }
            return sum;
        }

        public static int TwoVarsPathTwo(int a, int b)
        {
            int counter = 0;
            int sum = 0;
            for (int j = 0; j < 5; j++)
            {
                sum += counter;
                for (int i = 0; i < 5; i++)
                {
                    counter++;
                }
            }
            return sum;
        }
        public static int ThreeVarsPath(int a, int b)
        {
            int sum = 1;
            for (int i = 1; i < 5; i++)
            {
                for (int j = 1; j < 6; j++)
                {
                    sum *= i - j;
                }
            }
            return sum;
        }
        public static int ThreeVarsPathTwo(int a, int b)
        {
            int sum = 1;
            for (int j = 1; j < 6; j++)
            {
                for (int i = 1; i < 5; i++)
                {
                    sum *= i - j;
                }
            }
            return sum;
        }
    }
}
