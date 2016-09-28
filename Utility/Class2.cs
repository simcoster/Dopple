﻿using System;
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

        //static public void DoMerge(int[] numbers, int left, int mid, int right)
        //{
        //    int[] temp = new int[25];
        //    int i, left_end, num_elements, tmp_pos;

        //    left_end = (mid - 1);
        //    tmp_pos = left;
        //    num_elements = (right - left + 1);

        //    while ((left <= left_end) && (mid <= right))
        //    {
        //        if (numbers[left] <= numbers[mid])
        //            temp[tmp_pos++] = numbers[left++];
        //        else
        //            temp[tmp_pos++] = numbers[mid++];
        //    }

        //    while (left <= left_end)
        //        temp[tmp_pos++] = numbers[left++];

        //    while (mid <= right)
        //        temp[tmp_pos++] = numbers[mid++];

        //    for (i = 0; i < num_elements; i++)
        //    {
        //        numbers[right] = temp[right];
        //        right--;
        //    }
        //}

        //struct MergePosInfo
        //{
        //    public int left;
        //    public int mid;
        //    public int right;
        //};

        //static public void MergeSort_Iterative(int[] numbers, int left, int right)
        //{
        //    int mid;
        //    if (right <= left)
        //        return;

        //    List<MergePosInfo> list1 = new List<MergePosInfo>();
        //    List<MergePosInfo> list2 = new List<MergePosInfo>();

        //    MergePosInfo info;
        //    info.left = left;
        //    info.right = right;
        //    info.mid = -1;

        //    list1.Insert(list1.Count, info);

        //    while (true)
        //    {
        //        if (list1.Count == 0)
        //            break;

        //        left = list1[0].left;
        //        right = list1[0].right;
        //        list1.RemoveAt(0);
        //        mid = (right + left) / 2;

        //        if (left < right)
        //        {
        //            MergePosInfo info2;
        //            info2.left = left;
        //            info2.right = right;
        //            info2.mid = mid + 1;
        //            list2.Insert(list2.Count, info2);

        //            info.left = left;
        //            info.right = mid;
        //            list1.Insert(list1.Count, info);

        //            info.left = mid + 1;
        //            info.right = right;
        //            list1.Insert(list1.Count, info);
        //        }
        //    }


        //    for (int i = 0; i < list2.Count; i++)
        //    {
        //        DoMerge(numbers, list2[i].left, list2[2].mid, list2[2].right);
        //    }

        //}

        public static void QuicksortMiddle(IComparable[] elements, int left, int right)
        {
            int i = left, j = right;
            IComparable pivot = elements[(left + right) / 2];

            while (i <= j)
            {
                while (elements[i].CompareTo(pivot) < 0)
                {
                    i++;
                }

                while (elements[j].CompareTo(pivot) > 0)
                {
                    j--;
                }

                if (i <= j)
                {
                    // Swap
                    IComparable tmp = elements[i];
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

    }
}
