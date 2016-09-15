using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace SortLibrary
{
    public class SharedObject : MarshalByRefObject
    {
        const int dataCount = 10; // Кол-во элементов в массиве
        const int tasksCount = 2; // максимальное кол-во задач
        const int partSize = dataCount / tasksCount;

        Queue<Task> pendingTasks; // очередь задач ожидающих обработки
        Object tasksLock;

        //List<Task> finishedTasks;

        int[] dataArray;
        int[] MinMax;
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


                temp = new Task();

                for (int i = 0; i < dataCount; i=dataCount)
                {
                    temp.indexes.Add(i);
                }
                pendingTasks.Enqueue(temp);

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

            }
        }
    }

    [Serializable]
    public class Task
    {

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
