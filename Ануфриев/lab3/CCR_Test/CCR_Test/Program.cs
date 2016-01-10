using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Ccr.Core;

namespace CCR_Test
{
    class Data
    {
        public int row = 0;
    }

    class Program
    {
        public static int[,] array;
        public static int[,] arrayLinear;

        const int arraySize = 300;
        static public void ShellSort(int row)
        {
            int size = arraySize;
            int step = size / 10;//инициализируем шаг.
            while (step > 0)//пока шаг не 0
            {
                for (int i = 0; i < (size - step); i++)
                {
                    int j = i;
                    //будем идти начиная с i-го элемента
                    while (j >= 0 && array[row, j] > array[row,j + step])
                    //пока не пришли к началу массива
                    //и пока рассматриваемый элемент больше
                    //чем элемент находящийся на расстоянии шага
                    {
                        //меняем их местами
                        int temp = array[row,j];
                        array[row, j] = array[row, j + step];
                        array[row, j + step] = temp;
                        j--;
                    }
                }
                step = step / 2;//уменьшаем шаг
            }
        }
        static void Task(Data d, Port<int> resp)
        {
            ShellSort(d.row);
            resp.Post(1);
        }

        static void ParallelShellSort()
        {
            Dispatcher dispatcher = new Dispatcher(arraySize, "THREAD_POOL");
            DispatcherQueue dsipQueue = new DispatcherQueue("Dispatcher", dispatcher);

            Port<int> port = new Port<int>();

            for (int i = 0; i < arraySize; ++i)
            {
                Data d = new Data();
                d.row = i;
                Arbiter.Activate(dsipQueue, new Task<Data, Port<int>>(d, port, Task));

            }

            return;
        }


        static void Main(string[] args)
        {
            System.IO.StreamWriter s = new System.IO.StreamWriter("in.txt");
            Random rand = new Random();
            Console.WriteLine("Generating random values: ");
            array = new int[arraySize, arraySize];
            arrayLinear = new int[arraySize, arraySize];

            int c = 0;
            for (int i = 0; i < arraySize; ++i)
            {
                for (int j = 0; j < arraySize; ++j)
                {
                    c = rand.Next(50);
                    array[i, j] = c;
                   arrayLinear[i, j] = c;
                   s.Write(" {0} ", c);
                    
                }
                s.WriteLine();
            }

            s.Close();

            Console.WriteLine("Start sorting: ");
            
            Stopwatch sw = new Stopwatch();

            sw.Start();
            ParallelShellSort();
            sw.Stop();

            Console.WriteLine("Parallel sort ended. Time: {0}", sw.Elapsed.Milliseconds);

            Console.WriteLine("Start linear sort: ");

            Stopwatch sw1 = new Stopwatch();
            sw1.Start();
            c = 0;
            for (int k = 0; k < arraySize; ++k)
            {
                for (int i = 0; i < arrayLinear.GetLength(1) - 1; i++)
                {
                    for (int j = 0; j < arrayLinear.GetLength(1) - i - 1; j++)
                    {
                        if (arrayLinear[k, j] > arrayLinear[k, j + 1])
                        {
                            c = arrayLinear[k, j + 1];
                            arrayLinear[k, j + 1] = arrayLinear[k, j];
                            arrayLinear[k, j] = c;
                        }
                    }
                }
            }
            sw1.Stop();

            Console.WriteLine("Planar sorting ended. Time: {0}", sw1.Elapsed.Milliseconds);


            s = new System.IO.StreamWriter("out.txt");
            for (int i = 0; i < arraySize; ++i)
            {
                for (int j = 0; j < arraySize; ++j)
                {
                    s.Write(" {0} ", array[i, j]);
                }
                s.WriteLine();
            }
            s.Close();

            Console.WriteLine("Writing to file ended.");

            Console.ReadLine();
            return;
        }
    }
}
