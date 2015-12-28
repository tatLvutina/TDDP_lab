using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RemoteBase
{
    //Класс, определяющий взаимодействие клиента и сервера
    public class Box : System.MarshalByRefObject
    {
        //Количество элементов в массиве
        public const int N = 25;
        //Массива
        public int[] boxArr;
        //Кол-во обработанных заданий
        public int numberTask;
        //Объекты, используемые для контроля доступа к
        //критической секции
        public object DispetcherLock;
        public object RewriteLock;
        //Переменная для задержки выдачи заданий
        public bool StopGetTask;
        //Массив заданий
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
            Console.WriteLine();
            //Число выполненых заданий
            numberTask = new int();
            numberTask = 0;
            //Инициализация массива заданий
            task = new int[8];
            task[0] = 0;
            task[1] = N / 2; ;
            task[2] = N / 4;
            task[3] = 0;
            task[4] = N / 2;
            task[5] = N / 4;
            task[6] = 0;
            task[7] = N;
            //Начальная инициализация
            DispetcherLock = new object();
            RewriteLock = new object();
            StopGetTask = new bool();
            StopGetTask = false;
        }

        //Метод выдачи очередного задания на выполнение
        public int GetTask()
        {
            lock (DispetcherLock)
            {
                while (StopGetTask) ;
                if (numberTask == 8)
                {
                    return -1;
                }
                if ((numberTask % 3) != 0)
                    StopGetTask = true;
                return task[numberTask++];
            }
        }

        //Метод перезаписи массива на сервере
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

        //Метод вывода массива на экран
        public void PrintBoxArray()
        {
            Console.WriteLine("Текущие значения массива:");
            for (int i = 0; i < N; i++)
                Console.Write(boxArr[i] + " ");
            Console.WriteLine();
        }

        //Метод, возвращающий число элементов в массиве
        public int CountElementsArray()
        {
            return N;
        }
    }
}



