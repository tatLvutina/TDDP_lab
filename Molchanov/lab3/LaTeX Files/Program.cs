using System;
using Microsoft.Ccr.Core;
using System.Threading;

namespace Sl_Matricz
{
    public class InputData
    {
        public int start; // начало диапазона 
        public int stop;  // конец диапазона 
        public int i;
    }

    class Program
    {
        static int m, n;
        static int nc;
        nc = 4; // количество ядер
        m = 11000; // количество строк матрицы
        n = 11000; // количество столбцов матрицы
        A = new int[m, n];
        B = new int[m, n];
        C = new int[m, n];

        static void Main(string[] args)
        {

            Stopwatch sWatch = new Stopwatch();
            sWatch.Start(); 
                for (int i = 0; i < m; i++) {
                     C[i, j] = 0;
                     for (int j = 0; j < n; j++)
                      C[i, j] += A[i, j] * B[i, j]; }
            sWatch.Stop();
            Console.WriteLine("Последовательный алгоритм = {0} мс.", sWatch.ElapsedMilliseconds.ToString());
            
            
            void Mul(InputData data, Port<int> resp)
            {
               Stopwatch sWatch = new Stopwatch();
                 sWatch.Start();
                 for (int i = data.start; i < data.stop; i++)
                 {
                    C[i] = 0;
                      for (int j = 0; j < n; j++)
                           C[i, j] += A[i, j] * B[i, j];
                }
                 sWatch.Stop();
            Console.WriteLine("Поток № {0}: Параллельный алгоритм = {1} мс.", Thread.CurrentThread.ManagedThreadId, 	sWatch.ElapsedMilliseconds.ToString());
            resp.Post(1);
}

            InputData[] ClArr = new InputData[nc];
            for (int i = 0; i < nc; i++)
                ClArr[i] = new InputData();
//Далее, задаются исходные данные для каждого экземпляра вычислительного метода:
// делим количество строк в матрице на nc частей
            int step = (Int32)(m / nc);
// заполняем массив параметров
            int c = -1;
            for (int i = 0; i < nc; i++)
            {
              ClArr[i].start = c + 1;
              ClArr[i].stop = c + step;
              c = c + step;
            }
//Создаётся диспетчер с пулом из двух потоков:
            Dispatcher d = new Dispatcher(nc, "Test Pool");
            DispatcherQueue dq = new DispatcherQueue("Test Queue", d);
//Описывается порт, в который каждый экземпляр метода Mul() отправляет сообщение после завершения вычислений:
            Port<int> p = new Port<int>();
//Метод Arbiter.Activate помещает в очередь диспетчера две задачи (два экземпляра метода Mul):
            for (int i = 0; i < nc; i++)
                Arbiter.Activate(dq, new Task<InputData, Port<int>>(ClArr[i], p, Mul));
//С помощью метода Arbiter.MultipleItemReceive запускается задача (приёмник), которая обрабатывает получение двух сообщений портом p:
            Arbiter.Activate(Environment.TaskQueue, Arbiter.MultipleItemReceive(true, p, nc, delegate(int[] array)
            {
               Console.WriteLine("Вычисления завершены");
            }));
        }
    }
}
