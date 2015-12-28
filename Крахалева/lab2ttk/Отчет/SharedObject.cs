using System;
using System.Collections.Generic;

namespace SortLibrary
{
    public class SharedObject : MarshalByRefObject
    {
    
        static int number,i;
       static double sumEl;



        const int dataCount = 100; // Кол-во элементов в массиве
        const int tasksCount = 2; // максимальное кол-во задач

        Queue<Task> pendingTasks; // очередь задач ожидающих обработки
        Object tasksLock;

        //List<Task> finishedTasks;

        int[] dataArrayA, dataArrayB;
 
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
            dataArrayA = new int[dataCount];
            dataArrayB = new int[dataCount];

            for (int i = 0; i < dataCount; i++)
            {
                dataArrayA[i] = r.Next(0, dataCount * tasksCount);
                dataArrayB[i] = r.Next(0, dataCount * tasksCount);
            }
        }



        public int[] FetchData(Task task, int n)
        {
            Log.Print("Client has fetched data");

            if (n==0)
            { 
                int[] tempA = new int[task.stop-task.start];
            int j = 0;
            for (int i = task.start; i < task.stop; i++)
            {
                tempA[j] = dataArrayA[i];
                j++;
            }
                return tempA;
            }
            else
            {
                int[] tempB = new int[task.stop - task.start];
                int j = 0;
                for (int i = task.start; i < task.stop; i++)
                {
                    tempB[j] = dataArrayB[i];
                    j++;
                }
            return tempB;
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

        public void Finish(double sr)
        {
             
            Log.Print("Client has finished task");
            lock (dataLock)
            {   
                Console.Out.Write("Полученное значение: "+sr);
                sumEl+=sr;
                i++;
                Console.Out.WriteLine();

            }
            //finishedTasks.Add(task);
            if (pendingTasks.Count == 0)
            {

                Console.Out.WriteLine();

                Console.Out.WriteLine();
                Console.Out.Write("Итоговый результат: "+(sumEl));
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
