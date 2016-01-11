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

                int inner, outer, temp;
                for (outer = 1; outer < task.indexes.Count; outer++)
                {
                    temp = arr[outer];
                    inner = outer;
                    while (inner > 0 && arr[inner - 1] >= temp)
                    {
                        arr[inner] = arr[inner - 1];
                        inner--;
                    }
                    arr[inner] = temp;
                }

                //int inner, outer, temp;
                //int h = 1;

                //while (h <= task.size / 3)
                //    h = h * 3 + 1;

                //while (h > 0)
                //{
                //    for (outer = h; outer < task.size; outer++)
                //    {
                //        temp = task.data[outer];
                //        inner = outer;

                //        while (inner > h - 1 && task.data[inner - h] >= temp)
                //        {
                //            task.data[inner] = task.data[inner - h];
                //            inner -= h;
                //        }
                //        task.data[inner] = temp;
                //    }
                //    h = (h - 1) / 3;
                //}

                Console.Out.WriteLine("Обработанные данные:");
                display();
                obj.Finish(task, arr);
            //}
            //catch (System.Net.WebException e)
            //{
            //    Console.Out.WriteLine("Error " + e.Message);
            //}

            return 1;
        }

        void display()
        {
            for (int i = 0; i < task.indexes.Count; i++)
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
