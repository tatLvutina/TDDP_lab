using System;
using System.Text;
using System.Threading;
using Microsoft.Ccr.Core;
using System.Diagnostics;

namespace Robotics
{
    class Program
    {
        private int processCount;   // Число создаваемых обработчиков
        private int itemCount;      // Размер сортируемого массива
        private int[] raw;          // Массив сортируемый 

        public Program(int itemCount, int processCount)
        {
            this.processCount = processCount;
            this.itemCount = itemCount;
            raw = new int[itemCount];
            Random rand = new Random();             // Начальный массив
            for (int i = 0; i < itemCount; i++)     // заполняется произвольными значениями
                raw[i] = rand.Next(0, 10);          
        }

        static void Main(string[] args)
        {
            // Выполнение линейного алгоритма, результат - время его выполнения
            Console.WriteLine("1: " + new Program(100000000, 1).workSync());
            // Начинаем выполнение параллельного алгоритма, результат - время инициализации
            // Вычисления будут вестись в 2 обработчиках 
            Console.WriteLine("2: " + new Program(100000000, 2).workAsync());
        }

        
        //Рекурсивное разбиениеисходного массива на подмассивы и применение
        //алгоритма слияния.
       
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

        //Слияние упорядоченных подмассивов.
        //Для сохранения исходных данных используется временный массив, данные которого
        //копируются в исходный массив.
        
        public void MainMerge(int[] raw, int left, int middle, int right)
        {
            int pos = 0;            // Индекс элемента ,текущего, во временном хранилище
            int ri = middle;        // Индекс элемента ,текущего,  правого подмассива
            int li = left;          // Индекс элемента ,текущего,  левого подмассива
            int ltail = middle - 1; // Индекс элемента ,последнего,  левого подмассива
            int rtail = right;      // Индекс элемента ,последнего,  правого подмассива
            int length = right - left + 1;  // Размер временного хранилища
            int[] temp = new int[length];   // Само временное хранилище

            while ((li <= ltail) && (ri <= rtail)) // Перемещение наименьшего элемента во временное хранилище
            {                                      
                if (raw[li] <= raw[ri])            
                    temp[pos++] = raw[li++];       
                else                               
                    temp[pos++] = raw[ri++];
            }

            while (li <= ltail)                    // Перемешение оставшихся элементов левого подмасива 
                temp[pos++] = raw[li++];           

            while (ri <= rtail)                    // Перемещение оставшихся элементов правого подмасива 
                temp[pos++] = raw[ri++];           

            for (int i = 0; i < length; i++)       // Перемещение данных из временного хранилища
                raw[left + i] = temp[i];           
        }

        
        //Метод, осуществляющий запуск линейного алгоритма сортировки слиянием.
        //Результат - итоговое время работы алгоритма.
       
        public long workSync()
        {
            Stopwatch watcher = new Stopwatch();
            watcher.Start();
            SortMerge(raw, 0, itemCount - 1);
            watcher.Stop();
            return watcher.ElapsedMilliseconds;
        }

      
         //Метод, осуществляющий запуск параллельного алгоритма сортировки слиянием.
         //Результат - время подготовки необходимого для запуска асинхронных
         //вычислений и разбиения задачи на подзадачи.
        
        public long workAsync()
        {
            Stopwatch watcher = new Stopwatch(); // вычисление полного времени вычислений
            watcher.Start();                     

            InputData[] ClArr = new InputData[processCount];    // Создал хранилище начальных данных для подзадач
            for (int i = 0; i < processCount; i++)              
                ClArr[i] = new InputData();                     

            int step = (Int32)(itemCount / processCount);       // Размер данных

            int c = -1;
            for (int i = 0; i < processCount; i++)              // Заполнил хранилище исходными данными 
            {                                                   
                ClArr[i].start = c + 1;                         
                ClArr[i].stop = c + step;
                c += step;
            }

            // Создадим диспетчер задач
            Dispatcher d = new Dispatcher(processCount, "Test Pool");
            DispatcherQueue dq = new DispatcherQueue("Test Queue", d);
            // Определим экземпляр порта
            Port<int> port = new Port<int>();
            // Заполняем диспетчер задачами
            for (int i = 0; i < processCount; i++)
            {
                Arbiter.Activate(dq, new Task<InputData, Port<int>>(ClArr[i], port, sort));
            }

            // Зададим задачу, которая будет выполнена после выполнения всех остальных задач
            Arbiter.Activate(dq, Arbiter.MultipleItemReceive(true, port, processCount, delegate (int[] array)
                {
                    // Проведем окончательно слияние 2 отсортированных подмасива
                    MainMerge(raw, 0, (itemCount - 1) / 2 + 1, itemCount - 1);
                    watcher.Stop();
                    // Выведем сообщение об времени выполнения алгоритма
                    Console.WriteLine("Общее время выполнения: " + watcher.ElapsedMilliseconds);
                }
            ));
            return watcher.ElapsedMilliseconds;
        }

        public void sort(InputData data, Port<int> resp)
        {
            Stopwatch sWatch = new Stopwatch();
            sWatch.Start();

            // Запустим на выполнение линейный алгоритм
            SortMerge(raw, data.start, data.stop);

            sWatch.Stop();
            // Выведем информацию о том, сколько по времени работал конкретно этот поток
            Console.WriteLine("Поток № {0}: локальное время работы = {1} мс.",
                Thread.CurrentThread.ManagedThreadId,
                sWatch.ElapsedMilliseconds.ToString());
            resp.Post(1);
        }

        
    }

    public class InputData
    {
        public int start; // начало диапазона
        public int stop;  // конец диапазона
    }
}
