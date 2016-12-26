using System;

using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

using Lib;

namespace Client
{
    class Client
    {
        TcpChannel chan;
        SharedObject obj;

        Task task;

        public Client() {
            chan = new TcpChannel();
            ChannelServices.RegisterChannel(chan, false);
            obj = (SharedObject)Activator.GetObject(typeof(Lib.SharedObject), "tcp://localhost:8081/Пул данных");
        }

        public int run()
        {
            task = obj.GetTask();
            if (task == null)
                return 0;
            int[,] ATemp;
            int[] BTemp;
            int[] C;
            obj.GetData(task, out BTemp, out ATemp);
            C = new int[SharedObject.n];
            Console.Out.Write("Полученные данные:");

            Console.Out.Write("\nСтроки матрицы A:\n");
            for (int i = task.start; i <= task.stop; i++) // строк матрицы взяли столько, сколько указано для данного клиента (от строки start до строки stop)
            {
                Console.Out.WriteLine();
                for (int j = 0; j < SharedObject.n; j++) // столбцов - столько, сколько есть всего 
                {
                    Console.Out.Write(ATemp[i, j] + "\t");
                }
            }

            Console.Out.Write("\n\nВектор-столбец B:\n");
            Console.Out.WriteLine();
            for (int j = 0; j < SharedObject.n; j++)
            {
                Console.Out.Write(BTemp[j] + "\n");   //кол-во строк вектор-столбца обязательно равно кол-ву столбцов умножаемой на него матрицы
            }

            Console.Out.WriteLine("\nПроверка вычисления C[i]:");
            for (int i = task.start; i <= task.stop; i++)
            {
                C[i] = 0;
                Console.Out.WriteLine();
                for (int j = 0; j < SharedObject.n; j++)
                {
                    C[i] += ATemp[i, j] * BTemp[j];
                    Console.Out.Write(" {0} * {1} ", ATemp[i, j], BTemp[j]);
                    if (j + 1 != SharedObject.n)
                    {
                        Console.Out.Write("+");
                    }
                    else
                    {
                        Console.Out.Write("= {0}", C[i]);
                    }
                }
                Console.Out.WriteLine();
            }

            obj.Finish(C);

            return 1;
        }
        static void Main(string[] args)
        {
            Client clientObj = new Client();
            Console.Out.WriteLine("Клиент запушен");

            while (clientObj.run() != 0)
                Console.In.ReadLine();

            Console.Out.WriteLine("Задания концились, нажмите Enter для выхода");
            Console.ReadLine();
        }
    }
}
