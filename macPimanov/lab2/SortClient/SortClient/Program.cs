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
        TcpChannel kanal;
        SharedObject obj;
        int[] arr;
        int max;
        int min;
        Task task;

        public Shell()
        {
            kanal = new TcpChannel();
            ChannelServices.RegisterChannel(kanal, false);
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

               
                for (outer = 0; outer < task.stop - task.start; outer++)
                {
                    for (int j = outer + 1; j < task.stop - task.start; j++)
                    {
                        if (arr[j] < arr[outer])
                        {
                            var temp = arr[outer];
                            arr[outer] = arr[j];
                            arr[j] = temp;
                        }
                    }

                    
                }


                Console.Out.WriteLine("Обработанные данные:");
                display();
                obj.Finish(task, arr);
            return 1;
        }

        void display()
        {
            for (int i = 0; i < task.stop - task.start; i++)
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

            Console.Out.WriteLine("Заданий нет");
            Console.ReadLine();
            

        }
    }
}
