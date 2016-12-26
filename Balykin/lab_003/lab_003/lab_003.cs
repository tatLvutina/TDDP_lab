using System;
using System.Threading;

using Microsoft.Ccr.Core;

namespace lab_003
{
    public class InputData {
        public int start; // начало диапазона 
        public int stop;  // конец диапазона 
        //public int i;
    }

    class lab_003 {
        static int[,] A; //хранение матрицы
        static int[] B;  //хранение вектор-столбца для умножения
        static int[] C;  //хранение результата
        static int m;    //количество строк матрицы
        static int n;    //количество столбцов матрицы
        static int nc;   //количество ядер

        static void Test() {
            nc = 2;
            ConsoleKey press;

            Console.WriteLine("Желаете задать размер матрицы самостоятельно? (Y/N)\n");

            press = Console.ReadKey(true).Key;
            if (press != ConsoleKey.N & press != ConsoleKey.Y) {
                Console.WriteLine("Некорректная клавиша");
                press = ConsoleKey.N;
            }

            if (press == ConsoleKey.Y) {
                Console.Write("Введите количество строк матрицы: ");
                m = Convert.ToInt32(Console.ReadLine());
                Console.Write("Введите количество столбцов матрицы: ");
                n = Convert.ToInt32(Console.ReadLine());
            } else if (press == ConsoleKey.N) {
                m = 10000;
                n = 10000;
                Console.WriteLine("\nРазмеры матрицы заданы автоматически и составляют {0} x {1}\n", m, n);
            }

            A = new int[m, n];
            B = new int[n];
            C = new int[m];

            Console.WriteLine("\n\nЖелаете заполнить матрицу и вектор-столбец самостоятельно? (Y/N)\n");
            press = Console.ReadKey(true).Key;

            if (press != ConsoleKey.N & press != ConsoleKey.Y) {
                Console.WriteLine("Некорректная клавиша");
                press = ConsoleKey.N;
            }

            if (press == ConsoleKey.Y) {
                Console.WriteLine("Ввод матрицы\n");
                for (int i = 0; i < m; i++) {
                    for (int j = 0; j < n; j++) {
                        Console.Write("A[{0},{1}] = ", i + 1, j + 1);
                        A[i, j] = Convert.ToInt32(Console.ReadLine());
                    }

                }
                Console.WriteLine("\n\nВвод вектор-столбца\n");
                for (int j = 0; j < n; j++) {
                    Console.Write("B[{0}] = ", j + 1);
                    B[j] = Convert.ToInt32(Console.ReadLine());
                }
            } else if (press == ConsoleKey.N) {
                Console.WriteLine("Заполнение случайными значениями...\n");
                Random r = new Random();
                for (int i = 0; i < m; i++) {
                    for (int j = 0; j < n; j++) {
                        A[i, j] = r.Next(100);
                    }
                }
                for (int j = 0; j < n; j++) {
                    B[j] = r.Next(100);
                }

                Console.WriteLine("Исходная матрица и вектор-столбец успешно заполнены случайными значениями!\n");
            }
        }

        static void SequentialMul() {
            System.Diagnostics.Stopwatch sWatch = new System.Diagnostics.Stopwatch();
            sWatch.Start();
            for (int i = 0; i < m; i++) {
                C[i] = 0;
                //Console.WriteLine("\n");  //эта строка тоже относится к блоку проверки правильности
                for (int j = 0; j < n; j++) {
                    C[i] += A[i, j] * B[j];
                    //Внимание!
                    //ниже закоментирован блок отображения проверки правильности умножения
                    //Если включить данный блок в исполняемый код,
                    //то для корректного вывода проверки 
                    //рекомендуется брать матрицы небольшого размера
                    //например, 3х3
                    //
                    //Console.Write(" {0} * {1} ", A[i,j], B[j]); 
                    //if (j+1 != n)
                    //{
                    //    Console.Write("+");                     
                    //}
                    //else
                    //{
                    //    Console.Write("= {0}", C[i]);
                    //}
                }
            }
            //Console.WriteLine("\n");   //эта строка тоже относится к блоку проверки правильности
            sWatch.Stop();
            Console.WriteLine("Последовательный алгоритм = {0} мс.",
            sWatch.ElapsedMilliseconds.ToString());

        }

        static void ParallelMul() {
            // создание массива объектов для хранения параметров
            InputData[] ClArr = new InputData[nc];
            for (int i = 0; i < nc; i++) {
                ClArr[i] = new InputData();
            }
            //Далее, задаются исходные данные для каждого экземпляра
            //вычислительного метода:
            // делим количество строк в матрице на nc частей
            int step = (Int32)(m / nc);
            // заполняем массив параметров
            int c = -1;
            for (int i = 0; i < nc; i++) {
                ClArr[i].start = c + 1;
                ClArr[i].stop = c + step;
                c = c + step;
            }
            //Создаётся диспетчер с пулом из двух потоков:
            Dispatcher d = new Dispatcher(nc, "Test Pool");
            DispatcherQueue dq = new DispatcherQueue("Test Queue", d);
            //Описывается порт, в который каждый экземпляр метода Mul()
            //отправляет сообщение после завершения вычислений:
            Port<int> p = new Port<int>();
            //Метод Arbiter.Activate помещает в очередь диспетчера две задачи(два
            //экземпляра метода Mul):
            for (int i = 0; i < nc; i++) {
                Arbiter.Activate(dq, new Task<InputData, Port<int>>(ClArr[i], p, Mul));
            }
            //Первый параметр метода Arbiter.Activate – очередь диспетчера,
            //который будет управлять выполнением задачи, второй параметр –
            //запускаемая задача.

            //С помощью метода Arbiter.MultipleItemReceive запускается задача
            //(приёмник), которая обрабатывает получение двух сообщений портом p:
            Arbiter.Activate(dq, Arbiter.MultipleItemReceive(true, p, nc, delegate (int[] array) {
                dispResult();
                Console.WriteLine("Вычисления завершены");
                Console.ReadKey(true);
                Environment.Exit(0);
            }));
        }

        static void Mul(InputData data, Port<int> resp) {
            System.Diagnostics.Stopwatch sWatch = new System.Diagnostics.Stopwatch();
            sWatch.Start();

            for (int i = data.start; i < data.stop; i++) {
                C[i] = 0;
                for (int j = 0; j < n; j++)
                    C[i] += A[i, j] * B[j];
            }
            sWatch.Stop();
            Console.WriteLine("Поток № {0}: Паралл. алгоритм = {1} мс.",
           Thread.CurrentThread.ManagedThreadId,
           sWatch.ElapsedMilliseconds.ToString());
            resp.Post(1);
        }

        static void dispResult() {
            int i;
            Console.WriteLine("Показать результат умножения? (Y/N)\n");

            var press = Console.ReadKey(true).Key;

            if (press != ConsoleKey.N & press != ConsoleKey.Y) {
                Console.WriteLine("Некорректная клавиша");
                press = ConsoleKey.N;
            }

            if (press == ConsoleKey.Y) {
                for (i = 0; i < m; i++)
                    Console.WriteLine("C[{0}]: {1}", i + 1, C[i].ToString());
            }

            if (press == ConsoleKey.N) {
                Console.WriteLine("Вывод результата отклонен.");
            }
        }

        static void Main(string[] args) {
            Test();
            SequentialMul();
            dispResult();
            ParallelMul();
        }
    }

}
