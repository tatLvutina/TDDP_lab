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

                task = obj.GetTask();
                if (task == null)
                    return 0;

                arr = obj.FetchData(task);
               
                Console.Out.WriteLine("Полученные данные:");
                display();

                int  outer;
                float sum=0;
    
                for (outer = 0; outer < task.stop - task.start; outer++)
                {
                    sum += arr[outer];
                }
   
                Console.Out.WriteLine("Сумма элементов задания:");
                Console.Out.Write(sum);
                Console.Out.WriteLine("  ");

                obj.Finish(sum);

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

            

        }
    }
}
