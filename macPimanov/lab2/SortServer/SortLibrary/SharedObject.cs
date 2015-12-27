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

        const int dataCount = 100; // Количество элементов в массиве
        const int tasksCount = 2; // Максимальное кол-во задач

        Queue<Task> pendingTasks; // Очередь задач, ожидающих обработки
        Object tasksLock;

        //List<Task> finishedTasks;

        int[] dataArray;
        int[] MinMax;
        Object dataLock;

        public SharedObject()
        {
            Log.Print("Создаю задания и данные");
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

            int step = dataCount / tasksCount; // Распределение массива на каждую задачу

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
            // Проверка длины массива
            // Если длина равна 1, то возвращаем массив, так как он не нуждается в сортировке
            if (buff.Length > 1)
            {
                // Массивы для хранения половинок входящего буфера
                int[] left = new int[buff.Length / 2];
                // Для проверки некорректного разбиения массива, в случае если длина - непарное число
                int[] right = new int[buff.Length - left.Length];

                // Заполнение подмассивов данными из входящего массива
                for (int i = 0; i < left.Length; i++)
                {
                    left[i] = buff[i];
                }
                for (int i = 0; i < right.Length; i++)
                {
                    right[i] = buff[left.Length + i];
                }
                // Если длина субмассивов больше единицы, то мы повторно вызываем функцию разбиения массива с применением рекурсии
                if (left.Length > 1)
                    left = Sort(left);
                if (right.Length > 1)
                    right = Sort(right);
                // Сортировка слиянием половинок
                buff = MergeSort(left, right);
            }
            // Возврат отсортированного массива
            return buff;
        }

        static int[] MergeSort(int[] left, int[] right)
        {

            // Буфер для отсортированного массива
            int[] buff = new int[left.Length + right.Length];
            // Счетчики длины трех массивов
            int i = 0;  // Соединенный массив
            int l = 0;  // Левый массив
            int r = 0;  // Правый массив
            // Сортировка сравнением элементов
            for (; i < buff.Length; i++)
            {
                // Если правая часть уже использована, дальнейшее движение происходит только в левой
                // Проверка на выход правого массива за пределы
                if (r >= right.Length)
                {
                    buff[i] = left[l];
                    l++;
                }
                // Проверка на выход за пределы левого массива и сравнение текущих значений обоих массивов
                else if (l < left.Length && left[l] < right[r])
                {
                    buff[i] = left[l];
                    l++;
                }
                else
                {
                    buff[i] = right[r];
                    r++;
                    // Подсчет количества инверсий
                    if (l < left.Length)
                        number += left.Length - l;
                }
            }
            // Возврат отсортированного массива
            return buff;
        }

        public int[] FetchData(Task task)
        {
            Log.Print("Клиент получил задание");
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
            Log.Print("Клиент запросил задание");
            lock (tasksLock)
            {
                if (pendingTasks.Count == 0)
                {
                    Log.Print("Заданий нет");
                    return null;
                }
                else
                    return pendingTasks.Dequeue();
                //return (pendingTasks.Count == 0 ? null : pendingTasks.Dequeue());
            }
        }

        public void Finish(Task task, int[] data)
        {          
            Log.Print("Клиент завершил задание");
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
                Log.Print("Последнее задание завершено");
                Console.Out.WriteLine("\n\n");
                Console.Out.WriteLine("Сортировка массива(Клиент)");
                for (int i = 0; i < 100; i++)
                {
                    Console.Out.Write(dataArray[i] + " ");
                }
                Console.Out.WriteLine("\n\n");
                dataArray = Sort(dataArray);

                Console.Out.WriteLine("Сортировка массива(Сервер)");
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
        //public List<int> indexes;
        //public Task()
        //{
        //    indexes = new List<int>();
        //}
    }

    public class Log
    {
        // Логгирование
        public static void Print(String msg)
        {
            System.Console.WriteLine("[" + DateTime.Now.Hour.ToString() + ":" +
                DateTime.Now.Minute.ToString() + ":" + DateTime.Now.Second.ToString() 
            + "] " + msg);
        }
    }
}
