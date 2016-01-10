using System;
using System.Collections.Generic;
using IronPython;
using IronPython.Hosting;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;

namespace SortLibrary
{
    public class SharedObject : MarshalByRefObject
    {

        const int tasksCount = 2;   //максимальное число задач
        public int fin_res;         //для результата

        Queue<Task> QueTasks; // очередь задач ожидающих обработки
        Object tasksLock;

        Object dataLock;

        public SharedObject()
        {
            QueTasks = new Queue<Task>();
            CreateTasks();

            tasksLock = new Object();
            dataLock = new Object();
        }


        public void CreateData(int flag, out dynamic fun, out dynamic Low, out dynamic Up, out dynamic step, out dynamic N)
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

            string str_fun = "x**2";

            if (flag==1)
                ServerConsole.Print("Подынтегральная функция:" + str_fun);

            int Temp_Low = 1;
            int Temp_Up = 10;
            int Temp_step = (Int32)((Temp_Up - Temp_Low) / 2);
            int Temp_N = 10000;
            scope.SetVariable("fun", str_fun);
            scope.SetVariable("Low", Temp_Low);
            scope.SetVariable("Up", Temp_Up);
            scope.SetVariable("step", Temp_step);
            scope.SetVariable("N", Temp_N);

            //непосредственный запуск модуля
            engine.ExecuteFile("D://monte-carlo.py", scope);

            //теперь можно "разобрать" запущенный скрипт на части, вытаскивая из него необходимые функции и переменные
            fun = scope.GetVariable("fun");
            Low = scope.GetVariable("Low");
            Up = scope.GetVariable("Up");
            step = scope.GetVariable("step");
            N = scope.GetVariable("N");
        }

        void CreateTasks()
        {
            ServerConsole.Print("\n\nСоздание задач...\n");
            Task temp;

            //обратимся к скрипту за нужными переменными
            dynamic fun, Low, Up, step, N;
            CreateData(1, out fun, out  Low, out  Up, out  step, out  N);

            //распределение массива поровну на каждого клиента
            ServerConsole.Print("Длина отрезка:{0}",Up-Low+1);
            ServerConsole.Print("Клиентов:{0}",tasksCount);
            ServerConsole.Print("Часть отрезка, приходящаяся на клиента:{0}",step+1);

            dynamic Low_temp = Low;
            for (int i = 0; i < tasksCount; i++)
            {
                temp = new Task();
                ServerConsole.Print("\nКоординаты отрезка для клиента #{0}",i+1);
                temp.start = Low_temp;              
                ServerConsole.Print("Начало: {0}",temp.start);
                if (i + 1 == tasksCount)
                    temp.stop = Up;
                else
                    temp.stop = temp.start + step;
                  
                ServerConsole.Print("Конец: {0}", temp.stop);

                Low_temp = Low_temp + step;
                QueTasks.Enqueue(temp);                      //добавление задачи в конец очереди
            }
            ServerConsole.Print("\nЗадачи успешно созданы и распределены!");
        }



        public void GetData(Task task, out dynamic Cl_Low, out dynamic Cl_Up, out dynamic Cl_fun, out dynamic Cl_N)
        {
            dynamic fun, Low, Up, step, N;
            CreateData(0, out fun, out Low, out Up, out step, out N);

            ServerConsole.Print("\nКлиент начал получение данных для обработки!");      
            Cl_Low = task.start;
            Cl_Up = task.stop;
            Cl_fun = fun;
            Cl_N = N;
            Console.Out.WriteLine("Координаты отрезка, передаваемого клиенту:");
            ServerConsole.Print("Начало: {0}", Cl_Low);
            ServerConsole.Print("Конец: {0}", Cl_Up);
            ServerConsole.Print("Клиент получил данные для обработки!\n\n");
        }

        public Task GetTask()
        {
            ServerConsole.Print("\nКлиент запросил задачу");
            lock (tasksLock)
            {
                if (QueTasks.Count == 0) //если задачи кончились
                {
                    ServerConsole.Print("Больше нет задач..."); //сообщим об этом
                    return null;
                }
                else
                    return QueTasks.Dequeue(); //если еще не кончились - вернем следующую задачу, извлеченную из очереди
            }
        }

        public void Finish(dynamic res)
        {

            lock (dataLock)
            {
                fin_res += res; 
                ServerConsole.Print("\nКлиент успешно завершил задачу!");
            }

            if (QueTasks.Count == 0)
            {
                Console.Out.Write("\n\nПолученный результат: {0} \n", fin_res);
            }
        }
    }



    [Serializable]
    public class Task
    {
        public int start = 0, stop = 0;  //определение начала и конца отрезка
    }
    public class ServerConsole //вывод записи в консоли на сервере
    {
        public static void Print(String msg)  
        {
            Console.WriteLine(msg);
        }
        public static void Print(String msg, int param1)  
        {
            Console.WriteLine(msg, param1);
        }
    }




}

