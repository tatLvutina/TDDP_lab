using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace SortLibrary
{
    public class SharedObject : MarshalByRefObject
    {
        int minim = 10000;
        int maxim = 0;
       int  max = 0;
       int  min = 100000;
        const int dataCount = 100; // Кол-во элементов в массиве
        const int tasksCount = 2; // максимальное кол-во задач

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
            int k1 = 0;
            int k2 = 5;

            for (int j=0; j<2; j++)
            {
               
                temp = new Task();

                for (int i = k1; i < k2; i++)
                {
                    temp.indexes.Add(i);
                }
                pendingTasks.Enqueue(temp);
                k1 += 5;
                k2 += 5;



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

            for (int i = task.start; i < task.stop; i++)
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

        public void Finish(Task task, int max, int min)
        {
          
            Log.Print("Client has finished task");
            lock (dataLock)
            {
                task.maxEl.Add(max);
                task.minEl.Add(min);
            }
            //finishedTasks.Add(task);
            if (pendingTasks.Count == 0)
            {
                Log.Print("Final task has finished");
  
                for (int i = 0; i < tasksCount-1; i++)
                {
                    if (maxim < task.maxEl[i]) maxim = task.maxEl[i];
                }
                
                for (int i = 0; i < tasksCount-1; i++)
                {
                    if (minim > task.minEl[i]) minim = task.minEl[i];
                }

                Console.Write(minim + "  ");
                Console.Write(maxim + "  ");
                Console.WriteLine();
            }
        }
    }

    [Serializable]
    public class Task
    {
        public List<int> maxEl;
        public List<int> minEl;
        public int start = 0;
        public int stop = 5;
        public List<int> indexes;
        public Task()
        {
            indexes = new List<int>();
            maxEl = new List<int>();
            minEl = new List<int>();
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
