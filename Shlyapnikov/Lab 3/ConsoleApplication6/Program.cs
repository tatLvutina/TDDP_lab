
using System;
using Microsoft.Ccr.Core;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;


namespace ConsoleApplication6
{
    class Program
    {
        int[] A;
        int[] C;
        int m;
        int nc;

        public class InputData
        {
            public int start; // начало диапазона
            public int stop; // начало диапазона
        }

        public void TestFunction()
        {
            nc = 2; // количество ядер
            m = 1000000; // количество строк матрицы
            A = new int[m];
            C = new int[m];

            Random r = new Random();
            for (int i = 0; i < m; i++)
            {
                 A[i] = r.Next(100);
            }

            for (int i = 0; i < m; i++)
            {
                C[i] = 0;
            }

            SequentialMul();

            ParallelMul();
        }

        public void ParallelMul()
        {
            // создание массива объектов для хранения параметров
            InputData[] ClArr = new InputData[nc];
            for (int i = 0; i < nc; i++)
                ClArr[i] = new InputData();
            // делим количество строк в матрице на nc частей
            int step = (Int32)(m / nc);
            // заполняем массив параметров
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
                Arbiter.Activate(dq, new Task<InputData, Port<int>>(ClArr[i], p, Mul));

            Arbiter.Activate(dq, Arbiter.MultipleItemReceive(true, p, nc, delegate(int[] array)
            {
                int res = 0;

                for (int i = 0; i < m; i++)
                {
                    res += C[i];
                }

                Console.WriteLine("Вычисления завершены: " + res.ToString());

                //Console.WriteLine("Вычисления завершены");
            }));

            
        }

        public void Mul(InputData data, Port<int> resp)
        {
            Stopwatch sWatch = new Stopwatch();
            sWatch.Start();
            for (int i = data.start; i < data.stop; i = i + 2)
            {
                if (i < data.stop)
                    C[i] = A[i] * A[i + 1];
            }
            sWatch.Stop();
            Console.WriteLine("Поток № {0}: Паралл. алгоритм = {1} мс.",
            Thread.CurrentThread.ManagedThreadId,
            sWatch.ElapsedMilliseconds.ToString());
            resp.Post(1);
        }

        public void SequentialMul()
        {
            Stopwatch sWatch = new Stopwatch();
            sWatch.Start();
            for (int i = 0; i < m; i = i + 2)
            {
                C[i] = A[i] * A[i + 1];
            }
            sWatch.Stop();

            int res = 0;

            for (int i = 0; i < m; i++)
            {
                res += C[i];
            }

            Console.WriteLine("Вычисления завершены: " + res.ToString());

            Console.WriteLine("Последовательный алгоритм = {0} мс.", sWatch.ElapsedMilliseconds.ToString());
        }

        static void Main(string[] args)
        {
            Program p = new Program();

            p.TestFunction();
        }
    }
}
