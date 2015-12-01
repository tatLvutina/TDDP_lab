using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SortLibrary;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace SortClient
{
    class Shell
    {
        TcpChannel chan;
        SharedObject obj;
        int[] arr;
        int max;
        int min;
        Task task;

        public Shell()
        {
            chan = new TcpChannel();
            ChannelServices.RegisterChannel(chan, false);
            obj = (SharedObject)Activator.GetObject(typeof(SortLibrary.SharedObject), "tcp://localhost:8081/DataPool");
        }

        public int sort()
        {
            //try
            //{
                task = obj.GetTask();
                if (task == null)
                    return 0;

                arr = obj.FetchData(task);
               
                Console.Out.WriteLine("Полученные данные:");
                display();

                int  outer;
                max = arr[0];
                min = arr[0];
               
                for (outer = 0; outer < task.stop - task.start; outer++)
                {
                    if (max < arr[outer]) max = arr[outer];
                    if (min > arr[outer]) min = arr[outer];

                    
                }


                Console.Out.WriteLine("Обработанные данные:");
                Console.Out.WriteLine("Max: " + max + ". Min: " + min);
                obj.Finish(task, max, min);
            //}
            //catch (System.Net.WebException e)
            //{
            //    Console.Out.WriteLine("Error " + e.Message);
            //}
                task.stop = 10; //МЕГАКОСТЫЛЬ
                task.start = 6;
            return 1;
        }

        void display()
        {
            for (int i = task.start; i < task.stop; i++)
            {
                Console.Out.Write(arr[i]);
                Console.Out.Write("  ");
            }
            Console.Out.WriteLine();
        }
    }

    class Program
    {


        static void Main(string[] args)
        {
            Shell shellObj = new Shell();
            Console.Out.WriteLine("Клиент запущен");

            while (shellObj.sort() != 0)
                Console.In.ReadLine();

            Console.Out.WriteLine("Задачи кончились, нажмите Enter...");
            Console.ReadLine();
            

        }
    }
}
