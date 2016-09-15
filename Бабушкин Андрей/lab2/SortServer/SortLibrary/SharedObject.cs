using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace SortLibrary
{
    public class SharedObject : MarshalByRefObject
    {
        const int dataCount = 100; // Кол-во элементов в массиве
        const int tasksCount = 10; // максимальное кол-во задач
        const int partSize = dataCount / tasksCount;

        Queue<Task> pendingTasks; // очередь задач ожидающих обработки
        Object tasksLock;

        //List<Task> finishedTasks;

        int[] dataArray;
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
            //Random r = new Random(DateTime.Now.Millisecond);
            //pendingTasks = new Queue<Task>();
            //finishedTasks = new List<Task>();

            
            //for (int i = 0; i < tasksCount; i++)
            //{
            //    Task temp = new Task();
            //    temp.data = new int[partSize];

            //    for (int j = 0; j < partSize; j++)
            //        temp.data[j] = r.Next(0, dataCount * tasksCount);

            //    temp.size = partSize;
            //    pendingTasks.Enqueue(temp);
                
            //}
            int h = 1;
            int j = 0;
            Task temp;

            while (h <= dataCount / 3)
                h = h * 3 + 1;

            while (h > 0)
            {
                temp = new Task();

                for (int i = 0; i < dataCount; i += h)
                {
                    temp.indexes.Add(i);
                }
                pendingTasks.Enqueue(temp);
                h = (h - 1) / 3;
            }
        }
        void GenerateData()
        {
            Random r = new Random();
            dataArray = new int[dataCount];

            for (int i = 0; i < dataCount; i++)
                dataArray[i] = r.Next(0, dataCount * tasksCount);
        }

        //void FinalMerge()
        //{
        //    int min; 
        //    int minIndex = 0;
        //    int outer = 0;

        //    Log.Print("Prepare to merging data");
        //    Log.Print("Finished tasks count: " + finishedTasks.Count);
        //    while (outer < dataCount)
        //    {
        //        min = dataCount * tasksCount; // максимально возможное число
        //        for (int i = 0; i < finishedTasks.Count; i++)
        //            if (min > finishedTasks[i].data[0])
        //            {
        //                min = finishedTasks[i].data[0];
        //                minIndex = i;
        //                Log.Print("minIndex: " + minIndex);
        //            }

        //        for (int j = 0; j < partSize; j++)
        //        {
        //            dataArray[outer] = finishedTasks[minIndex].data[j];
        //            outer++;
        //        }
        //        finishedTasks.RemoveAt(minIndex);
        //        Log.Print("Remained tasks: " + finishedTasks.Count);
        //    }

        //    CheckSort();
        //}
        void CheckSort()
        {
            for (int i = 1; i < dataCount; i++)
                if (dataArray[i - 1] > dataArray[i])
                {
                    Log.Print("Check Sort failed! Element #" + i);
                    for (int j = i - 10; j < i + 10; j++)
                        Console.Out.Write(dataArray[j] + "  ");
                    Console.Out.WriteLine();
                   // return;
                }
            Log.Print("Check sort successful");
            PrintAll();
        }

        void PrintAll()
        {
            for (int i = 0; i < dataCount; i++)
                Console.Write(dataArray[i] + "  ");

            Console.WriteLine();
        }

        public int[] FetchData(Task task)
        {
            Log.Print("Client has fetched data");
            int[] temp = new int[task.indexes.Count];

            for (int i = 0; i < task.indexes.Count; i++)
                temp[i] = dataArray[task.indexes[i]];

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

        public void Finish(Task task, int[] data)
        {
            Log.Print("Client has finished task");
            lock (dataLock)
            {
                for (int i = 0; i < task.indexes.Count; i++)
                    dataArray[task.indexes[i]] = data[i];
            }
            //finishedTasks.Add(task);
            if (pendingTasks.Count == 0)
            {
                Log.Print("Final task has finished");
                CheckSort();
            }
        }
    }

    [Serializable]
    public class Task
    {
        //public int start;
        //public int stop;
        //public int[] data;
        //public int size;
        //public List<int> data;
        public List<int> indexes;
        public Task()
        {
            indexes = new List<int>();
        }
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
