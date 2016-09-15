using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Ccr.Core;
using System.Diagnostics;
using System.IO;

namespace ConsoleApplication1
{
    class Program
    {
        //Метод быстрой сортировки части массива
        static void qsort(int[] a, int left, int right)
        {
            int l = left;
            int r = right;
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

        //Класс-задание
        public class InputData
        {
            public string str;
            public int[] array; 
            public int start = 0;
            public int stop = 0;
        }

        //Функция-нить исполнения
        //Принимает в виде аргумента контейнер с заданием, 
        //сортирует часть массива
        //Посылает сообщение в порт
        static void thread_fun(InputData d, Port<int> resp)
        {
            Stopwatch sWatch = new Stopwatch();
            sWatch.Start();
            qsort(d.array, d.start, d.stop);
            sWatch.Stop();
            Console.WriteLine(d.str+"Время работы: "+ sWatch.ElapsedMilliseconds);
            resp.Post(1);
        }

        //Метод параллельной сортировки массива
        static void parral_qsort(int[] a, int left, int right)
        {
            //Создаём диспетчеры
            Dispatcher d = new Dispatcher(2, " Test Pool");
            DispatcherQueue dq = new DispatcherQueue(" Test Queue", d);

            //Описываем порт
            Port<int> p = new Port<int>();

            //Первый этап параллельной сортировки
            //Параллельно сортируются две половины массива
            InputData data = new InputData();
            data.str = "Первая половина массива ";
            data.array = a; data.start = 0; data.stop = right/2 + 1;
            Arbiter.Activate(dq, new Task<InputData, Port<int>>(data, p, thread_fun));
            Console.WriteLine("Первый этап");
            Console.ReadKey();
            data.str = "Вторая половина массива ";
            data.array = a; data.start = right/2 + 1; data.stop = right;
            Arbiter.Activate(dq, new Task<InputData, Port<int>>(data, p, thread_fun));
            Console.ReadKey();

            //Второй этап параллельной сортировки
            //Сортируется центральная часть массива
            Console.WriteLine();
            Console.WriteLine("Второй этап");
            Stopwatch sWatch = new Stopwatch();
            sWatch.Start();
            qsort(a,right/4,right/4+right/2 +1);
            sWatch.Stop();
            Console.WriteLine("Средняя часть массива   Время работы: " + sWatch.ElapsedMilliseconds);
            Console.ReadKey();

            //Третий этап параллельной сортировки
            //Параллельно сортируются две половины массива
            data.str = "Первая половина массива ";
            data.array = a; data.start = 0; data.stop = right / 2 + 1;
            Arbiter.Activate(dq, new Task<InputData, Port<int>>(data, p, thread_fun));
            Console.WriteLine("Первый этап");
            Console.ReadKey();
            data.str = "Вторая половина массива ";
            data.array = a; data.start = right / 2 + 1; data.stop = right;
            Arbiter.Activate(dq, new Task<InputData, Port<int>>(data, p, thread_fun));
            Console.ReadKey();
            return;
        }

        //
        static void Main(string[] args)
        {
            //Объявление массива
            int[] arr;
            //Число элементов
            int N = 10000;

            //Выделение памяти под массив
            arr = new int[N];

            Console.WriteLine("Начало работы программы");
            //Файловый поток для вывода в файл исходного массива
            FileInfo f = new FileInfo("array.txt");
            StreamWriter w = f.CreateText();

            //Инициализация массива случайными 
            Random r = new Random();
            int n=0;
            for (int i = 0; i < N; i++)
            {
                arr[i] = r.Next(900)+100;
                //Вывод элемента массива в файл
                w.Write(arr[i]); w.Write(" "); n++;
                if (n == 10)
                {
                    w.WriteLine(); n = 0;
                }
            }
            w.Close();

            //Парралельная сортировка
            Stopwatch sWatch = new Stopwatch();
            sWatch.Start();
            parral_qsort(arr,0,N-1);
            sWatch.Stop();

            Console.WriteLine();
            Console.WriteLine("Время работы программы: " + sWatch.ElapsedMilliseconds);

            //Файловый поток для вывода в файл отсортированного массива
            FileInfo f1 = new FileInfo("mod_array.txt");
            StreamWriter w1 = f1.CreateText();
            n = 0;
            //Вывод в файл отсортированного массива
            for (int i = 0; i < N; i++)
            {
                w1.Write(arr[i]); w1.Write(" "); n++;
                if (n == 10)
                {
                    w1.WriteLine(); n = 0;
                }
            }
            w1.Close();
                
            Console.WriteLine();

            Console.WriteLine("Завершение работы программы");
            Console.ReadKey();
        }
    }
}