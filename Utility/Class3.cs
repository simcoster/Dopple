using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Utility
{
    public class Class3
    {
        public void SimpleTwoLoops()
        {

            int j = 0;
            if (j> 5)
            {
                Console.Write("blah");
            } 
        }
        public int SumArray(int[] array)
        {
            int sum = 0;
            for(int i=0; i <array.Length; i++)
            {
                sum += array[i];
            }

            return sum;
        }

        public static int SumArrayInStages(int[] array)
        {
            int sumEven = 0;
            for (int i = 0; i < array.Length; i +=2)
            {
                sumEven += array[i];
            }

            int sumOdd = 0;
            for (int i = 1; i < array.Length; i += 2)
            {
                sumOdd += array[i];
            }

            return sumEven + sumOdd;

        }
    }
}
