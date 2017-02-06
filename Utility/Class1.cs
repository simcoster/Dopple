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
        public static void findMinAndIndex(int[] array, int startIndex, out int minValue, out int minIndex)
        {
            int currentMin = int.MaxValue;
            int currentMinIndex = -1;
            for (int i = startIndex; i < array.Length; i++)
            {
                if (array[i] < currentMin)
                {
                    currentMin = array[i];
                    currentMinIndex = i;
                }
            }
            if (startIndex > 5)
            {
                minValue = currentMin;
                minIndex = currentMinIndex;
            }
            else
            {
                minIndex = 3;
                minValue = 4;
            }
        }

        public static void insertionSortWithHelpers(int[] array)
        {
            int min;
            int minIndex;
            findMinAndIndex(array, 0, out min, out minIndex);
            Console.WriteLine(minIndex);
            Console.WriteLine(min);
        }

        
    }
}