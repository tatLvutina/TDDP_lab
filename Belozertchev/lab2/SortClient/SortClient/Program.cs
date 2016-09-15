using System;
using SortLibrary;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace SortClient
{
    class Shell
    {
        TcpChannel chan;
        SharedObject obj;
        int[] Arr;

        Task task;

        public Shell()
        {
            chan = new TcpChannel();
            ChannelServices.RegisterChannel(chan, false);
            obj = (SharedObject)Activator.GetObject(typeof(SortLibrary.SharedObject), "tcp://localhost:8081/DataPool");
        }

        public int sort()
        {

                task = obj.GetTask();
                if (task == null)
                    return 0;

                Arr = obj.FetchData(task);
               
                Console.Out.WriteLine("Полученные данные:");
                display();

                int  outer;
                float Sum = 0;
    
                for (outer = 0; outer < task.stop - task.start; outer++)
                {
                    Sum = Sum + Arr[outer];
                }
   
                Console.Out.WriteLine("Сумма элементов задания:");
                Console.Out.Write(sum);
                Console.Out.WriteLine("  ");

                obj.Finish(Sum);

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

            Console.Out.WriteLine("Задачи закончились, нажмите Enter для выхода");
            Console.ReadLine();
            

        }
    }
}
