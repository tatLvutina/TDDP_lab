using System;
using Microsoft.Ccr.Core;
using System.Threading;

namespace ConsoleApplication3
{
    public class InputData
    {
        public int start; // начало диапазона 
        public int stop;  // конец диапазона 
        //public int i;
    }

    class Program
    {
        static int[] a;
        static int[] b;
        static int[] mem; //для "честности" запомним сгнерированный массив случайных чисел, чтобы оба алгоритма сортировали одинаковые данные
        static int n;
        static int nc;

        static void Mul(InputData data, Port<int> resp)
         {
             int i, j;
             System.Diagnostics.Stopwatch sWatch = new System.Diagnostics.Stopwatch();
             sWatch.Start();


            for (i = data.start; i <= data.stop-1; i++)
             {
                 for (j = i+1; j <= data.stop; j++)
                 {
                      if (a[j] < a[i]) //сортировка пузырьком
                    {
                        var temp = a[i];
                        a[i] = a[j];
                        a[j] = temp;
                    }
                }
             }
            //внимательно присмотревшись к этой функции и к условиям, в которых она вызывается, можно сделать вывод
            //что она вернет массив, состоящий из двух сортированных половинок, но не сортированный целиком
            //так оно и есть, потому что параллельный пузырек - необычная задумка
            //в данной лабораторной работе эта проблема решается в делегате, описанном в приемнике, штатными средствами C#
             sWatch.Stop();
             resp.Post(1);
             Console.WriteLine("Поток № {0}: Параллельный алгоритм = {1} мс.", Thread.CurrentThread.ManagedThreadId, sWatch.ElapsedMilliseconds.ToString());
        }

        public static void arrDisplay() //функция вывода массива 
        {
            int i;
            Console.WriteLine("Показать текущее содержимое массива? (Y/N)\n");

            var press = Console.ReadKey(true).Key;

            if (press == ConsoleKey.Y)
            {
                Console.WriteLine("Параметры вывода:\n\tДля вывода массива вертикально с нумерацией нажмите V\n\tДля вывода массива подряд без нумерации горизантально нажмите H\n\tКомпактный режим: Для вывода первых и последних 25 элементов нажмите С\n");
                var pressnext = Console.ReadKey(true).Key;
                if (pressnext == ConsoleKey.V)
                {
                    for (i = 0; i < n; i++)
                        Console.WriteLine("{0}: {1}", i + 1, a[i].ToString());
                }
                else if (pressnext == ConsoleKey.H)
                {
                    for (i = 0; i < n; i++)
                        Console.Write("{0}\t", a[i].ToString());
                }
                else if (pressnext == ConsoleKey.C)
                {
                    for (i = 0; i < 25; i++)
                        Console.WriteLine("{0}: {1}", i + 1, a[i].ToString());
                    Console.WriteLine("...");
                    for (i = n - 25; i < n; i++)
                        Console.WriteLine("{0}: {1}", i + 1, a[i].ToString());
                }
                else
                {
                    Console.WriteLine("Некорректная клавиша. Вывод массива отклонен...\n");
                }
            }
            else if (press == ConsoleKey.N)
            {
                Console.WriteLine("Вывод массива отклонен...\n");
            }
            else
            {
                Console.WriteLine("Некорректная клавиша. Вывод массива отклонен...\n");
            }

        }
        static void Main(string[] args)
        {
            int i;
            nc = 2;
            n = 50000;

            Console.WriteLine("\nМассив включает в себя {0} элементов\n", n);

            a = new int[n];
            mem = new int[n];
            b = new int[nc];

            Random r = new Random();
            for (int j = 0; j < n; j++)
                a[j] = r.Next(10000);
            a.CopyTo(mem,0); //запомнили полученный массив
            Console.WriteLine("Исходный массив успешно заполнен случайными значениями!\n");

            arrDisplay();

            System.Diagnostics.Stopwatch sWatch = new System.Diagnostics.Stopwatch();
            Console.WriteLine("Начата последовательная сортировка массива...\n");
            sWatch.Start();
            for (i = 0; i <= n - 1; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    if (a[j] < a[i])
                    {
                        var temp = a[i];
                        a[i] = a[j];
                        a[j] = temp;
                    }
                }
            }
            sWatch.Stop();
            Console.WriteLine("Массив отсортирован последовательным алгоритмом!\n");
            Console.WriteLine("Последовательный алгоритм = {0} мс.", sWatch.ElapsedMilliseconds.ToString());

            arrDisplay(); //показали отсортированный массив

            mem.CopyTo(a, 0); ; // восстановили массив со случайными числами 

            // создание массива объектов для хранения параметров 
            InputData[] tempArray = new InputData[nc];
            i = 0;
            while (i < nc)
                {tempArray[i] = new InputData(); i++;}
            // делим количество элементов  в массиве на nc частей 
            int step = (Int32)(n / nc);
            // заполняем массив параметров 
            int c = -1;
            for (i = 0; i < nc; i++)
            {
                tempArray[i].start = c + 1;
                tempArray[i].stop = c + step;
                c = c + step;
            }
            Dispatcher d = new Dispatcher(nc, "Test Pool");
            DispatcherQueue dq = new DispatcherQueue("Test Queue", d);
            Port<int> p = new Port<int>();


            for (i = 0; i < nc; i++)
                Arbiter.Activate(dq, new Task<InputData, Port<int>>(tempArray[i], p, Mul));

            Console.WriteLine("Начата параллельная сортировка массива...\n");
            Arbiter.Activate(dq, Arbiter.MultipleItemReceive(true, p, nc, delegate(int[] array)
     {
         Console.WriteLine("Массив отсортирован параллельным алгоритмом!\n");
         System.Diagnostics.Stopwatch newWatch = new System.Diagnostics.Stopwatch();
         Console.WriteLine("Начата последовательная сортировка массива...\n");
         newWatch.Start();
         Array.Sort(a); //тот самый делегат в приемнике, с помощью которого шлейфуется результат параллельной сортировки пузырьком
         newWatch.Stop();
         Console.WriteLine("Окончательная сортировка средтвами C#: {0} мс.\n", newWatch.ElapsedMilliseconds.ToString());
         arrDisplay();
         Console.WriteLine("Вычисления завершены");
         Console.ReadKey(true);
         Environment.Exit(0);
     }));

        }
    }
}

