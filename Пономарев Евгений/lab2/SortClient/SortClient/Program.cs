using System;
using SortLibrary;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using IronPython;
using IronPython.Hosting;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;

namespace SortClient
{
    class Shell
    {
        TcpChannel chan;
        SharedObject obj;

        Task task;

        public Shell()
        {
            chan = new TcpChannel();
            ChannelServices.RegisterChannel(chan, false);
            obj = (SharedObject)Activator.GetObject(typeof(SortLibrary.SharedObject), "tcp://localhost:8080/Work");
        }

        public int Int()
        {      
            task = obj.GetTask();
      
            if (task == null)
                return 0;

            dynamic Cl_Low, Cl_Up, Cl_fun, Cl_N;
            obj.GetData(task, out Cl_Low, out Cl_Up, out Cl_fun, out Cl_N);
            Console.Out.Write("Полученные данные:");

            Console.Out.Write("\nКоординаты отрезка:\n");
            ServerConsole.Print("Начало: {0}", Cl_Low);
            ServerConsole.Print("Конец: {0}", Cl_Up);

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
            //вытаскиваем интегрирующую функцию
            dynamic monte_carlo = scope.GetVariable("monte_carlo");

            dynamic result = monte_carlo(Cl_fun, Cl_Low, Cl_Up, Cl_N);            

            obj.Finish(result);

            return 1;
        }

    }

    class Program
    {


        static void Main(string[] args)
        {
            Shell shellObj = new Shell();
            Console.Out.WriteLine("Клиент запущен!");

            while (shellObj.Int() != 0)
                Console.In.ReadLine();

            Console.Out.WriteLine("Задач больше нет!");
            Console.ReadLine();
            

        }
    }
}
