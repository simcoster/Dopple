using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Utility
{
    public class Class2
    {
        public static int SumInSingleStep(int a, int b, int c)
        {
            return a + b +c;
        }
        public static int SumInTwoStep(int a, int b, int c)
        {
            return new[] { a, b, c }.Sum();
        }
        public static void insertionSort(int[] array)
        {
            for (int i = 0; i < array.Length - 1; i++)
            {
                int nextMin = int.MaxValue;
                int nextMinIndex = -1;
                for (int j = i; j < array.Length; j++)
                {
                    if (array[j] < nextMin)
                    {
                        nextMin = array[j];
                        nextMinIndex = j;
                    }
                }
                if (nextMin < array[i])
                {
                    int temp = array[i];
                    array[i] = array[nextMinIndex];
                    array[nextMinIndex] = temp;
                }
            }
        }

   
        public int[] SelectionSort(int[] arr)
        {
            //1. Find min
            //2. Swap it with first element
            //3. Repeat starting from secong position onwards.
            int _min = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                _min = i;
                for (int j = i; j < arr.Length; j++)
                {
                    if (arr[j] < arr[_min])
                        _min = j;
                }
                int _temp = arr[i];
                arr[i] = arr[_min];
                arr[_min] = _temp;
            }
            return arr;
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

        public bool BinarySearchRec(int first, int last, int[] mynumbers, int target)
        {
            if (first == last)
            {
                return mynumbers[first] == target;
            }

            var mid = (first + last) / 2;

            if (target < mynumbers[mid])
            {
                return BinarySearchRec(mid + 1, last, mynumbers, target);
            }
            if (target > mynumbers[mid])
            {
                return BinarySearchRec(first, mid - 1, mynumbers, target);
            }
            else
            {
                return true;
            }
        }
    }
}
