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
        static void QuickSortLeftPivot(int[] a)
        {
            QuickSortLeftPivotRec(a, 0, a.Length - 1);
        }
        static void QuickSortLeftPivotRec(int[] a, int start, int end)
        {
            if (start >= end)
            {
                return;
            }

            int num = a[start];

            int i = start, j = end;

            while (i < j)
            {
                while (i < j && a[j] > num)
                {
                    j--;
                }

                a[i] = a[j];

                while (i < j && a[i] < num)
                {
                    i++;
                }

                a[j] = a[i];
            }

            a[i] = num;
            QuickSortLeftPivotRec(a, start, i - 1);
            QuickSortLeftPivotRec(a, i + 1, end);
        }
    }
}
