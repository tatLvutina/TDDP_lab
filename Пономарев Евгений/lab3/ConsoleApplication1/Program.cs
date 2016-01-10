using System;
using Microsoft.Ccr.Core;
using System.Threading;
using IronPython;
using IronPython.Hosting;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;

namespace ConsoleApplication3
{
    public class InputData
    {
        public int start; // начало диапазона 
        public int stop;  // конец диапазона 
    }

    class Program
    {
        static int nc=2;   //количество ядер

    
        static void SequentialIntegration(dynamic monte_carlo, dynamic fun, dynamic Low, dynamic Up, dynamic step, dynamic N)
        {
            //взятие определенного интеграла по всему диапазону
            
            System.Diagnostics.Stopwatch sWatch = new System.Diagnostics.Stopwatch();
            sWatch.Start();

            dynamic result = monte_carlo(fun, Low, Up, N);
            
            sWatch.Stop();
            Console.WriteLine("Результат: " + result);
            Console.WriteLine("Последовательный алгоритм = {0} мс.",
            sWatch.ElapsedMilliseconds.ToString());

        }

        static void ParallelIntegration(dynamic step, dynamic Low, dynamic Up, ScriptScope scope)
        {
            // создание массива объектов для хранения параметров
            InputData[] ClArr = new InputData[nc];
            for (int i = 0; i < nc; i++)
                ClArr[i] = new InputData();

            //Далее, задаются исходные данные для каждого экземпляра
            //вычислительного метода:
            // заполняем массив параметров
            dynamic Low_temp = Low;
            for (int i = 0; i < nc; i++)
            {
                ClArr[i].start = Low_temp;
                if (i + 1 == nc)
                    ClArr[i].stop = Up;
                else
                    ClArr[i].stop = Low_temp + step;

                Low_temp = Low_temp + step;
            }
            //Создаётся диспетчер с пулом из двух потоков:
            Dispatcher d = new Dispatcher(nc, "Test Pool");
            DispatcherQueue dq = new DispatcherQueue("Test Queue", d);
            //Описывается порт, в который каждый экземпляр метода Int()
            //отправляет сообщение после завершения вычислений:
            Port<int> p = new Port<int>();
            //Метод Arbiter.Activate помещает в очередь диспетчера две задачи(два
            //экземпляра метода Mul):

            System.Diagnostics.Stopwatch ssWatch = new System.Diagnostics.Stopwatch();
            ssWatch.Start();

            for (int i = 0; i < nc; i++)
                Arbiter.Activate(dq, new Task<InputData, Port<int>, ScriptScope>(ClArr[i], p, scope, Int));
            //Первый параметр метода Arbiter.Activate – очередь диспетчера,
            //который будет управлять выполнением задачи, второй параметр –
            //запускаемая задача.
            //С помощью метода Arbiter.MultipleItemReceive запускается задача
            //(приёмник), которая обрабатывает получение двух сообщений портом p:
            Arbiter.Activate(dq, Arbiter.MultipleItemReceive(true, p, nc, delegate (int[] array)
            {
                Console.WriteLine("Вычисления завершены");
                ssWatch.Stop();
                Console.WriteLine("Полное время работы {0} мс", ssWatch.ElapsedMilliseconds.ToString());
                Console.ReadKey(true);
                Environment.Exit(0);
            }));
            

            
            Console.ReadKey(true);
            Environment.Exit(0);
        }


        static void Int(InputData data, Port<int> resp, ScriptScope scope)
        {
           //достаем функцию для интегрирования
           dynamic monte_carlo = scope.GetVariable("monte_carlo");
           //достаем необходимые переменные
           dynamic fun = scope.GetVariable("fun");
           dynamic N = scope.GetVariable("N");
           dynamic result;

           System.Diagnostics.Stopwatch sWatch = new System.Diagnostics.Stopwatch();
           sWatch.Start();

           result = monte_carlo(fun, data.start, data.stop, N);

           sWatch.Stop();
           
           Console.WriteLine("Поток № {0}: Паралл. алгоритм = {1} мс,. Результат: {2}",
           Thread.CurrentThread.ManagedThreadId,
           sWatch.ElapsedMilliseconds.ToString(), result);
           resp.Post(1);
        }


        static void Main(string[] args)
        {

            //Класс ScriptEngine применяется для создания движка, выполняющего скрипт.
            //Объект ScriptScope позволяет взаимодействовать со скриптом, получая или устанавливая его переменные, получая ссылки на функции.


            ScriptEngine engine = Python.CreateEngine();
            ScriptScope scope = engine.CreateScope();

            //В вычислительном модуле python используется модуль random.py
            //он находится в IronPython.StdLib
            //для работы программы необходимо подключить папку с StdLib:
            var paths = engine.GetSearchPaths();
            //путь к папке
            paths.Add(@"D:\IronPython.StdLib.2.7.5\content\Lib");
            engine.SetSearchPaths(paths);

            //непосредственный запуск модуля
            engine.ExecuteFile("D://monte-carlo.py", scope);

            //теперь можно "разобрать" запущенный скрипт на части, вытаскивая из него необходимые функции и переменные
            //вытаскиваем две функции
            dynamic param_enter = scope.GetVariable("param_enter");
            dynamic monte_carlo = scope.GetVariable("monte_carlo");

            //запускаем одну из них
            param_enter();

            //вытаскиваем введенные пользователем данные
            dynamic fun = scope.GetVariable("fun");
            dynamic Low = scope.GetVariable("Low");
            dynamic Up = scope.GetVariable("Up");
            dynamic step = scope.GetVariable("step");
            dynamic N = scope.GetVariable("N");

            //интегрируем целостно
            SequentialIntegration(monte_carlo,fun,Low,Up,step,N);

            //интегрируем параллельно
            ParallelIntegration(step,Low,Up,scope);
        }
    }
}

