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
        int[] arr1,arr2;

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

                arr1 = obj.FetchData(task,0);
                arr2 = obj.FetchData(task,1);
                Console.Out.WriteLine("Полученные данные:");
                display();

                int  outer;
                float sum=0;

                for (outer = 0; outer < task.stop - task.start; outer++)
                {
                    sum += arr1[outer]*arr2[outer];
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
                Console.Out.Write(" ("+arr1[i]+" "+arr2[i]+")");
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
