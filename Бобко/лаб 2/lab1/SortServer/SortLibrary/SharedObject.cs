using System;
using System.Collections.Generic;

namespace SortLibrary
{
    public class SharedObject : MarshalByRefObject
    {
    
        static int number;
        int minim = 10000;
        int maxim = 0;

        public List<int> maxes = new List<int>();
        public List<int> mins = new List<int>();

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

            int step = dataCount / tasksCount; // на каждую задачу приходится равная порция массива

            for (int i = 0; i < tasksCount; i++)
            {
                temp = new Task();
                temp.start = i * step;
                temp.stop = temp.start + step - 1;
                pendingTasks.Enqueue(temp);
            }


            //int k1 = 0;
            //int k2 = 5;

            //for (int j=0; j<2; j++)
            //{
               
            //    temp = new Task();

            //    for (int i = k1; i < k2; i++)
            //    {
            //        temp.indexes.Add(i);
            //    }
            //    pendingTasks.Enqueue(temp);
            //    k1 += 5;
            //    k2 += 5;



            //}
        }

        void GenerateData()
        {
            Random r = new Random();
            dataArray = new int[dataCount];

            for (int i = 0; i < dataCount; i++)
                dataArray[i] = r.Next(0, dataCount * tasksCount);
        }

        static int[] Sort(int[] buff)
        {
            //проверка длинны массива
            //если длина равна 1, то возвращаем массив, 
            //так как он не нуждается в сортировке
            if (buff.Length > 1)
            {
                //массивы для хранения половинок входящего буфера
                int[] left = new int[buff.Length / 2];
                //для проверки ошибки некорректного разбиения массива,
                //в случае если длина непарное число
                int[] right = new int[buff.Length - left.Length];

                //заполнение субмассивов данными из входящего массива
                for (int i = 0; i < left.Length; i++)
                {
                    left[i] = buff[i];
                }
                for (int i = 0; i < right.Length; i++)
                {
                    right[i] = buff[left.Length + i];
                }
                //если длина субмассивов больше еденици,
                //то мы повторно (рекурсивно) вызываем функцию разбиения массива
                if (left.Length > 1)
                    left = Sort(left);
                if (right.Length > 1)
                    right = Sort(right);
                //сортировка слиянием половинок
                buff = MergeSort(left, right);
            }
            //возврат отсортированного массива
            return buff;
        }

        static int[] MergeSort(int[] left, int[] right)
        {

            //буфер для отсортированного массива
            int[] buff = new int[left.Length + right.Length];
            //счетчики длины трех массивов
            int i = 0;  //соединенный массив
            int l = 0;  //левый массив
            int r = 0;  //правый массив
            //сортировка сравнением элементов
            for (; i < buff.Length; i++)
            {
                //если правая часть уже использована, дальнейшее движение происходит только в левой
                //проверка на выход правого массива за пределы
                if (r >= right.Length)
                {
                    buff[i] = left[l];
                    l++;
                }
                //проверка на выход за пределы левого массива
                //и сравнение текущих значений обоих массивов
                else if (l < left.Length && left[l] < right[r])
                {
                    buff[i] = left[l];
                    l++;
                }
                //если текущее значение правой части больше
                else
                {
                    buff[i] = right[r];
                    r++;
                    //подсчет количества инверсий
                    if (l < left.Length)
                        number += left.Length - l;
                }
            }
            //возврат отсортированного массива
            return buff;
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
                //return (pendingTasks.Count == 0 ? null : pendingTasks.Dequeue());
            }
        }

        public void Finish(Task task, int[] data)
        {
          
            Log.Print("Client has finished task");
            lock (dataLock)
            {
                int j = 0;
                for (int i = task.start; i < task.stop; i++)
                {
                    
                    dataArray[i] = data[j];
                    j++;
                    Console.Out.Write(dataArray[i]+"  ");
                    
                }
                Console.Out.WriteLine();

            }
            //finishedTasks.Add(task);
            if (pendingTasks.Count == 0)
            {
                Log.Print("Final task has finished");
                Console.Out.WriteLine("\n\n");
                Console.Out.WriteLine("Sorting an array(Client)");
                for (int i = 0; i < 100; i++)
                {
                    Console.Out.Write(dataArray[i] + " ");
                }
                Console.Out.WriteLine("\n\n");
                dataArray = Sort(dataArray);

                Console.Out.WriteLine("Sorting an array(MERGE on server)");
                for (int i = 0; i < 100; i++)
                {
                   Console.Out.Write( dataArray[i]+" ");
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
      //  public List<int> indexes;
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
