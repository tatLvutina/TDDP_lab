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
            obj = (SharedObject)Activator.GetObject(typeof(SortLibrary.SharedObject), "tcp://localhost:8080/Lab");
        }

        public int sort()
        {
                task = obj.GetTask();
                if (task == null)
                    return 0;

                arr = obj.GetData(task);
               
                Console.Out.WriteLine("Полученные данные:");
                display();

            for (int i = 0; i <= arr.Length - 1; i++)
            {
                for (int j = i + 1; j < arr.Length; j++)
                {
                    if (arr[j] < arr[i])
                    {
                        var spam = arr[i];
                        arr[i] = arr[j];
                        arr[j] = spam;
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
            for (int i = 0; i < SharedObject.clientPortion; i++)
            {
                Console.Out.Write("{0}\t", arr[i].ToString());
            }
            Console.Out.WriteLine();
        }
    }

    class Program
    {


        static void Main(string[] args)
        {
            Shell shellObj = new Shell();
            Console.Out.WriteLine("Клиент запущен!");

            while (shellObj.sort() != 0)
                Console.In.ReadLine();

            Console.Out.WriteLine("Все задачи решены!");
            Console.ReadLine();
            

        }
    }
}
