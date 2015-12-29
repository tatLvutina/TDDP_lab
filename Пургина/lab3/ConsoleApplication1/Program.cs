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
        public int j=0;
    }

    class Program
    {
        static int[] a;
        static int[] b;
        static int n;
        static int nc;
 

        static void Mul(InputData data, Port<int> resp)
         {
             int i;
             float sum = 0;
             System.Diagnostics.Stopwatch sWatch = new System.Diagnostics.Stopwatch();
             sWatch.Start();
             for (i = data.start; i <= data.stop; i++)

             {
                 
                 sum += a[i];
             }


             Console.WriteLine("Среднее в потоке №{0} = {1}", Thread.CurrentThread.ManagedThreadId, Math.Round((sum / n), 0));
             
             sWatch.Stop();  
             resp.Post(1);
            
                
                 Console.WriteLine("Поток № {0}: Параллельный алгоритм = {1} мс.", Thread.CurrentThread.ManagedThreadId, sWatch.ElapsedMilliseconds.ToString());

        }
        

            static void Main(string[] args)
        {
            int outer,i=0;
            nc = 4;
            n = 10000001;
            float sum = 0;
            a = new int[n];
            b = new int[nc];
 

            Random r = new Random();
            for (int j = 0; j < n; j++)
                a[j] = r.Next(100);

            System.Diagnostics.Stopwatch sWatch = new System.Diagnostics.Stopwatch();
            sWatch.Start();
            for (outer = 0; outer < n; outer++)
            {
                
                sum += a[outer];
            }


   

            sWatch.Stop();

            Console.WriteLine("Последовательный алгоритм = {0} мс.", sWatch.ElapsedMilliseconds.ToString());
           Console.WriteLine("Среднее в последовательном={0}",Math.Round(( sum / n),0) + " ");


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
         
            
            Dispatcher d = new Dispatcher(nc, "Test Pool");
            DispatcherQueue dq = new DispatcherQueue("Test Queue", d);
            Port<int> p = new Port<int>();


            for (i = 0; i < nc; i++)
                Arbiter.Activate(dq, new Task<InputData, Port<int>>(ClArr[i], p, Mul));

            Arbiter.Activate(dq, Arbiter.MultipleItemReceive(true, p, nc, delegate(int[] array)
     {   }));

        }
    }
}
