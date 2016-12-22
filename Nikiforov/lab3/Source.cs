using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Ccr.Core;
using System.Threading;

namespace ConsoleApplication6
{
    public class InputData
    {
        public int start; 
        public int stop;           
    }

    class Program
    {
        static int[] A;
        static int[] B;
        static int n;
        static int nc;

        static void MinMax(InputData data, Port<int> resp)
        {
            int min = A[0], max = A[0];
            System.Diagnostics.Stopwatch sWatch = new System.Diagnostics.Stopwatch();
            sWatch.Start();
            for (int i = data.start; i <= data.stop; i++)
            {
                if (A[i] < min)
                    min = A[i];

                if (A[i] > max)
                    max = A[i];
            }
            sWatch.Stop();
            resp.Post(1);

            Console.WriteLine(min);
            Console.WriteLine(max);
            Console.WriteLine("Thread {0}: Parallel algorithm = {1} ms.", Thread.CurrentThread.ManagedThreadId, sWatch.ElapsedMilliseconds.ToString());
        }


        static void Main(string[] args)
        {
            int min, max;
            nc = 2;
            n = 100000000;
            A = new int[n];
            Random r = new Random();
            for (int j = 0; j < n; j++)
                A[j] = r.Next(1000);
            System.Diagnostics.Stopwatch sWatch = new System.Diagnostics.Stopwatch();
            sWatch.Start();
            min = A[0];
            for (int i = 1; i < n; i++)
            {
                if (A[i] < min)
                    min = A[i];
            }
            max = A[0];
            for (int i = 1; i < n; i++)
            {
                if (A[i] > max)
                    max = A[i];
            }

            Console.WriteLine(min);
            Console.WriteLine(max);

            sWatch.Stop();

            Console.WriteLine("Sequential algorithm = {0} ms.", sWatch.ElapsedMilliseconds.ToString());

            
            InputData[] ClArr = new InputData[nc];
            for (int i = 0; i < nc; i++)
                ClArr[i] = new InputData();
            
            int step = (Int32)(n / nc);
            
            int c = -1;
            for (int i = 0; i < nc; i++)
            {
                ClArr[i].start = c + 1;
                ClArr[i].stop = c + step;            
                c = c + step;
            }
            Dispatcher d = new Dispatcher(nc, "Test Pool");
            DispatcherQueue dq = new DispatcherQueue("Test Queue", d);
            Port<int> p = new Port<int>();


            for (int i = 0; i < nc; i++)
                Arbiter.Activate(dq, new Task<InputData, Port<int>>(ClArr[i], p, MinMax));

            Arbiter.Activate(dq, Arbiter.MultipleItemReceive(true, p, nc, delegate (int[] array)
            { }));

        }
    }
}
