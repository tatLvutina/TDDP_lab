//Последовательное выполнение

//Используемые директивы
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Ccr.Core;
using System.Diagnostics;
using System.IO;

//Консольное приложение
namespace ConsoleApplication1
{
    class Program
    {
        //Алгоритм быстрой сортировки
        //Значения left and right - диапазон сортируемого массива
        static void qsort(int []arr_q, int left, int right)
        {
            int l 	= left;
            int r 	= right;
            int temp = 0;
            //Установка опорного значения
            int middle = arr_q[(l + r) / 2];
            
            //Разделение массива на 3 части
            while (l <= r)
            {
                //Все элементы меньше опорного
                while ((arr_q[l] < middle) && (l <= right))
                {
                    l++;
                }
                //Все элементы больше опорного
                while ((arr_q[r] > middle) && (r >= left))
                {
                    r--;
                }
                //Все элементы равные опорному
                if (l <= r)
                {
                    temp = arr_q[l];
                    arr_q[l] = arr_q[r];
                    arr_q[r] = temp;
                    l++;
                    r--;
                }
            }
            //Сортировка левой части
            if (r > left)
            {
                qsort(arr_q, left, r);
            }
            //Сортировка правой части
            if (l < right)
            {
                qsort(arr_q, l, right);
            }

            return;
        }

        //Главная функция
        static void Main(string[] args)
        {
            //Объявление массива целых чисел
            int []my_arr;
            //Число элементов
            int N = 500000;
            //Выделение памяти под массив
            my_arr = new int[N];
            //Вывод состояния начала работы программы
            Console.WriteLine("Начало работы программы");
            Console.WriteLine("N = " + N);
            //Файловый поток для вывода в файл исходного массива
            FileInfo file = new FileInfo("my_array.txt");
            StreamWriter w = file.CreateText();

            //Инициализация массива случайными числами 
            Random rand = new Random();
            int n = 0;
            for (int i = 0; i < N; i++)
            {
                my_arr[i] = rand.Next(9000) + 1000;
                w.Write(my_arr[i]); w.Write(" "); n++;
                if (n == 10)
                {
                    w.WriteLine(); n = 0;
                }
            }
            w.Close();

            //Объявление таймера выполнения сортировки
            Stopwatch sWatch = new Stopwatch();

            //Запуск таймера сортировки
            sWatch.Start();

            //Сортировка участка массива (в этом случае весь массив)
            Console.WriteLine("Выполняется сортировка массива");
            qsort(my_arr, 0, N-1);

            //Остановка таймера
            sWatch.Stop();

            file = new FileInfo("sort_array.txt");
            w = file.CreateText();

            //Вывод отсортированного массива в файл
            n = 0;
            for (int i = 0; i < N; i++)
            {
                w.Write(my_arr[i]); w.Write(" "); n++;
                if (n == 10)
                {
                    w.WriteLine(); n = 0;
                }
            }
            w.Close();

            //Вывод времени работы сортировки на экран
            Console.WriteLine("Время работы сортировки = " + sWatch.ElapsedMilliseconds);
            //Указание окончания выполнения программы
            Console.WriteLine("Программа завершена");
            Console.ReadKey();
        }
    }
}
