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
                    array[i] = nextMin;
                    array[nextMinIndex] = temp;
                }
            }
        }

        public static void insertionSortWithHelpers(int[] array)
        {
            for (int i = 0; i < array.Length - 1; i++)
            {
                Tuple<int, int> nextMinAndIndex = findMinAndIndex(array, i);
                if (nextMinAndIndex.Item1 < array[i])
                {
                    swapTwo(array, i, nextMinAndIndex.Item2);
                }
            }
        }

        public static void swapTwo(int[] array, int i, int j)
        {
            int temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }

        public static void swapTwoTwo(int[] array)
        {
            for (int i=0; i<array.Length; i++)
            {
                int temp = array[i];
                array[i] = array[i+1];
                array[i+1] = temp;
            }
        }

        public static Tuple<int, int> findMinAndIndex(int[] array, int startIndex)
        {
            int currentMin = int.MaxValue;
            int currentMinIndex = -1;
            for(int i=startIndex; i<array.Length; i++)
            {
                if (array[i] < currentMin)
                {
                    currentMin = array[i];
                    currentMinIndex = i;
                }
            }
            return new Tuple<int, int>(currentMin, currentMinIndex);
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

        public static void QuicksortMiddle(int[] elements, int left, int right)
        {
            int i = left, j = right;
            int pivot = elements[(left + right) / 2];

            while (i <= j)
            {
                while (elements[i] <pivot )
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
                QuicksortMiddle(elements, left, j);
            }

            if (i < right)
            {
                QuicksortMiddle(elements, i, right);
            }
        }

        static void QuickSortLeftPivot(int[] a, int start, int end)
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
            QuickSortLeftPivot(a, start, i - 1);
            QuickSortLeftPivot(a, i + 1, end);
        }

        static void GetGreatest(int[] a, int start, int end)
        {
            int biggest = a[start];
            for (int i=biggest+1; i<=end; i++)
            {
                if (biggest < a[i])
                {
                    biggest = a[i];
                }
            }
        }

        static void InIf(int a)
        {
            int b =4;
            if (a >5)
            {
                a = -a;
                b = -b;
            }
        }

        static void OutIf(int a)
        {
            int b =4;
            if (a > 5)
            {
                b = -b;
            }
            a = -a;
        }
    }
}
