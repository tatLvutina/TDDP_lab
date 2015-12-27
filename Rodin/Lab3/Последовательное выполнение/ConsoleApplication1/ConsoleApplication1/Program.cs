using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Ccr.Core;
using System.Diagnostics;

namespace ConsoleApplication1
{
    class Program
    {
        static void qsort(int []a, int left, int right)
        {
            int l 	= left;
            int r 	= right;
            int val = 0;
            int mid = a[(l + r) / 2];

            while (l <= r)
            {
                while ((a[l] < mid) && (l <= right))
                {
                    l++;
                }
                while ((a[r] > mid) && (r >= left))
                {
                    r--;
                }

                if (l <= r)
                {
                    val = a[l];
                    a[l] = a[r];
                    a[r] = val;
                    l++;
                    r--;
                }
            }

            if (r > left)
            {
                qsort(a, left, r);
            }
            if (l < right)
            {
                qsort(a, l, right);
            }

            return;
        }

        static void Main(string[] args)
        {
            //Объявление массива
            int []arr;
            int N = 10;
            //Выделение памяти под массив
            arr = new int[N];
            //Инициализация массива случайными 
            Random r = new Random();
            for (int i = 0; i < N; i++)
            {
                arr[i] = r.Next(90) + 10;
                Console.Write(arr[i]+" ");
            }

            //Сортировка
            qsort(arr, 0, N);

            Console.WriteLine();
            Console.WriteLine("============================================");
            for (int i = 0; i < N; i++)
                Console.Write(arr[i]+" ");
        }
    }
}
