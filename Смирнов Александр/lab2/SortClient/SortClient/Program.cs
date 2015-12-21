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

        Task task;

        public Shell()
        {
            chan = new TcpChannel();
            ChannelServices.RegisterChannel(chan, false);
            obj = (SharedObject)Activator.GetObject(typeof(SortLibrary.SharedObject), "tcp://localhost:8080/Work");
        }

        public int sort()
        {      
                task = obj.GetTask();
      
                if (task == null)
                    return 0;
                int[,] ATemp;
                int[] BTemp;
                int[] C;
                 obj.GetData(task, out BTemp, out ATemp);
               C= new int[SharedObject.n];
                Console.Out.Write("Полученные данные:");

            Console.Out.Write("\nСтроки матрицы A:\n");
            for (int i = task.start; i <= task.stop; i++) // строк матрицы взяли столько, сколько указано для данного клиента (от строки start до строки stop)
                {
                Console.Out.WriteLine();
                for (int j = 0; j < SharedObject.n; j++) // столбцов - столько, сколько есть всего 
                    {
                        Console.Out.Write(ATemp[i,j]+"\t");
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

    }

    class Program
    {


        static void Main(string[] args)
        {
            Shell shellObj = new Shell();
            Console.Out.WriteLine("Клиент запущен!");

            while (shellObj.sort() != 0)
                Console.In.ReadLine();

            Console.Out.WriteLine("Задач больше нет!");
            Console.ReadLine();
            

        }
    }
}
