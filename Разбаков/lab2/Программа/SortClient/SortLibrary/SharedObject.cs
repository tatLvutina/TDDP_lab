using System;
using System.Collections.Generic;

namespace SortLibrary
{
    public class SharedObject : MarshalByRefObject
    {

        static int i;
        public static int m = 100, n = 100;
       



        const int dataCount = 100; // Кол-во элементов в массиве
        const int tasksCount = 2; // максимальное кол-во задач

        Queue<Task> pendingTasks; // очередь задач ожидающих обработки
        Object tasksLock;

        //List<Task> finishedTasks;

        double[,] dataArray;
       public double[] B = new double[n];
       public double[] C = new double[m];
        Object dataLock;

        public SharedObject()
        {
            Log.Print("Create tasks and data");
            //dataArray = new int[dataCount];
            pendingTasks = new Queue<Task>();
            GenerateData();
            GenerateTasks();

            tasksLock = new Object();
            dataLock = new Object();
        }

        void GenerateTasks()
        {
            Task temp;

            int step = dataCount / tasksCount; // на каждую задачу приходится равная порция массива

            for (int i = 0; i < tasksCount; i++)
            {
                temp = new Task();
                temp.start = i * step;
                temp.stop = temp.start + step - 1;
                pendingTasks.Enqueue(temp);
            }


        }

        void GenerateData()
        {
            Random r = new Random();
            dataArray = new double[m,n];
            B = new double[n];
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    dataArray[i, j] = r.Next(0, 10);
                }
            }
            for (int j = 0; j < n; j++)
            {
                B[j] = r.Next(0, 10);
            }
        }



        public void FetchData(Task task,out double[] b1,out double[,] temp  )
        {
            Log.Print("Client has fetched data");
             temp = new double[m,n];
            b1 = new double[n];
            for (int i = task.start; i < task.stop; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    temp[i,j] = dataArray[i,j];
                }

            }
            for (int j = 0; j < n; j++)
            {
                b1[j] = B[j];
            }
        }

        public Task GetTask()
        {
   
            Log.Print("Client has requested task");
            lock (tasksLock)
            {
                if (pendingTasks.Count == 0)
                {
                    Log.Print("No more tasks");
                    return null;
                }
                else
 
                    return pendingTasks.Dequeue();
                //return (pendingTasks.Count == 0 ? null : pendingTasks.Dequeue());
            }
        }

        public void Finish(double[] mas)
        {
             
            Log.Print("Client has finished task");
            lock (dataLock)
            {
                
                    for (int i = 0; i < n; i++)
                    {
                        C[i]+= mas[i];
                    }
                

            }
            //finishedTasks.Add(task);
            if (pendingTasks.Count == 0)
            {
                Console.Out.Write("Полученные значения ");
                Console.Out.WriteLine();
                for (int j = 0; j < n; j++)
                {
                    Console.Out.Write(C[j] + " ");
                }
                Console.Out.WriteLine();
            }
        }
    }



    [Serializable]
    public class Task
    {
        public int start = 0;
        public int stop = 0;
    //  public List<int> srEL;
        //public Task()
        //{
        //    indexes = new List<int>();

        //}
    }

    public class Log
    {
        // вывести время и msg
        public static void Print(String msg)
        {
            System.Console.WriteLine("[" + DateTime.Now.Hour.ToString() + ":" +
                DateTime.Now.Minute.ToString() + ":" + DateTime.Now.Second.ToString() 
            + "] " + msg);
        }
    }
}
