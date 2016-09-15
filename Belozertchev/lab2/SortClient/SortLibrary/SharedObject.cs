using System;
using System.Collections.Generic;

namespace SortLibrary
{
    public class SharedObject : MarshalByRefObject
    {
    
        static int number,i;
       static double[] srEl = new double[2];



        const int dataCount = 100; // Кол-во элементов в массиве
        const int tasksCount = 2; // максимальное кол-во задач

        Queue<Task> pendingTasks; // очередь задач ожидающих обработки
        Object tasksLock;

        int[] dataArray;
 
        Object dataLock;

        public SharedObject()
        {
            Log.Print("Create tasks and data");
            pendingTasks = new Queue<Task>();
            GenerateData();
            GenerateTasks();

            tasksLock = new Object();
            dataLock = new Object();
        }

        void GenerateTasks()
        {
            Task temp;

            int step = dataCount / tasksCount; // делим массив на части

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
            dataArray = new int[dataCount];

            for (int i = 0; i < dataCount; i++)
                dataArray[i] = r.Next(0, dataCount * tasksCount);
        }



        public int[] FetchData(Task task)
        {
            Log.Print("Client has fetched data");
            int[] temp = new int[task.stop-task.start];
            int j = 0;
            for (int i = task.start; i < task.stop; i++)
            {
                temp[j] = dataArray[i];
                j++;
            }

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
            }
        }

        public void Finish(int sr)
        {
             
            Log.Print("Client has finished task");
            lock (dataLock)
            {   
                srEl[i]=sr;
                i++;
                Console.Out.WriteLine();

            }
            if (pendingTasks.Count == 0)
            {
                Console.Out.WriteLine();
                Console.Out.Write(srEl[0]+" ");
                Console.Out.Write(srEl[1] + " ");
                Console.Out.WriteLine();
            }
        }
    }



    [Serializable]
    public class Task
    {
        public int start = 0;
        public int stop = 0;
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
