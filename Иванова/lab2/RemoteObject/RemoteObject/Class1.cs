using System;
using System.Collections.Generic;

namespace SearchClasses
{
    public class RemoteObject : MarshalByRefObject
    {
        static int[] result = new int[2];
        static int[] subresult = new int[4];
        const int arrSize = 100; // Исходный массиве
        const int tasksCount = 2; // Количество задач
        private object manageLock;
        public bool managed = false;
        public int idCount = 0;
        Queue<Task> TaskList; // очередь задач
        Object tasksLock;

        //List<Task> finishedTasks;

        int[] dataArray;

        Object dataLock;

        public RemoteObject()
        {
            TimerOutput.Print("Инициализация");
            TaskList = new Queue<Task>();
            tasksLock = new Object();
            dataLock = new Object();
            manageLock = new object();
            
        }
        public bool joinToServer()
        {
            lock (manageLock)
            {
                if (managed)
                {
                    idCount++;
                    return true;
                }
                else return false;
            }
        }
        public bool setManagingClient()
        {
            lock (manageLock)
            {
                if (!managed)
                {
                    managed = true;
                    return managed;
                }
                else 
                {TimerOutput.Print("Задания не сформированы");
                return false;}
            }
        }
        public void GenerateTasks()
        {
            Task temp;

            int step = arrSize / tasksCount; // на каждую задачу приходится равная порция массива

            for (int i = 0; i < tasksCount; i++)
            {
                temp = new Task();
                temp.start = i * step;
                temp.stop = temp.start + step - 1;
                TaskList.Enqueue(temp);
            }
        }

        public void Create()
        {
            Random r = new Random();
            dataArray = new int[arrSize];

            for (int i = 0; i < arrSize; i++)
                dataArray[i] = r.Next(0, arrSize * tasksCount);
        }
        public int[] getData(Task task)
        {
            TimerOutput.Print("Клиент запросил данные ");
            int[] temp = new int[task.stop - task.start];
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
            TimerOutput.Print("Запрос задачи");
            lock (tasksLock)
            {
                if (TaskList.Count == 0)
                {
                    TimerOutput.Print("нет готовых задач");
                    return null;
                }
                else
                    return TaskList.Dequeue();
             }
        }

        public void Complete(int[] res)
        {

            TimerOutput.Print("Клиент выполнил задачу");
            lock (dataLock)
            {
                subresult[idCount] = res[0];
                subresult[idCount+1] = res[1];
                Console.Out.WriteLine();

            }
             if (TaskList.Count == 0)
            {
                result[0] = subresult[0];
                for (int i = 0; i < 4; i++)
                {
                    if (subresult[i] < result[0])
                        result[0] = subresult[i];
                }
                result[1] = subresult[0];
                for (int i = 0; i < 4; i++)
                {
                    if (subresult[i] > res[0])
                        result[0] = subresult[i];
                }
                Console.Out.WriteLine();
                Console.Out.Write(result[0] + " ");
                Console.Out.Write(result[1] + " ");
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

    public class TimerOutput
    {
        public static void Print(String msg)
        {
            System.Console.WriteLine("[" + DateTime.Now.Hour.ToString() + ":" +
                DateTime.Now.Minute.ToString() + ":" + DateTime.Now.Second.ToString()
            + "] " + msg);
        }
    }
}
