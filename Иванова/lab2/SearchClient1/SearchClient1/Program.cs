using System;
using SearchClasses;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace SearchClient
{
    class SubProgram
    {
        TcpChannel chan;
        RemoteObject obj;
        int[] arr;
        Task task;

        public SubProgram()
        {
            chan = new TcpChannel();
            ChannelServices.RegisterChannel(chan, false);
            obj = (RemoteObject)Activator.GetObject(typeof(SearchClasses.RemoteObject), "tcp://localhost:8080/Search");
            obj.Create();
            obj.GenerateTasks();
        }

        public int search()
        {
            obj.setManagingClient();
            obj.joinToServer();
            task = obj.GetTask();
            if (task == null)
                return 0;

            arr = obj.getData(task);
            int[] res = new int[2];
            res[0] = arr[0];//min
            res[1] = arr[0];//max
            Console.Out.WriteLine("Полученные данные:");
            display();

            for (int i = 0; i < task.stop - task.start; i++)
            {
                if (arr[i] < res[0])
                    res[0] = arr[i];
            }
            res[1] = arr[0];
            for (int i = 0; i < task.stop - task.start; i++)
            {
                if (arr[i] > res[1])
                    res[1] = arr[i];
            }
            Console.WriteLine("Минимальное: " + res[0], "\n");
            Console.WriteLine("Максимальное: " + res[1], "\n");
            obj.Complete(res);
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
            SubProgram SubProgramObj = new SubProgram();
            Console.Out.WriteLine("Клиент запущен");
            while (SubProgramObj.search() != 0)
                Console.In.ReadLine();
        }
    }
}

