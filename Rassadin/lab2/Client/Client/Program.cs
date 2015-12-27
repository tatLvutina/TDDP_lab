using System;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using RemoteBase;

namespace Remoting
{
    class Program
    {
        //Метод быстрой сортировки части массива
        public static void qsort(int[] a, int left, int right)
        {
            int l = left;
            int r = right;
            int val = 0;
            int mid = a[(l + r) / 2];

            while (l <= r)
            {
                while ((a[l] < mid) && (l <= right))
                {
                    l++;
                }
                while ((a[r] > mid) && (r >= left))
                {
                    r--;
                }

                if (l <= r)
                {
                    val = a[l];
                    a[l] = a[r];
                    a[r] = val;
                    l++;
                    r--;
                }
            }

            if (r > left)
            {
                qsort(a, left, r);
            }
            if (l < right)
            {
                qsort(a, l, right);
            }

            return;
        }

        static void Main()
        {
            //Задержка времени для запуска сервера
            Console.WriteLine("Нажмите Enter после запуска сервера");
            Console.ReadLine();

            //Регистрирование канала
            ChannelServices.RegisterChannel(new TcpClientChannel(), true);
            Box obj = (Box)Activator.GetObject(typeof(Box), "tcp://localhost:8086/Hi");

            //Если объект класса, предназначеного для взаимодействия сервера и
            //клиента не создан, завершаем работу программы
            if (obj == null)
            {
                Console.WriteLine("Не удается найти сервер");
                return;
            }

            Console.WriteLine("Соединение установлено");

            //Цикл запроса задания от сервера, длящийся до тех пор
            //пока не завершена сортировка массива
            do
            {
            Console.WriteLine("---------------------------------------------");
            Console.WriteLine("Для получения задания с сервера нажмите ENTER");
            Console.ReadLine();
            //Копирование массива с сервера
            int[] arr = obj.boxArr;
            //Получение индекса начала обрабатываемого отрезка
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
                obj.RewriteBoxArr(arr, left, right);
                Console.WriteLine("Результаты возвращены на сервер");
            }
            else
            {
                Console.WriteLine("Сортировка массива завершена");
            }
            } while (obj.numberTask != 8);

            //Вызов метода вывода отсортированного массива на зкран
            obj.PrintBoxArray();

            //Завершение работы программы
            Console.WriteLine("Нажмите кнопку для завершения работы клиента");
            Console.ReadLine();
        }
    }
}

