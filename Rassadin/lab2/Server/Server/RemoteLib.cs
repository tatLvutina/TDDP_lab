using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RemoteBase
{
    //Класс, определяющий взаимодействие клиента и сервера
    public class Box : System.MarshalByRefObject
    {
        public const int N = 25;
        public int[] boxArr;
        public int numberTask;
        public object DispetcherLock;
        public object RewriteLock;
        public bool StopGetTask;
        public bool ready;

        /*public class Task
        {
            //
            public int left;
            public int right;
            //
            public Task()
            {
                left = new int();
                right = new int();
                left = 0;
                right = 0;
            }
            //
            public Task(int l, int r)
            {
                left = new int();
                right = new int();
                left = l;
                right = r;
            }
            //
            public void InitTask(int l, int r)
            {
                left = l;
                right = r;
            }
        }*/

        //
        //public Task[] task;
        public int[] task;

        //Конструктор класса Box
        public Box()
        {
            Console.WriteLine("Вызван конструктор класса Box");
            //Выделение памяти под массив
            boxArr = new int[N];
            //Заполнение массива случайными числами
            Random r = new Random();
            Console.WriteLine("Сгенерирован следующий массив чисел:");
            for (int i = 0; i < N; i++)
            {
                boxArr[i] = r.Next(90) + 10;
                Console.Write(boxArr[i] + " ");
            }
            boxArr[N - 1] = 98;
            Console.WriteLine();
            //
            numberTask = new int();
            numberTask = 0;
            //
            task = new int[8];
            task[0] = 0;
            task[1] = N / 2; ;
            task[2] = N / 4;
            task[3] = 0;
            task[4] = N / 2;
            task[5] = N / 4;
            task[6] = 0;
            task[7] = N;
            //
            /*task[0] = new Task(0, N / 2);
            task[1] = new Task(N / 2, N);
            task[2] = new Task(N / 4, (N * 3) / 4);
            task[3] = new Task(0, N / 2);
            task[4] = new Task(N / 2, N);
            task[5] = new Task(N / 4, (N * 3) / 4);
            task[6] = new Task(0, N / 2);
            task[7] = new Task(N / 2, N);*/
            /*task[0].left = 0;
            task[0].right = N / 2;
            task[1].left = N / 2;
            task[1].right = N;
            task[2].left = N / 4;
            task[2].right = (N * 3) / 4;
            task[3].left = 0;
            task[3].right = N / 2;
            task[4].left = N / 2;
            task[4].right = N;
            task[5].left = N / 4;
            task[5].right = (N * 3) / 4;
            task[6].left = 0;
            task[6].right = N / 2;
            task[7].left = N / 2;
            task[7].right = N;*/
            //
            DispetcherLock = new object();
            RewriteLock = new object();
            ready = new bool();
            StopGetTask = new bool();
            ready = false;
            StopGetTask = false;
        }

        public int GetTask()
        {
            lock (DispetcherLock)
            {
                while (StopGetTask) ;
                if (numberTask == 8)
                {
                    ready = true;
                    return -1;
                }
                if ((numberTask % 3) != 0)
                    StopGetTask = true;
                return task[numberTask++];
            }
        }

        public void RewriteBoxArr(int[] arr, int left, int right)
        {
            lock (RewriteLock)
            {
                for (int i = left; i < right; i++)
                    boxArr[i] = arr[i];
                if (right > N / 2)
                    StopGetTask = false;
            }
        }

        public void PrintBoxArray()
        {
            Console.WriteLine("Текущие значения массива:");
            for (int i = 0; i < N; i++)
                Console.Write(boxArr[i] + " ");
            Console.WriteLine();
        }

        public int CountElementsArray()
        {
            return N;
        }
    }
}



