//Параллельное выполнение

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
        //В данном случае ф-я сортировки части
        static void qsort(int[] arr_q, int left, int right)
        {
            int l = left;
            int r = right;
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

        //Класс-задание
        public class InputData
        {
            public string str;
            public int[] array;
            public int start = 0;   //нач.диап
            public int stop = 0;    //кон.диап.
        }

        //Функция-нить исполнения
        //Принимает в виде аргумента контейнер с заданием, 
        //сортирует часть массива
        //Посылает сообщение в порт
        static void thread_fun(InputData d, Port<int> resp)
        {
            qsort(d.array, d.start, d.stop);
            Console.WriteLine(d.str);
            resp.Post(1);
        }

        
        //параллельная сортирока с нитями исполнения
        static void paral_qsort(int[] arr_q, int left, int right)
        {
            int l = left;
            int r = right;
            int temp = 0;
            int middle = arr_q[(l + r) / 2];

            while (l <= r)
            {
                while ((arr_q[l] < middle) && (l <= right))
                {
                    l++;
                }
                while ((arr_q[r] > middle) && (r >= left))
                {
                    r--;
                }

                if (l <= r)
                {
                    temp = arr_q[l];
                    arr_q[l] = arr_q[r];
                    arr_q[r] = temp;
                    l++;
                    r--;
                }
            }

            InputData data1 = new InputData();
            InputData data2 = new InputData();

            if (r > left)
            {
                data1.str="Нить исполнения 1";

                data1.array = arr_q;
                data1.start = left;
                data1.stop = r;
                //вместо qsort(arr_q, left, r);
            }
            if (l < right)
            {
                data2.str = "Нить исполнения 2";

                data2.array = arr_q;
                data2.start = l;
                data2.stop = right;
                //вместо qsort(arr_q, l, right);
            }

            //Создаём диспетчеры с пулом из 2 потоков
            Dispatcher d = new Dispatcher(2, " Test Pool");
            DispatcherQueue dq = new DispatcherQueue(" Test Queue", d);

            //Описываем (определяем) порт, в который каждый экземпляр метода отправляет сообщение после завершения вычислений
            Port<int> p = new Port<int>();

            //Метод Arbiter.Activate помещает в очередь диспетчера две задачи (два экземпляра метода)
            //Первый параметр метода Arbiter.Activate – очередь диспетчера,
            //который будет управлять выполнением задачи, второй параметр – запускаемая задача.
            Arbiter.Activate(dq, new Task<InputData, Port<int>>(data1, p, thread_fun));
            Arbiter.Activate(dq, new Task<InputData, Port<int>>(data2, p, thread_fun));

            return;
        }

        

        //Главная функция
        static void Main(string[] args)
        {
            //Объявление массива целых чисел
            int[] arr;
            //Число элементов
            int N = 500000;

            //Выделение памяти под массив
            arr = new int[N];
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
                arr[i] = rand.Next(90) + 10;
                w.Write(arr[i]); w.Write(" "); n++;
                if (n == 10)
                {
                    w.WriteLine(); n = 0;
                }
            }
            w.Close();

            //Парралельная сортировка
            //Объявление таймера выполнения сортировки
            Stopwatch sWatch = new Stopwatch();
            //Запуск таймера сортировки
            sWatch.Start();
            paral_qsort(arr, 0, N - 1);
            sWatch.Stop();

            Console.ReadKey();
            //Вывод времени работы сортировки на экран
            Console.WriteLine("Время работы сортировки: " + sWatch.ElapsedMilliseconds);

            file = new FileInfo("sort_array.txt");
            w = file.CreateText();
            //Вывод отсортированного массива в файл
            n = 0;
            for (int i = 0; i < N; i++)
            {
                w.Write(arr[i]); w.Write(" "); n++;
                if (n == 10)
                {
                    w.WriteLine(); n = 0;
                }
            }
            w.Close();
            //Указание окончания выполнения программы
            Console.WriteLine("Завершение работы программы");
            Console.ReadKey();
        }
    }
}