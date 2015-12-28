using System;
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
        static long [] a;
        static long [] b;
        static long[] ResultArr;
        static int n;
        static int nc;
        static void Mul(InputData data, Port<int> resp)
         {
            long Result = 0;
            int i = 0;
             System.Diagnostics.Stopwatch sWatch = new System.Diagnostics.Stopwatch();
            Random r = new Random();
            for (int j = 0; j < n; j++)
            {
                a[j] = r.Next(1, 3);
                b[j] = r.Next(1, 3);

            }
            long ResultA = 1;
            long ResultB = 1;
            long Res = 0;
            sWatch.Start();
             for (i = data.start; i <= data.stop; i++)
             {

                ResultA = ResultA * a[i];
                ResultB = ResultB * b[i];
                ResultA = ResultA / 25;
                ResultArr[i] = ResultA + ResultB;
                Res = Res + ResultArr[i];

            }
             sWatch.Stop();  
             resp.Post(1);
            

                 Console.WriteLine("Поток № {0}: Параллельный алгоритм = {1} мс.", Thread.CurrentThread.ManagedThreadId, sWatch.ElapsedMilliseconds.ToString());
        }
        

            static void Main(string[] args)
        {
            int i;
            nc = 4;
            n = 10000000;

            a = new long[n];
            b = new long[n];
            ResultArr = new long[n];
            long ResultA = 1;
            long Res = 0;
            long ResultB = 1;


            Random r = new Random();
            for (int j = 0; j < n; j++)
            {
                a[j] = r.Next(1, 3);
                b[j] = r.Next(1, 3);
                
            }

            System.Diagnostics.Stopwatch sWatch = new System.Diagnostics.Stopwatch();
            sWatch.Start();
            for (i = 0; i < n; i++)
            {

                ResultA = ResultA * a[i];
                ResultB = ResultB * b[i];
                
                ResultArr[i] = ResultA + ResultB;
                Res = Res + ResultArr[i];
            }


            
            Console.WriteLine(ResultB);
            sWatch.Stop();
         
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
