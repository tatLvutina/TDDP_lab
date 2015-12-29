using System;
using Microsoft.Ccr.Core;
using System.Threading;

namespace ConsoleApplication1
{
    public class InputData //используется для описания задания для каждого метода
    {
        public int start; // начало диапазона 
        public int stop;  // конец диапазона 
        public int i;
    }

    class Program
    {
        static int[] Arr;
        static int n;
        static int nc;
        static void Sum(InputData data, Port<int> resp) //параллельный алгоритм
        {
            int Result = 0;
            int i = 0;
            System.Diagnostics.Stopwatch sWatch = new System.Diagnostics.Stopwatch();//для отсчитывания времени
            Random r = new Random();
            for (int j = 0; j < n; j++)
                Arr[j] = r.Next(5);
            sWatch.Start();
            
                for (i = data.start; i <= data.stop; i++)
                {
                    Result = Result + Arr[i]; //сумма массива
                }
        
             resp.Post(1);//результат отпраляется на порт

            Console.WriteLine("Поток № {0}: Параллельный алгоритм = {1} мс.", Thread.CurrentThread.ManagedThreadId, sWatch.ElapsedMilliseconds.ToString());
            sWatch.Stop();
            Console.WriteLine("Сумма:");
            Console.WriteLine(Result);
        }
        
        
            static void Main(string[] args)
        {
            int i;
            nc = 4;// количество ядер
            n = 100000000;

            Arr = new int[n];
            int Result = 0;

            Random r = new Random();
            for (int j = 0; j < n; j++)
                Arr[j] = r.Next(5);

            System.Diagnostics.Stopwatch sWatch = new System.Diagnostics.Stopwatch();
            sWatch.Start();
            for (i = 9; i < n; i++)//последовательный алгоритм
            {
                Result = Result + Arr[i];
            }

            Console.WriteLine("Сумма:");
            Console.WriteLine(Result);

            sWatch.Stop();
         
            Console.WriteLine("Последовательный алгоритм = {0} мс.", sWatch.ElapsedMilliseconds.ToString());


            InputData[] ClArr = new InputData[nc];// создание массива объектов для хранения параметров 
            for (i = 0; i < nc; i++)
                ClArr[i] = new InputData();

            int step = (Int32)(n / nc); // делим количество элементов  в массиве на nc частей 
            
            int c = -1;
            for (i = 0; i < nc; i++)// заполняем массив параметров 
            {
                ClArr[i].start = c + 1;
                ClArr[i].stop = c + step;
                ClArr[i].i = i;
                c = c + step;
            }
            Dispatcher d = new Dispatcher(nc, "Test Pool");
            DispatcherQueue dq = new DispatcherQueue("Test Queue", d);
            Port<int> p = new Port<int>();

            for (i = 0; i < nc; i++)
                Arbiter.Activate(dq, new Task<InputData, Port<int>>(ClArr[i], p, Sum));

            Arbiter.Activate(dq, Arbiter.MultipleItemReceive(true, p, nc, delegate(int[] array)
     {   }));}}}
