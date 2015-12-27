//Библиотека

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RemoteBase
{
    //Класс, определяющий взаимодействие клиента и сервера
    public class Interface : System.MarshalByRefObject
    {
        //Количество элементов в сортируемом массиве
        public const int N = 25;
        //Сортируемый массив чисел
        public int[] interfaceArr;
        //Кол-во обработанных заданий (последнее выполненное задание)
        //Для диспетчеризации заданий и для контроля завершения сортировки массива
        public int numberTask;
        //Объекты, используемые для контроля доступа к критической секции при запросе заданий с сервера
        public object DispetcherLock;
        //Объект, контролирующий доступ к критической секции перезаписи массива
        public object RewriteLock;
        //Переменная для задержки выдачи заданий, пока не завершится обработка текущей
        public bool StopGetTask;
        //Массив заданий (хранит номера первых элементов обрабатываемых отрезков массива)
        public int[] task;

        //Конструктор класса Interface
        public Interface()
        {
            Console.WriteLine("Вызван конструктор класса Interface");
            //Выделение памяти под массив
            interfaceArr = new int[N];
            //Инициализация массива случайными числами
            Random r = new Random();
            Console.WriteLine("Сгенерирован следующий массив чисел:");
            for (int i = 0; i < N; i++)
            {
                interfaceArr[i] = r.Next(90) + 10;
                Console.Write(interfaceArr[i] + " ");
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
        //Возвращает номер первого элемента отрезка массива; 
        //Если отрезок последний в текущем проходе блокирует выдачу следующих заданий до завершения текущей обработки.
        public int GetTask()
        {
            //Оператор lock предусматривает возможность доступа к критической секции кода
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
        //(возвращает результаты обработки задания на сервер)
        //В качестве аргументов принимает данные об отрезке, который необходимо переписать в массиве на сервере.
        public void RewriteInterfaceArr(int[] arr, int left, int right)
        {
            //Критическая секция
            lock (RewriteLock)
            {
                for (int i = left; i < right; i++)
                    interfaceArr[i] = arr[i];
                if (right > N / 2)
                    StopGetTask = false;
            }
        }

        //Метод вывода текущего содержимого массива на экран
        public void PrintInterfaceArray()
        {
            Console.WriteLine("Текущие значения массива:");
            for (int i = 0; i < N; i++)
                Console.Write(interfaceArr[i] + " ");
            Console.WriteLine();
        }

        //Метод, возвращающий число элементов в массиве (длина)
        public int CountElementsArray()
        {
            return N;
        }
    }
}



