using System;
using System.Text;
using System.Threading;
using Microsoft.Ccr.Core;
using System.Diagnostics;

namespace MergeSortRobotics
{
    class Program
    {
        private int processCount;   // Количество создаваемых обработчиков
        private int itemCount;      // Размер сортируемого массива
        private int[] raw;          // Сортируемый массив

        public Program(int itemCount, int processCount)
        {
            this.processCount = processCount;
            this.itemCount = itemCount;
            raw = new int[itemCount];
            Random rand = new Random();             // Исходный массив
            for (int i = 0; i < itemCount; i++)     // будет заполнен 
                raw[i] = rand.Next(0, 10);          // произвольными значениями
        }

        static void Main(string[] args)
        {
            // Начинаем выполнение линейного алгоритма, результат - время его выполнения
            Console.WriteLine("1: " + new Program(100000000, 1).workSync());
            // Начинаем выполнение параллельного алгоритма, результат - время инициализации
            // Приняли решение, что вычисления будут вестись в 2 обработчиках (большее 
            // количество меняет логику работы => написанный код)
            Console.WriteLine("2: " + new Program(100000000, 2).workAsync());
        }

        /*
        * Метод, осуществляющий 1 и 2 шаг алгоритма -  рекурсивное разбиение 
        * исходного массива на подмассивы и применение к результатам деления
        * алгоритма слияния.
        */
        public void SortMerge(int[] raw, int left, int right)
        {
            if (right > left)
            {
                int mid = (right + left) / 2;
                SortMerge(raw, left, mid);
                SortMerge(raw, (mid + 1), right);
                MainMerge(raw, left, (mid + 1), right);
            }
        }

        /*
        * Метод, осуществляющий третий шаг алгоритма - слияние упорядоченных подмассивов.
        * Для сохранности исходных данных используется временный массив, данные которого
        * впоследствии копируются в исходный массив.
        */
        public void MainMerge(int[] raw, int left, int middle, int right)
        {
            int pos = 0;            // Индекс текущего элемента во временном хранилище
            int ri = middle;        // Индекс текущего элемента правого подмассива
            int li = left;          // Индекс текущего элемента левого подмассива
            int ltail = middle - 1; // Индекс последнего элемента левого подмассива
            int rtail = right;      // Индекс последнего элемента правого подмассива
            int length = right - left + 1;  // Длина временного хранилища
            int[] temp = new int[length];   // Само временное хранилище

            while ((li <= ltail) && (ri <= rtail)) // Перемещение наименьшего элемента
            {                                      // во временное хранилише.
                if (raw[li] <= raw[ri])            // Выполняется до тех пор
                    temp[pos++] = raw[li++];       // пока в одном из подмассивов
                else                               // "не останется" элементов
                    temp[pos++] = raw[ri++];
            }

            while (li <= ltail)                    // Перемешение остатков 
                temp[pos++] = raw[li++];           // левого подмассива

            while (ri <= rtail)                    // Перемещение остатков 
                temp[pos++] = raw[ri++];           // правого подмассива

            for (int i = 0; i < length; i++)       // Перемещение данных из
                raw[left + i] = temp[i];           // временного хранилища
        }

        /*
        * Метод, осуществляющий запуск линейного алгоритма сортировки слиянием.
        * Результат - итоговое время работы алгоритма в мс.
        */
        public long workSync()
        {
            Stopwatch watcher = new Stopwatch();
            watcher.Start();
            SortMerge(raw, 0, itemCount - 1);
            watcher.Stop();
            return watcher.ElapsedMilliseconds;
        }

        /*
        * Метод, осуществляющий запуск параллельного алгоритма сортировки слиянием.
        * Результат - время подготовки окружения, необходимого для запуска асинхронных
        * вычислений и разбиения исходной задачи на подзадачи.
        */
        public long workAsync()
        {
            Stopwatch watcher = new Stopwatch(); // "Итоговый" делегат сможет вычислить
            watcher.Start();                     // полное время вычислений

            InputData[] ClArr = new InputData[processCount];    // Создали хранилища
            for (int i = 0; i < processCount; i++)              // исходных данных
                ClArr[i] = new InputData();                     // для подзадач

            int step = (Int32)(itemCount / processCount);       // Размер блока данных

            int c = -1;
            for (int i = 0; i < processCount; i++)              // Заполнение хранилищ
            {                                                   // исходных данных
                ClArr[i].start = c + 1;                         // значениями
                ClArr[i].stop = c + step;
                c += step;
            }

            // Создали диспетчер задач
            Dispatcher d = new Dispatcher(processCount, "Test Pool");   
            // Обернули его очередью
            DispatcherQueue dq = new DispatcherQueue("Test Queue", d);
            // Определили экземпляр порта
            Port<int> port = new Port<int>();
            // Начали заполнять диспетчер задачами
            for (int i = 0; i < processCount; i++)
            {
                Arbiter.Activate(dq, new Task<InputData, Port<int>>(ClArr[i], port,
                    // Определяем временного обработчика задачи
                    // В нем мы и будем выполнять наш алгоритм сортировки
                    // По сути, мы создаем множество линейных алгоритмов,
                    // каждый из которых работает со свокй порцией данных
                    delegate (InputData data, Port<int> resp) 
                    {
                        Stopwatch sWatch = new Stopwatch();
                        sWatch.Start();

                        // Запустим на выполнение линейный алгоритм
                        SortMerge(raw, data.start, data.stop);

                        sWatch.Stop();  
                        // Покажем, сколько по времени работал конкретно этот поток
                        Console.WriteLine("Поток № {0}: локальное время работы = {1} мс.",
                            Thread.CurrentThread.ManagedThreadId,
                            sWatch.ElapsedMilliseconds.ToString());
                        resp.Post(1);
                    }
                    )
                );
            }

            // Зададим задачу, которая будет выполнена после выполнения всех остальных задач
            // (по факту она ожидает поступление сигналов в порт, равное количеству задач,
            // но каждая задача в конце своей работы генерирует в порт сигнал, так что
            // допущение вполне оправдано)
            Arbiter.Activate(dq, Arbiter.MultipleItemReceive(true, port, processCount, delegate (int[] array)
                {
                    // Проведем финальное слияние (у нас отсортировано 2 подмассива)
                    MainMerge(raw, 0, (itemCount - 1) / 2 + 1, itemCount - 1);
                    watcher.Stop();
                    // Отрапортуем об итоговом времени выполнения алгоритма
                    Console.WriteLine("Общее время выполнения: " + watcher.ElapsedMilliseconds);
                }
            ));
            return watcher.ElapsedMilliseconds;
        }

        /*
        * Метод на случай проверки результата работы алгоритма 
        *  Console.WriteLine(this);
        */
        public override String ToString()
        {
            StringBuilder temp = new StringBuilder();
            foreach (int item in raw)
            {
                temp.Append(item);
                temp.Append(' ');
            }
            return temp.ToString();
        }
    }

    public class InputData
    {
        public int start; // начало диапазона
        public int stop;  // конец диапазона
    }
}
