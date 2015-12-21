using System;
using System.Collections.Generic;

namespace SortLibrary
{
    public class SharedObject : MarshalByRefObject
    {
        const int dataCount = 10;  // всего задач
        const int tasksCount = 2;   // на сколько частей делятся задачи (иными словами, это - кол-во клиентов, необходмых для цельного выполнения работы)
        public static int clientPortion = dataCount / tasksCount;
        Queue<Task> QueTasks; // храним задачи, ожидающие обработку
        Object tasksLock;

        public List<int> maxes = new List<int>();
        public List<int> mins = new List<int>();

        int[] dataArray;
        Object dataLock;

        public SharedObject()
        {
            QueTasks = new Queue<Task>();
            GenerateData();
            GenerateTasks();

            tasksLock = new Object();
            dataLock = new Object();
        }

        void GenerateTasks()
        {
            ourServer.Print("Распределение задач...");
            Task temp;

            
            ourServer.Print("Всего элементов в матрице:{0}", dataCount);
            ourServer.Print("Клиентов:{0}", tasksCount);
            ourServer.Print("Кол-во элементов на клиента:{0}", clientPortion);

            for (int i = 0; i < tasksCount; i++)
            {
                temp = new Task();
                ourServer.Print("\nИнициализация счетчика элементов, выделенных для клиента #{0}", i + 1);
                temp.start = i * clientPortion;
                ourServer.Print("Индекс начального элемента: {0}", temp.start + 1);
                temp.stop = temp.start + clientPortion - 1;
                ourServer.Print("Индекс конечного элемента: {0}", temp.stop + 1);
                QueTasks.Enqueue(temp);                      //добавление задачи в конец очереди
            }
            ourServer.Print("\nЗадачи успешно распределены!");
        }

        void GenerateData()
        {
            ourServer.Print("Заполнение массива случайными элементами...");
            Random r = new Random();
            dataArray = new int[dataCount];

            for (int i = 0; i < dataCount; i++)
            {
                dataArray[i] = r.Next(100);
                Console.Out.Write("{0}\t", dataArray[i].ToString());
            }
        }

        static int[] Sort(int[] temp)
        {
            for (int i = 0; i <= temp.Length - 1; i++)
            {
                for (int j = i + 1; j < temp.Length; j++)
                {
                    if (temp[j] < temp[i])
                    {
                        var spam = temp[i];
                        temp[i] = temp[j];
                        temp[j] = spam;
                    }
                }
            }
            return temp;
        }

        public int[] GetData(Task task)
        {
            ourServer.Print("Клиент получил следующие данные для обработки:");
            int[] temp = new int[clientPortion];
            int j = 0;
            for (int i = task.start; i <= task.stop; i++)
            {
                temp[j] = dataArray[i];
                Console.Out.Write("{0}\t", dataArray[i].ToString());
                j++;
            }
            return temp;
        }

        public Task GetTask()
        {
            ourServer.Print("\nКлиент запросил задачу");
            lock (tasksLock)
            {
                if (QueTasks.Count == 0)
                {
                    ourServer.Print("Задач больше нет!");
                    return null;
                }
                else
                    return QueTasks.Dequeue();
            }
        }

        public void Finish(Task task, int[] data)
        {
          
            ourServer.Print("\nКлиент завершил обработку данных");
            lock (dataLock)
            {
                int j = 0;
                for (int i = task.start; i <= task.stop; i++)
                {
                    
                    dataArray[i] = data[j]; //соединяем сортированный части в целый массив
                    j++;
                    Console.Out.Write("{0}\t", dataArray[i].ToString());  //отображаем только часть, обработанную данным клиентом

                }
                Console.Out.WriteLine();
            }

            if (QueTasks.Count == 0)
            {
                ourServer.Print("Все задачи решены");
                Console.Out.WriteLine("\n\n");
                Console.Out.WriteLine("Результаты работы клиентов, помещенные в один массив:");
                for (int i = 0; i < dataCount; i++)
                {
                    Console.Out.Write("{0}\t", dataArray[i].ToString());   //выводим массив, состоящий из двух сортированых половинок
                }
                Console.Out.WriteLine("\n\n");
                dataArray = Sort(dataArray);     //финальная сортировка

                Console.Out.WriteLine("Дополнительна сортировка на сервере:");
                for (int i = 0; i < dataCount; i++)
                {
                    Console.Out.Write("{0}\t", dataArray[i].ToString()); //отображение результирующего, полностью отсортированного массива
                }
                Console.Out.WriteLine();
            }
        }
    }



    [Serializable]
    public class Task
    {
        public int start = 0, stop = 0;
    }

    public class ourServer
    {
        public static void Print(String msg)
        {
            Console.WriteLine(msg);
        }
        public static void Print(String msg, int param1)
        {
            Console.WriteLine(msg, param1);
        }
    }
}
