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
        public int start;
        public int stop;
    }
    class Program
    {
        static double[] A;
        static double[] B;
        static int m;
        static int nc;
        static void sinus(InputData data, Port<int> resp)
        {
            System.Diagnostics.Stopwatch sWatch = new System.Diagnostics.Stopwatch();
            sWatch.Start();
            for (int i = data.start; i <= data.stop; i++)
            {
                B[i] = Math.Sin(2 * i * Math.PI / 1024);
            }
            sWatch.Stop();
            resp.Post(1);
            Console.WriteLine(" Thread {0}: Parallel algorithm = {1} t. ", Thread.CurrentThread.ManagedThreadId, sWatch.ElapsedTicks);
        }
        static void Main(string[] args)
        {
            nc = 2;
            m = 1025;
            A = new double[m];
            B = new double[m];
            System.Diagnostics.Stopwatch sWatch = new System.Diagnostics.Stopwatch();
            sWatch.Start();
            for (int i = 0; i < m; i++)
            {
                A[i] = Math.Sin((2 * i * Math.PI) / 1024);
            }
            sWatch.Stop();
            Console.WriteLine(A[0]);
            Console.WriteLine(A[1]);
            Console.WriteLine(A[511]);
            Console.WriteLine(A[512]);
            Console.WriteLine(A[513]);
            Console.WriteLine(A[1023]);
            Console.WriteLine(A[1024]);
            Console.WriteLine(" Sequential algorithm = {0} t. ", sWatch.ElapsedTicks);
            InputData[] ClArr = new InputData[nc];
            for (int i = 0; i < nc; i++)
                ClArr[i] = new InputData();
            int step = (Int32)(m / nc);
            int c = -1;
            for (int i = 0; i < nc; i++)
            {
                ClArr[i].start = c + 1;
                ClArr[i].stop = c + step;
                c = c + step;
            }
            Dispatcher d = new Dispatcher(nc, " Test Pool ");
            DispatcherQueue dq = new DispatcherQueue(" Test Queue", d);
            Port<int> p = new Port<int>();
            for (int i = 0; i < nc; i++)
                Arbiter.Activate(dq, new Task<InputData, Port<int>>(ClArr[i], p, sinus));
            Arbiter.Activate(dq, Arbiter.MultipleItemReceive(true, p, nc, delegate (int[] array)
            {
                Console.WriteLine(B[0]);
                Console.WriteLine(B[1]);
                Console.WriteLine(B[511]);
                Console.WriteLine(B[512]);
                Console.WriteLine(B[513]);
                Console.WriteLine(B[1023]);
                Console.WriteLine(B[1024]);
            }));
        }
    }
}