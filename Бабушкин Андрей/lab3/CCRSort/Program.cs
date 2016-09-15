using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Ccr.Core;
using System.Threading;

namespace CCRSort
{
    class Program
    {
        static void Main(string[] args)
        {
            Sorting seq = new Sorting();
            Sorting par = new Sorting();

            seq.SequentialShell();
            par.ParallelShell();
            Console.Read();
        }
    }


    class Sorting
    {
        const int partSize = 100000; // элементов в одном процессе
        const int nc = 4; // кол-во процессов
        int count;
        int[] dataArray;
        Object dataLock;
        InputData[] processes; // процессы для параллельной сортировки


        Dispatcher d = new Dispatcher(nc, "Pool");
        DispatcherQueue dq;
        Port<int> p1;

        class InputData
        {
            public int start;
            public int stop;
        }


        public Sorting()
        {
            Random r = new Random();
            count = partSize * nc;
            dataArray = new int[count];
            dataLock = new Object();

            for (int i = 0; i < count; i++)
                dataArray[i] = r.Next(count * 10);

        }

        public void SequentialShell()
        {
            Console.Write("Последовательная сортировка: ");
            //Console.WriteLine("Массив до сортировки:");
            //Display(0, partSize * nc);

            Stopwatch sw = new Stopwatch();
            sw.Start();

            int inner, outer, temp;
            int h = (count) / 2;

            while (h > 0)
            {
                for (outer = h; outer < count; outer++)
                {
                    temp = dataArray[outer];
                    inner = outer;

                    while (inner > h - 1 && dataArray[inner - h] >= temp)
                    {
                        dataArray[inner] = dataArray[inner - h];
                        inner -= h;
                    }
                    dataArray[inner] = temp;
                }
                h = h / 2;
            }
            
            sw.Stop();
            Console.WriteLine("Сортировка завершена. Длительность: {0} мс", sw.ElapsedMilliseconds.ToString());
            //Console.WriteLine("Отсортированный массив: ");
            //Console.WriteLine("Результат:");
            //Display(0, nc * partSize);

        }

        public void ParallelShell()
        {
            processes = new InputData[nc];
            Console.WriteLine("Параллельная сортировка");
            //Console.WriteLine("Массив до сортировки:");
            //Display(0, partSize * nc);

            int c = -1;
            for (int i = 0; i < nc; i++)
            {
                processes[i] = new InputData();
                processes[i].start = c + 1;
                processes[i].stop = c + partSize - 1;
                c += partSize - 1;
            }

             dq = new DispatcherQueue("Queue", d);
             p1 = new Port<int>();


            for (int i = 0; i < nc; i++)
            {
                Arbiter.Activate(dq, new Task<InputData, Port<int>>(processes[i], p1, FirstPhase));
            }

            // Все потоки завершили работу, финальная сортировка
            Arbiter.Activate(dq, Arbiter.MultipleItemReceive(true, p1, nc, delegate(int[] array)
            {
                Stopwatch sw = new Stopwatch();

                Console.WriteLine("Финальная сортировка");
                sw.Start();
                

                //int inner, temp;

                //for (int i = 0; i < partSize * nc; i++)
                //{
                //    temp = dataArray[i];
                //    inner = i;

                //    while (inner > 0 && dataArray[inner - 1] >= temp)
                //    {
                //        dataArray[inner] = dataArray[inner - 1];
                //        inner--;
                //    }
                //    dataArray[inner] = temp;
                //}
                Array.Sort(dataArray);
                sw.Stop();
                Console.WriteLine("Финальная сортировка заняла {0} мс", sw.ElapsedMilliseconds.ToString());
                //Console.WriteLine("Результат:");
                //Display(0, nc * partSize);
            
            }
        ));
        }
        
        // первая фаза алгоритма: сортировка части
        void FirstPhase(InputData data, Port<int> resp)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            // по идее каждый поток работает со своей частью массива
            // поэтому его не надо защищать lock'ом

            int inner, outer, temp;
            int h = partSize / 2;

            while (h > 0)
            {
                for (outer = h+data.start; outer < data.stop; outer++)
                {
                    temp = dataArray[outer];
                    inner = outer;

                    while (inner > (data.start + h - 1) && dataArray[inner - h] >= temp)
                    {
                        dataArray[inner] = dataArray[inner - h];
                        inner -= h;
                    }
                    dataArray[inner] = temp;
                }
                h = h / 2;
            }

             
            sw.Stop();
            Console.WriteLine("Поток {0}: {1} мс", Thread.CurrentThread.ManagedThreadId, sw.ElapsedMilliseconds.ToString());
            //Console.WriteLine("Результат:");
            //Display(data.start, data.stop);
            resp.Post(1);
        }


        void Display(int start, int stop)
        {
            for (int i = start; i < stop; i++)
                Console.Write(dataArray[i] + "  ");

            Console.WriteLine();
        }
        
    }

}
