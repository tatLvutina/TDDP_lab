
using System;
using Microsoft.Ccr.Core;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;


namespace C11
{
    class Program
    {
        int[] A;
        int m;
        int nc;
        int parts;
        public static double h = 0.001;
        public double a;
        public double b;
        double result = 0;
        double planeResult = 0;
        public double x = 0;

        public static double f(double x)
        {
            return x * x * x/(x*x +1);
        }

        public class InputData
        {
            public double a;
            public int steps;
        }

        public void TestFunction()
        {
            nc = 2; // количество ядер
            m = 100000000; // количество строк матрицы
            a = -6;
            b = 9000;
            parts = (int)((b - a) / h);
            SequentialMul();
            ParallelMul();
        }

        public void ParallelMul()
        {
            
            InputData[] data = new InputData[nc]; //назначение задач на каждый поток

            for (int i = 0; i < nc; ++i)
            {
                data[i] = new InputData();

                data[i].a = a + i * (parts / nc) * h;
                data[i].steps = parts / nc;
            }
            Dispatcher d = new Dispatcher(nc, "Test Pool");
            DispatcherQueue dq = new DispatcherQueue("Test Queue", d);
            Port<double> p = new Port<double>();

            for (int i = 0; i < nc; i++)
                Arbiter.Activate(dq, new Task<InputData, Port<double>>(data[i], p, Mul));

              Arbiter.Activate(dq, Arbiter.MultipleItemReceive(true, p, nc, delegate(double[] array) 
            {
                for (int i = 0; i < array.GetLength(0); ++i)
                    result += array[i];

                Console.WriteLine("Последовательный результат: {0}", planeResult);
                Console.WriteLine("Параллельный результат: {0}", result);
            
            }));

        }

        public void Mul(InputData data, Port<double> resp)
        {
            Stopwatch sWatch = new Stopwatch();
            double result = 0;
            sWatch.Start();
            for (int i = 0; i < data.steps; ++i)
                result += (f(data.a + i * h) + f(data.a + (i + 1) + h)) * (h) ;

            sWatch.Stop();
            Console.WriteLine("Поток № {0}: Параллельный алгоритм = {1} мс.",
            Thread.CurrentThread.ManagedThreadId,
            sWatch.ElapsedMilliseconds.ToString());
            resp.Post(result);
            
        }

        public void SequentialMul()
        {
            Stopwatch sWatch = new Stopwatch();
            sWatch.Start();
            for (int i = 0; i < parts; ++i)    //последовательный алгоритм
                planeResult += (f(a + h * i) + f(a + h * (i + 1))) * (h) / 2;
            sWatch.Stop();
            Console.WriteLine("Последовательный алгоритм = {0} мс.", sWatch.ElapsedMilliseconds.ToString());
        }

        static void Main(string[] args)
        {
            Program p = new Program();
            p.TestFunction();
        }
    }
}
