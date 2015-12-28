using System;
using System.Collections.Generic;

namespace SortLibrary
{
    public class SharedObject : MarshalByRefObject
    {
    
        const int dataCount = 1000000; // Кол-во элементов в массиве
        const int tasksCount = 2; // максимальное кол-во задач

        Queue<Task> pendingTasks; // очередь задач ожидающих обработки
        Object tasksLock;

        //List<Task> finishedTasks;

        double Pi = 0;
 
        Object dataLock;

        public SharedObject()
        {
            Log.Print("Create tasks and data");
      
            pendingTasks = new Queue<Task>();

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

      



        public double[] FetchData(Task task)
        {
            Log.Print("Client has fetched data");
            double[] temp = new double[task.stop-task.start];

            return temp;
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

        public void Finish(double PiTask)
        {
             
            Log.Print("Client has finished task");
            lock (dataLock)
            {
                Pi += PiTask;


            }
            //finishedTasks.Add(task);
            if (pendingTasks.Count == 0)
            {
                
                 Console.Out.WriteLine(" Pi= " + Pi);
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
