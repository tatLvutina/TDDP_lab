using System;
using System.Collections.Generic;

namespace SortLibrary
{
    public class SharedObject : MarshalByRefObject
    {

        static int i;
        public static int m = 4; //кол-во строк
        public static int n = 4; //кол-во столбцов

        const int dataCount = 4; //всего строк в матрице
        const int tasksCount = 2;  //максимальное число задач

        Queue<Task> QueTasks; // очередь задач ожидающих обработки
        Object tasksLock;

        public int[,] A = new int[m,n];    //матрица для умножения
        public int[] B = new int[n];   //вектор-столбец для умножения
        public int[] C = new int[m];   //хранение результата
        Object dataLock;

        public SharedObject()
        {
            QueTasks = new Queue<Task>();
            CreateData();
            CreateTasks();

            tasksLock = new Object();
            dataLock = new Object();
        }

        void CreateTasks()
        {
            ServerConsole.Print("\n\nСоздание задач...\n");
            Task temp;

            //распределение массива поровну на каждого клиента
            int clientPortion = dataCount / tasksCount;
            ServerConsole.Print("Всего строк в матрице:{0}",dataCount);
            ServerConsole.Print("Клиентов:{0}",tasksCount);
            ServerConsole.Print("Кол-во строк на клиента:{0}",clientPortion);           

            for (int i = 0; i < tasksCount; i++)
            {
                temp = new Task();
                ServerConsole.Print("\nИнициализация счетчика умножаемых строк для клиента #{0}",i+1);
                temp.start = i * clientPortion;              
                ServerConsole.Print("Начальная строка: {0}",temp.start+1);
                temp.stop = temp.start + clientPortion - 1;  
                ServerConsole.Print("Конечная строка: {0}", temp.stop+1);
                QueTasks.Enqueue(temp);                      //добавление задачи в конец очереди
            }
            ServerConsole.Print("\nЗадачи успешно созданы и распределены!");
        }

        void CreateData()
        {
            ServerConsole.Print("\n\n\nИсходные данные:");
            //***ввод тестовых значений для проверки правильности вычислений
            Console.Out.WriteLine("Матрица A:");
            int k = 1;
            for (int i = 0; i < m; i++)
            {
                Console.Out.WriteLine();
                for (int j = 0; j < n; j++)
                {
                    A[i, j] = k;
                    k++;
                    Console.Out.Write("{0}\t", A[i, j].ToString());
                }
            }

            Console.Out.WriteLine("\n\nВектор-столбец B:");
            k = 1;
            for (int j = 0; j < n; j++)
            {
                B[j] = k;
                k++;
                Console.Out.Write("{0}\n", B[j].ToString());
            }
            //***


            // заполнение случайными значениями
            //ServerConsole.Print("Заполнение случайными значениями...\n");
            //Random r = new Random();
            //for (int i = 0; i < m; i++)
            //{
            //    for (int j = 0; j < n; j++)
            //        A[i, j] = r.Next(100);
            //}
            //for (int j = 0; j < n; j++)
            //    B[j] = r.Next(100);

            //ServerConsole.Print("Исходная матрица и вектор-столбец успешно заполнены случайными значениями!\n");

        }



        public void GetData(Task task, out int[] Btemp, out int[,] Atemp)
        {           
            Atemp = new int[m, n];
            Btemp = new int[n];
            ServerConsole.Print("\nКлиент начал получение данных для обработки!");
            Console.Out.WriteLine("A[m,n] передаваемое клиенту:");
            for (int i = task.start; i <= task.stop; i++)
            {
                Console.Out.WriteLine();
                for (int j = 0; j < n; j++)
                {
                    Atemp[i, j] = A[i, j];
                    Console.Out.Write("{0}\t", Atemp[i, j].ToString());
                }

            }
            Console.Out.WriteLine("\n\nB[n] передаваемое клиенту:");
            for (int j = 0; j < n; j++)
            {
                Btemp[j] = B[j];
                Console.Out.Write("{0}\n", Btemp[j].ToString());
            }
            ServerConsole.Print("Клиент получил данные для обработки!\n\n");
        }

        public Task GetTask()
        {
            ServerConsole.Print("\nКлиент запросил задачу");
            lock (tasksLock)
            {
                if (QueTasks.Count == 0) //если задачи кончились
                {
                    ServerConsole.Print("Больше нет задач..."); //сообщим об этом
                    return null;
                }
                else
                    return QueTasks.Dequeue(); //если еще не кончились - вернем следующую задачу, извлеченную из очереди
            }
        }

        public void Finish(int[] mas)
        {
            lock (dataLock)
            {
                for (int i = 0; i < n; i++)
                {
                    C[i] += mas[i];
                }
                ServerConsole.Print("\nКлиент успешно завершил задачу!");
            }

            if (QueTasks.Count == 0)
            {
                Console.Out.Write("\n\nПолученный результат:\n");
                for (i = 0; i < m; i++)
                    Console.Out.WriteLine("C[{0}]: {1}", i + 1, C[i].ToString());
            }
        }
    }



    [Serializable]
    public class Task
    {
        public int start = 0, stop = 0;  //определение начала и конца диапазона
    }
    public class ServerConsole //вывод записи в консоли на сервере
    {
        //public static void Print(String msg, params int[] values)  //используется ключевое слово params для передачи неопределенного числа параметров в функцию
        //{
        //    Console.WriteLine(msg, values);
        //}

        //на случай проблем в работе params перегрузим метод Print
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

