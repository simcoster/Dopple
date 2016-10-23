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
        public static void insertionSort(int[] array)
        {
            int i = 0;
            int nextMin = int.MaxValue;
            int nextMinIndex = -1;
            if (nextMin < array[i])
            {
                int temp = array[i];
                array[i] = nextMin;
                array[nextMinIndex] = temp;
            }
        }

        public static void insertionSortWithHelpers(int[] array)
        {

            int i = 0;
            int min = int.MaxValue;
            int minIndex = -1;
            if (min < array[i])
            {
                swapTwo(array, i, minIndex);
            }
        }

        public static void swapTwo(int[] array, int i, int j)
        {
            int temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
    }
}
