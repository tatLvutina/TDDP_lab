using System;
using Microsoft.Ccr.Core;
using System.Threading;

namespace ConsoleApplication3
{
    public class InputData
    {
        public int start; // начало диапазона 
        public int stop;  // конец диапазона 
        //public int i;
    }

    class Program
    {
        static int[,] A; //хранение матрицы
        static int[] B;  //хранение вектор-столбца для умножения
        static int[] C;  //хранение результата
        static int m;    //количество строк матрицы
        static int n;    //количество столбцов матрицы
        static int nc;   //количество ядер

        static void filling()
        {
            Console.WriteLine("Заполнение матриц");
            nc = 4;

     
                m = 10000;
                n = 10000;
     



            A = new int[m, n];
            B = new int[n];
            C = new int[m];

   
                Random r = new Random();
                for (int i = 0; i < m; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        A[i, j] = r.Next(100);
                    }
                }
                for (int j = 0; j < n; j++)
                    B[j] = r.Next(100);

 
            
        }

        static void SequentialMul()
        {
            System.Diagnostics.Stopwatch sWatch = new System.Diagnostics.Stopwatch();
            sWatch.Start();
            for (int i = 0; i < m; i++)
            {
                C[i] = 0;
                for (int j = 0; j < n; j++)
                {
                    C[i] += A[i, j] * B[j];
                   
                }
            }
            sWatch.Stop();
            Console.WriteLine("Последовательный алгоритм = {0} мс.",sWatch.ElapsedMilliseconds.ToString());

        }

        static void ParallelMul()
        {

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

            Dispatcher d = new Dispatcher(nc, "Test Pool");
            DispatcherQueue dq = new DispatcherQueue("Test Queue", d);

            Port<int> p = new Port<int>();

            for (int i = 0; i < nc; i++)
                Arbiter.Activate(dq, new Task<InputData, Port<int>>(ClArr[i], p, Mul));

            Arbiter.Activate(dq, Arbiter.MultipleItemReceive(true, p, nc, delegate (int[] array)
            {
  
                Console.WriteLine("Вычисления завершены");
                Console.ReadKey(true);
                Environment.Exit(0);
            }));
        }


        static void Mul(InputData data, Port<int> resp)
        {
            System.Diagnostics.Stopwatch sWatch = new System.Diagnostics.Stopwatch();
            sWatch.Start();

            for (int i = data.start; i < data.stop; i++)
            {
                C[i] = 0;
                for (int j = 0; j < n; j++)
                    C[i] += A[i, j] * B[j];
            }
            sWatch.Stop();
            Console.WriteLine("Поток № {0}: Паралл. алгоритм = {1} мс.",
           Thread.CurrentThread.ManagedThreadId,
           sWatch.ElapsedMilliseconds.ToString());
            resp.Post(1);
        }

      

        static void Main(string[] args)
        {

            filling();
            Console.WriteLine("Старт вычислений");
            SequentialMul();

            ParallelMul();
        }
    }
}

       