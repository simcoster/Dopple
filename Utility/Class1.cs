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
        static public void MergeSortRec(int[] numbers, int left, int right)
        {
            if (left > right)
            {
                Console.Write("");
                return;
            }
            else if (right==2)
            {
                Console.Beep();
                MergeSortRec(numbers, left, right * 5);
                Console.Clear();

            }
            else
            {
                MergeSortRec(numbers, left, right / 3);
                Console.SetWindowSize(1, 2);
            }
        }

        static public void MergeSort(int[] numbers)
        {
            MergeSortRec(numbers, numbers.Length, 0);
        }

    }
}
