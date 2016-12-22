using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Ccr.Core;
using System.Threading;

namespace ConsoleApplication1
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
        static double[] B;
        static int n;
        static int nc;


        static void Mul(InputData data, Port<int> resp)
        {
            double sum, sred;
            int i;
            int num = data.start / (data.stop - data.start);
            System.Diagnostics.Stopwatch sWatch = new System.Diagnostics.Stopwatch();
            sWatch.Start();
            sum = 0;

            for (i = data.start; i <= data.stop; i++)
            {
                sum = sum + A[i];

            }
            sred = (sum / (n / 2));
            if (num == 0) B[0] = sred;
            else B[1] = sred;
            Console.WriteLine(sred);
            Console.WriteLine("Поток № {0}: Параллельный алгоритм = {1} мс.", Thread.CurrentThread.ManagedThreadId, sWatch.ElapsedMilliseconds.ToString());
            Console.WriteLine("_______");


            sWatch.Stop();
            resp.Post(1);



        }
        static void Main(string[] args)
        {
            double sum, sredd;
            int i;
            n = 100000000;
            nc = 2;
            A = new int[n];
            B = new double[nc];

            Random r = new Random();
            for (int j = 0; j < n; j++)
                A[j] = r.Next(100);

            System.Diagnostics.Stopwatch sWatch = new System.Diagnostics.Stopwatch();
            sWatch.Start();
            sum = 0;
            for (i = 0; i < n; i++)
            {
                sum = sum + A[i];
            }
            sredd = sum / n;

            Console.WriteLine(sredd);

            sWatch.Stop();
            Console.WriteLine("Последовательный алгоритм = {0} мс.", sWatch.ElapsedMilliseconds.ToString());
            Console.WriteLine("___________");


            InputData[] ClArr = new InputData[nc];
            for (i = 0; i < nc; i++)
                ClArr[i] = new InputData();

            int point = (Int32)(n / nc);
            // заполняем массив параметров 
            int c = -1;
            for (i = 0; i < nc; i++)
            {
                ClArr[i].start = c + 1;
                ClArr[i].stop = c + point;
                ClArr[i].i = i;
                c = c + point;
            }
            Dispatcher d = new Dispatcher(nc, "Test Pool");
            DispatcherQueue dq = new DispatcherQueue("Test Queue", d);
            Port<int> p = new Port<int>();

            for (i = 0; i < nc; i++)
                Arbiter.Activate(dq, new Task<InputData, Port<int>>(ClArr[i], p, Mul));

            Arbiter.Activate(dq, Arbiter.MultipleItemReceive(true, p, nc, delegate(int[] array)
            {
                if (B[0] != 0 && B[1] != 0) { Console.Write("(THR1+THR2)/2 = "); Console.WriteLine((B[0] + B[1]) / 2); }

            }));
        }
    }
}