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
        public int plus3(int a, int b, int c)
        {
            return a + b + c;
        }
        public static void QuickSortMiddle(int[] arr)
        {
            QuicksortMiddleRec(arr, 0, arr.Length);
        }

        public static void QuicksortMiddleRec(int[] elements, int left, int right)
        {
            int i = left, j = right;
            int pivot = elements[(left + right) / 2];

            while (i <= j)
            {
                while (elements[i] < pivot)
                {
                    i++;
                }

                while (elements[j] > pivot)
                {
                    j--;
                }

                if (i <= j)
                {
                    // Swap
                    int tmp = elements[i];
                    elements[i] = elements[j];
                    elements[j] = tmp;

                    i++;
                    j--;
                }
            }

            // Recursive calls
            if (left < j)
            {
                QuicksortMiddleRec(elements, left, j);
            }

            if (i < right)
            {
                QuicksortMiddleRec(elements, i, right);
            }
        }
    }
}
