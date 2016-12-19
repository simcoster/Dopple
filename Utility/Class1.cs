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
