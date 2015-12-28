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
             double Pi = 0,dx=0,x=0;
             System.Diagnostics.Stopwatch sWatch = new System.Diagnostics.Stopwatch();
             sWatch.Start();
             dx = 1 / 10000000.0;
             x = data.start / 10000000.0;
             for (i = data.start; i <= data.stop; i++)

             {

                 Pi += Math.Sqrt(1 - x * x)*4*dx;
                 x += dx;
             }


             Console.WriteLine("часть Pi №{0} = {1}", Thread.CurrentThread.ManagedThreadId,Pi);
             
             sWatch.Stop();  
             resp.Post(1);
            
                
                 Console.WriteLine("Поток № {0}: Параллельный алгоритм = {1} мс.", Thread.CurrentThread.ManagedThreadId, sWatch.ElapsedMilliseconds.ToString());

        }
        

            static void Main(string[] args)
        {
            double Pi = 0, dx = 0, x = 0;
            int outer,i=0;
            nc = 4;
            n = 10000000;
            dx = 1 / 10000000.0;
            x = 0;
     

            System.Diagnostics.Stopwatch sWatch = new System.Diagnostics.Stopwatch();
            sWatch.Start();
            for (outer = 0; outer < n; outer++)
            {

                Pi += Math.Sqrt(1 - x * x) ;
                x += dx;
            }

            Pi = Pi * 4 * dx;
   

            sWatch.Stop();

            Console.WriteLine("Последовательный алгоритм = {0} мс.", sWatch.ElapsedMilliseconds.ToString());
           Console.WriteLine("Пи в последовательном={0}",Pi + " ");


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
