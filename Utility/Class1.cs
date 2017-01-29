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
        static void QuickSortRec(int[] numbers)
        {
            QuickSort_Recursive(numbers, 0, numbers.Length - 1);
        }

        static void QuickSortIt(int[] numbers)
        {
            QuickSort_Iterative(numbers, 0, numbers.Length - 1);
        }

        static public int Partition(int[] numbers, int left, int right)
        {
            int pivot = numbers[left];
            while (true)
            {
                while (numbers[left] < pivot)
                    left++;

                while (numbers[right] > pivot)
                    right--;

                if (left < right)
                {
                    int temp = numbers[right];
                    numbers[right] = numbers[left];
                    numbers[left] = temp;
                }
                else
                {
                    return right;
                }
            }
        }

        struct QuickPosInfo
        {
            public int left;
            public int right;
        };

        static public void QuickSort_Iterative(int[] numbers, int left, int right)
        {

            if (left >= right)
                return; // Invalid index range

            List<QuickPosInfo> list = new List<QuickPosInfo>();

            QuickPosInfo info;
            info.left = left;
            info.right = right;
            list.Insert(list.Count, info);

            while (true)
            {
                if (list.Count == 0)
                    break;

                left = list[0].left;
                right = list[0].right;
                list.RemoveAt(0);

                int pivot = Partition(numbers, left, right);

                if (pivot > 1)
                {
                    info.left = left;
                    info.right = pivot - 1;
                    list.Insert(list.Count, info);
                }

                if (pivot + 1 < right)
                {
                    info.left = pivot + 1;
                    info.right = right;
                    list.Insert(list.Count, info);
                }
            }
        }

        static public int PartitionForRec(int[] numbers, int left, int right)
        {
            int pivot = numbers[left];
            while (true)
            {
                while (numbers[left] < pivot)
                    left++;

                while (numbers[right] > pivot)
                    right--;

                if (left < right)
                {
                    int temp = numbers[right];
                    numbers[right] = numbers[left];
                    numbers[left] = temp;
                }
                else
                {
                    return right;
                }
            }
        }

        static public void QuickSort_Recursive(int[] arr, int left, int right)
        {
            // For Recusrion
            if (left < right)
            {
                int pivot = PartitionForRec(arr, left, right);

                if (pivot > 1)
                    QuickSort_Recursive(arr, left, pivot - 1);

                if (pivot + 1 < right)
                    QuickSort_Recursive(arr, pivot + 1, right);
            }
        }
    }
}
