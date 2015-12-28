using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RemoteBase
{
    //Класс, определяющий взаимодействие клиента и сервера
    public class Interface : System.MarshalByRefObject
    {
        public const int N = 25;
        public int[] interfaceArr;
        public int numberTask;
        public object DispetcherLock;
        public object RewriteLock;
        public bool StopGetTask;
        public bool ready;

      
        //public Task[] task;
        public int[] task;

        //Конструктор класса Interface
        public Interface()
        {
            Console.WriteLine("Вызван конструктор класса Interface");
            //Выделение памяти под массив
            interfaceArr = new int[N];
            //Заполнение массива случайными числами
            Random r = new Random();
            Console.WriteLine("Сгенерирован следующий массив чисел:");
            for (int i = 0; i < N; i++)
            {
                interfaceArr[i] = r.Next(90) + 10;
                Console.Write(interfaceArr[i] + " ");
            }
            interfaceArr[N - 1] = 98;
            Console.WriteLine();
            
            numberTask = new int();
            numberTask = 0;
          
            task = new int[8];
            task[0] = 0;
            task[1] = N / 2; ;
            task[2] = N / 4;
            task[3] = 0;
            task[4] = N / 2;
            task[5] = N / 4;
            task[6] = 0;
            task[7] = N;
           
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

        public void RewriteInterfaceArr(int[] arr, int left, int right)
        {
            lock (RewriteLock)
            {
                for (int i = left; i < right; i++)
                    interfaceArr[i] = arr[i];
                if (right > N / 2)
                    StopGetTask = false;
            }
        }

        public void PrintInterfaceArray()
        {
            Console.WriteLine("Текущие значения массива:");
            for (int i = 0; i < N; i++)
                Console.Write(interfaceArr[i] + " ");
            Console.WriteLine();
        }

        public int CountElementsArray()
        {
            return N;
        }
    }
}



