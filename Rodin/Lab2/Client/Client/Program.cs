//Клиент

using System;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using RemoteBase;

namespace Remoting
{
    class Program
    {
        //Алгоритм быстрой сортировки части массива
        //Значения left and right - диапазон сортируемого массива
        public static void qsort(int[] arr_q, int left, int right)
        {
            int l = left;
            int r = right;
            int temp = 0;
            //Установка опорного значения
            int middle = arr_q[(l + r) / 2];
            //Разделение массива на три части
            while (l <= r)
            {
                //Все элементы меньше опорного
                while ((arr_q[l] < middle) && (l <= right))
                {
                    l++;
                }
                //Все элементы больше опорного
                while ((arr_q[r] > middle) && (r >= left))
                {
                    r--;
                }
                //Все элементы равные опорному
                if (l <= r)
                {
                    temp = arr_q[l];
                    arr_q[l] = arr_q[r];
                    arr_q[r] = temp;
                    l++;
                    r--;
                }
            }
            //Сортировка левой части
            if (r > left)
            {
                qsort(arr_q, left, r);
            }
            //Сортировка правой части
            if (l < right)
            {
                qsort(arr_q, l, right);
            }

            return;
        }

        //Главная функция
        static void Main()
        {
            //Задержка времени для запуска сервера
            Console.WriteLine("Нажмите Enter после запуска сервера");
            Console.ReadLine();

            //Регистрирование канала
            ChannelServices.RegisterChannel(new TcpClientChannel(), true);
            //Создание объекта определенного нами типа
            Interface obj = (Interface)Activator.GetObject(typeof(Interface), "tcp://localhost:8086/Hi");

            //Проверка на создание объекта класса
            //В случае его отсутствия выходим из программы
            if (obj == null)
            {
                Console.WriteLine("Не удается найти сервер");
                return;
            }

            Console.WriteLine("Соединение установлено");

            //Цикл повторения запроса задания от сервера
            //Длится пока не завершена сортировка массива,
            //т.е. не будут выбраны все задания из списка, хранящегося на сервере
            do
            {
            Console.WriteLine("---------------------------------------------");
            Console.WriteLine("Для получения задания с сервера нажмите ENTER");
            Console.ReadLine();
            //Копирование массива с сервера на клиент
            int[] arr = obj.interfaceArr;
            //Получение индекса начала обрабатываемого клиентом отрезка
            //(номера начала достаточно для определения обрабатываемого отрезка)
            int left = obj.GetTask();
            //Инициализация индекса конца отрезка
            int right = new int();
            if (left == 0)
                right = obj.CountElementsArray() / 2;
            else
            {
                if (left == obj.CountElementsArray() / 2)
                    right = obj.CountElementsArray() - 1;
                else
                {
                    if (left == obj.CountElementsArray() / 4)
                        right = (3 * obj.CountElementsArray()) / 4;
                }
            }
            Console.WriteLine("Получено задание " + obj.numberTask);
            //Если не получено последнее задание
            if (left != -1)
            {
                //Выполняется сортировка части массива
                qsort(arr, left, right);
                Console.WriteLine("Задание выполнено");
                //Результаты возвращаются на сервер
                obj.RewriteInterfaceArr(arr, left, right);
                Console.WriteLine("Результаты возвращены на сервер");
            }
            else
            {
                Console.WriteLine("Сортировка массива завершена");
            }
            } while (obj.numberTask != 8);

            //Вызов метода вывода отсортированного массива на зкран
            obj.PrintInterfaceArray();

            //Завершение работы программы
            Console.WriteLine("Нажмите кнопку для завершения работы клиента");
            Console.ReadLine();
        }
    }
}

