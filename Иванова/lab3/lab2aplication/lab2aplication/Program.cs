using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Ccr.Core;
using System.Threading;

namespace Lab2
{
    public class Data
    {
        public int start;
        public int stop;
        public int i;
    }

    class Program
    {
        static int[] arr ;
        static int[] task;
        static int size;
        static int countTask;

        static void ParSearch(Data data, Port<int> p)
        {
            int min2 = arr[0], max2 = arr[0];
            System.Diagnostics.Stopwatch sWatch = new System.Diagnostics.Stopwatch();
            sWatch.Start();
            for (int i = data.start; i <= data.stop; i++)
            {
                if (arr[i] < min2)
                    min2 = arr[i];

                if (arr[i] > max2)
                    max2 = arr[i];
            }
            sWatch.Stop();
            p.Post(1);

            Console.WriteLine(min2);
            Console.WriteLine(max2);
            Console.WriteLine("P {0}: Time of ParSearch {1} ms.", Thread.CurrentThread.ManagedThreadId, sWatch.ElapsedMilliseconds.ToString());
        }


        static void Main(string[] args)
        {
            int min, max;
            countTask = 2;
            size = 1000000;

            arr = new int[size];
            task = new int[countTask];

            Random r = new Random();
            for (int j = 0; j < size; j++) arr[j] = r.Next(100000);
            System.Diagnostics.Stopwatch sWatch = new System.Diagnostics.Stopwatch();
            sWatch.Start();
            min = arr[0];
            for (int i = 1; i < size; i++)
            {
                if (arr[i] < min)
                    min = arr[i];
            }
            max = arr[0];
            for (int i = 1; i < size; i++)
            {
                if (arr[i] > max)
                    max = arr[i];
            }

            Console.WriteLine(min);
            Console.WriteLine(max);

            sWatch.Stop();

            Console.WriteLine("Time of Search = {0} ms.", sWatch.ElapsedMilliseconds.ToString());
            Data[] TaskArr = new Data[countTask];
            for (int i = 0; i < countTask; i++) TaskArr[i] = new Data();
            int step = (Int32)(size / countTask);
            int count = -1;
            for (int i = 0; i < countTask; i++)
            {
                TaskArr[i].start = count + 1;
                TaskArr[i].stop = count + step;
                TaskArr[i].i = i;
                count = count + step;
            }
            Dispatcher disp = new Dispatcher(countTask, "Test Pool");
            DispatcherQueue dispQueue = new DispatcherQueue("Test Queue", disp);
            Port<int> p = new Port<int>();


            for (int i = 0; i < countTask; i++) Arbiter.Activate(dispQueue, new Task<Data, Port<int>>(TaskArr[i], p, ParSearch));
            Arbiter.Activate(dispQueue, Arbiter.MultipleItemReceive(true, p, countTask, delegate(int[] array){ }));
        }
    }
}
