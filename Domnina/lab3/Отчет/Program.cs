using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Ccr.Core;
using System.Threading;

namespace ConsoleApplication3
{
    public class InputData
    {
        public int start; // начало диапазона 
        public int stop;  // конец диапазона 
        public int i;
    }
    class Program
    {
        static int[] A;
        static int[] B;
        static int n;
        static int nc;

        static void Mul(InputData data, Port<int> resp)
         {
             int min2 = A[0], max2 = A[0], i;
             System.Diagnostics.Stopwatch sWatch = new System.Diagnostics.Stopwatch();
             sWatch.Start();  // запуск секундомера
             for (i = data.start; i <= data.stop; i++)
             {                                
                     if (A[i] < min2)
                         min2 = A[i];   
                     if (A[i] > max2)
                         max2 = A[i];   
             }
             sWatch.Stop();  // отключение секундомера
             resp.Post(1);
                 Console.WriteLine(min2);
                 Console.WriteLine(max2);
                 Console.WriteLine("Поток № {0}: Параллельный алгоритм = {1} мс.", Thread.CurrentThread.ManagedThreadId, sWatch.ElapsedMilliseconds.ToString());
        }

        static void Main(string[] args)
        {
            int i, min,max;
            nc = 2;  // количество ядер
            n = 100000000; // количество элементов в массиве
            A = new int[n];
            B = new int[nc];

            Random r = new Random(); // генирируем массив
            for (int j = 0; j < n; j++)
                A[j] = r.Next(100);
            System.Diagnostics.Stopwatch sWatch = new System.Diagnostics.Stopwatch();
            sWatch.Start();  // запуск секундомера
            min = A[0];
            for (i = 1; i < n; i++)
            {
                if (A[i] < min)
                    min = A[i];
            }
            max = A[0];
            for (i = 1; i < n; i++)
            {
                if (A[i] > max)
                    max = A[i];
            }
            Console.WriteLine(min);
            Console.WriteLine(max);
            sWatch.Stop();  // отключение секундомера
            Console.WriteLine("Последовательный алгоритм = {0} мс.", sWatch.ElapsedMilliseconds.ToString());
                        
            // создание массива объектов для хранения параметров 
            InputData[] ClArr = new InputData[nc];
            for (i = 0; i < nc; i++)
                ClArr[i] = new InputData();
            // делим количество элементов  в массиве на nc частей
            int step = (Int32)(n / nc);  
            // заполняем массив параметров 
            int c = -1;
            for (i = 0; i < nc; i++)
            {
                ClArr[i].start = c + 1;
                ClArr[i].stop = c + step;
                ClArr[i].i = i;
                c = c + step;
            }
            Dispatcher d = new Dispatcher(nc, "Test Pool"); // диспетчер с nc потоками в пуле
            DispatcherQueue dq = new DispatcherQueue("Test Queue", d); 
            Port<int> p = new Port<int>();
            
            for (i = 0; i < nc; i++)
                Arbiter.Activate(dq, new Task<InputData, Port<int>>(ClArr[i], p, Mul));
            Arbiter.Activate(dq, Arbiter.MultipleItemReceive(true, p, nc, delegate(int[] array)
     {   }));}}}
