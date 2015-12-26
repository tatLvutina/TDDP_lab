using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Ccr.Core;
using System.Threading;

namespace ConsoleApplicationLab3
{
    public class InputData
    {
        public int nachalo; //Начало диапазона 
        public int konec; //Конец диапазона 
        public int i;
    }

    class Program
    {
        static int[] a;
        static int[] b;
        static int n;
        static int nc;

        static void Mul(InputData data, Port<int> resp)
         {
             int i, j;
             System.Diagnostics.Stopwatch sWatch = new System.Diagnostics.Stopwatch();
             sWatch.Start();
             for (i = data.nachalo; i <= data.konec; i++)
             {
                 for (j = i; i <= data.konec; i++)
                 {
                      if (a[j] > a[j+1]) {
                 int b = a[j]; //Обмен элементами
                 a[j] = a[j+1];
                 a[j+1] = b;
                 }
             }
             }
             sWatch.Stop();  
             resp.Post(1);
            

                 Console.WriteLine("Поток # {0}: Параллельный алгоритм = {1} миллисекунд", Thread.CurrentThread.ManagedThreadId, sWatch.ElapsedMilliseconds.ToString());
        }
        

            static void Main(string[] args)
        {
            int i;
            nc = 2;
            n = 100000000;

            a = new int[n];
            b = new int[nc];

            Random r = new Random();
            for (int j = 0; j < n; j++)
                a[j] = r.Next(100);

            System.Diagnostics.Stopwatch sWatch = new System.Diagnostics.Stopwatch();
            sWatch.Start();
            for (i = 0; i <= n; i++)
            {
                for (int j = i; i <= n; i++)
                {
                    if (a[j] > a[j + 1])
                    {
                        int e = a[j]; //Обмен элементами
                        a[j] = a[j + 1];
                        a[j + 1] = e;
                    }
                }
            }



            sWatch.Stop();

            Console.WriteLine("Последовательный алгоритм = {0} мс.", sWatch.ElapsedMilliseconds.ToString());
            


            //Создание массива объектов для хранения параметров 
            InputData[] ClArr = new InputData[nc];
            for (i = 0; i < nc; i++)
                ClArr[i] = new InputData();
            //Делим количество элементов  в массиве на nc частей 
            int shag = (Int32)(n / nc);
            //Заполняем массив параметров 
            int c = -1;
            for (i = 0; i < nc; i++)
            {
                ClArr[i].nachalo = c + 1;
                ClArr[i].konec = c + shag;
                ClArr[i].i = i;
                c = c + shag;
            }
            Dispatcher d = new Dispatcher(nc, "Test Pool");
            DispatcherQueue dq = new DispatcherQueue("Test Queue", d);
            Port<int> p = new Port<int>();


            for (i = 0; i < nc; i++)
                Arbiter.Activate(dq, new Task<InputData, Port<int>>(ClArr[i], p, Mul));

            Arbiter.Activate(dq, Arbiter.MultipleItemReceive(true, p, nc, delegate(int[] array)
     {   }));

        }
    }
}
