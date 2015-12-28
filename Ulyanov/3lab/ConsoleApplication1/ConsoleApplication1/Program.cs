//Используемые директивы
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Ccr.Core;
using System.Diagnostics;
using System.IO;

//Консольное приложение
namespace ConsoleApplication1
{

    class Program
    {

        static public int mFunc(int x)
        {

            return x * x - 2 * x + 2;
        }


        static public int PerformIntegral(int a, int b, int n)
        {



            int res = 0;
            //число для прогонки цикла, т.к интеграл высчитывается слишком быстро и время 0ms;
            for (int i = 0; i < 1000000; i++)
            {
                res = ((mFunc(b) - mFunc(a)) * n) + (b - a);
            }

            return res;
        }



        //Класс-задание
        public class InputData
        {
            public string str;
            public int start = 0;   //нач.диап
            public int stop = 0;    //кон.диап.
            public int n = 0;
            public int result = 0;

        }

        //Функция-нить исполнения
        //Принимает в виде аргумента контейнер с заданием, 
        //сортирует часть массива
        //Посылает сообщение в порт
        static public int mRes = 0;

        static void task(InputData d, Port<int> resp)
        {

            d.result = PerformIntegral(d.start, d.stop, d.n);
            Console.WriteLine(d.str);
            Console.WriteLine("Начало: " + d.start + " конец: " + d.stop);
            Console.WriteLine("Результат на текущем шаге: " + d.result);
            mRes = mRes + d.result; //формируем общий результат

            resp.Post(1);
        }




        static void ParPerformInegral(int a, int b, int n)
        {
            int a1 = a;
            int b1 = b;



            InputData data1 = new InputData();
            InputData data2 = new InputData();



            data1.str = "Нить исполнения 1";


            data1.start = a;
            data1.stop = b / 2 + 1;
            data1.n = n;


            //вместо performInegral(a,b/2+1,n);


            data2.str = "Нить исполнения 2";

            data2.start = b / 2 + 1;
            data2.stop = b;
            data2.n = n;
            //вместо performInegral(b/2+1,b,n);


            //Создаём диспетчеры с пулом из 2 потоков
            Dispatcher d = new Dispatcher(2, " Test Pool");
            DispatcherQueue dq = new DispatcherQueue(" Test Queue", d);

            //Описываем (определяем) порт, в который каждый экземпляр метода отправляет сообщение после завершения вычислений
            Port<int> p = new Port<int>();

            //Метод Arbiter.Activate помещает в очередь диспетчера две задачи (два экземпляра метода)
            //Первый параметр метода Arbiter.Activate – очередь диспетчера,
            //который будет управлять выполнением задачи, второй параметр – запускаемая задача.
            Arbiter.Activate(dq, new Task<InputData, Port<int>>(data1, p, task));
            Arbiter.Activate(dq, new Task<InputData, Port<int>>(data2, p, task));

            return;
        }


        //Главная функция
        static void Main(string[] args)
        {

            int a = 0, b = 5, n = 3;
            int res1 = 0;



            Console.WriteLine("Начало работы программы");

            Console.WriteLine("Область интегрирования от: " + a + " до " + b + ", шаг " + n);
            Console.WriteLine("");


            Stopwatch sWatch = new Stopwatch();

            Console.WriteLine("");
            Console.WriteLine("Параллельный метод: ");
            sWatch.Start();
            ParPerformInegral(a, b, n);
            sWatch.Stop();



            Console.ReadKey();


            Console.WriteLine("Время работы параллельного вычисления интеграла: " + sWatch.ElapsedMilliseconds);
            Console.WriteLine("Результат: " + mRes);

            Stopwatch sWatch1 = new Stopwatch();

            sWatch1.Start();
            res1 = PerformIntegral(a, b, n);
            sWatch1.Stop();
            Console.WriteLine("");
            Console.WriteLine("Последовательный метод:");
            Console.WriteLine("Время работы ппоследовательного вычисления интеграла: " + sWatch1.ElapsedMilliseconds);
            Console.WriteLine("Результат: " + res1);
            //Указание окончания выполнения программы
            Console.WriteLine("Завершение работы программы");
            Console.ReadKey();

        }
    }
}