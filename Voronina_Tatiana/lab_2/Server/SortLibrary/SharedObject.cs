using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;

namespace SortLibrary
{
    public class SharedObject : MarshalByRefObject
    {
        int minim = 10000;
        int maxim = 0;

        public List<int> maxes = new List<int>();
        public List<int> mins = new List<int>();

        const int dataCount = 25; // Кол-во элементов в массиве
        const int tasksCount = 4; // максимальное кол-во задач

        Queue<Task> pendingTasks; // очередь задач ожидающих обработки
        Object tasksLock;

        //List<Task> finishedTasks;

        int[] dataArray;
        int[] MinMax;
        Object dataLock;

        public SharedObject()
        {
            Log.Print("Создание задачи и данных");
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
            int k2 = 9;

            for (int j=0; j < 4; j++)
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
            Log.Print("Клиент получил задачу");
            int[] temp = new int[task.stop-task.start];

            for (int i = task.start; i < task.stop; i++)
                temp[i] = dataArray[task.indexes[i]];

            return temp;
        }

        public Task GetTask()
        {
            Log.Print("Клиент запросил задачу");
            lock (tasksLock)
            {
                if (pendingTasks.Count == 0)
                {
                    Log.Print("Задач для выполнения нет");
                    return null;
                }
                else
                    return pendingTasks.Dequeue();
                //return (pendingTasks.Count == 0 ? null : pendingTasks.Dequeue());
            }
        }

        public void Finish(Task task, int max, int min)
        {
          
            Log.Print("Клиент выполнил задание");
            lock (dataLock)
            {
                maxes.Add(max);
                mins.Add(min);
            }
            //finishedTasks.Add(task);
            if (pendingTasks.Count == 0)
            {
                Log.Print("конечная задача выполнена");
  
                for (int i = 0; i < tasksCount; i++)
                {
                    if (maxim < maxes[i]) maxim = maxes[i];

                    if (minim > mins[i]) minim = mins[i];
                }
                Log.Print("Конечные значения \tMax: " + maxim + ". Min: " + minim );
                File.WriteAllText("E:\\new_file.txt", "Max: " + maxim + ". Min: " + minim);
            }
        }
    }

    [Serializable]
    public class Task
    {
        public List<int> maxEl; //Список максимальных значений
        public List<int> minEl; //Список минимальных значений
        public int start = 0; //Начальныйэлемент массива
        public int stop = 9; //Конечный элемент массива
        public List<int> indexes; //Список индексов элементов массива

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
