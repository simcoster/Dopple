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
        static public void DoMerge(int[] numbers, int left, int mid, int right)
        {
            int[] temp = new int[25];
            int i, left_end, num_elements, tmp_pos;

            left_end = (mid - 1);
            tmp_pos = left;
            num_elements = (right - left + 1);

            while ((left <= left_end) && (mid <= right))
            {
                if (numbers[left] <= numbers[mid])
                {
                    temp[tmp_pos] = numbers[left];
                    tmp_pos++;
                    left++;
                }
                else
                {
                    temp[tmp_pos] = numbers[mid];
                    tmp_pos++;
                    mid++;
                }
            }

            while (left <= left_end)
                temp[tmp_pos++] = numbers[left++];

            while (mid <= right)
                temp[tmp_pos++] = numbers[mid++];

            for (i = 0; i < num_elements; i++)
            {
                numbers[right] = temp[right];
                right--;
            }
        }


        static public void MergeSortRec(int[] numbers, int left, int right)
        {
            int mid;

            if (right > left)
            {
                mid = (right + left) / 2;
                MergeSortRec(numbers, left, mid);

                DoMerge(numbers, left, (mid + 1), right);
            }
        }

        static public void MergeSort(int[] numbers)
        {
            MergeSortRec(numbers, numbers.Length, 0);
        }

    }
}
