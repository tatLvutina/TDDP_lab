using System; 
using System.Collections.Generic;

namespace SortLibrary 
{
    public class SharedObject : MarshalByRefObject 
    {
        static int number, i; 
        static double sumEl;
        const int dataCount1 = 100; // Кол−во элементов в матрице 1
        const int dataCount2 = 100; // Кол−во элементов в матрице 2
        const int tasksCount = 2; // максимальное кол−во задач
        Queue<Task> pendingTasks ; // очередь задач ожидающих обработки Object tasksLock ;
        int [] dataArray1 ;
        int [] dataArray2 ;
        Object dataLock ;
        public SharedObject () { 
            Log.Print("Создаем задачи и данные") ; 
            pendingTasks = new Queue<Task>() ; 
            GenerateData () ; 
            GenerateTasks () ;
            tasksLock = new Object () ; 
            dataLock = new Object () ;
        }
        
        void GenerateTasks () { 
            Task temp;
            int step = dataCount1 / tasksCount ; // на каждую задачу должна приходится равная порция массива
            for ( int i = 0; i < tasksCount ; i++) { 
                temp = new Task();
                temp.start = i∗step;
                temp.stop = temp.start + step − 1; 
                pendingTasks.Enqueue(temp);
            }
        }
        
        void GenerateData () {
        Random r = new Random() ; 
        dataArray1 = new int [dataCount1];
        for ( int i = 0; i < dataCount1 ; i++) {
            for ( int j = 0; j < dataCount1 ; j++) {
                dataArray1[i][j] = r.Next(0 , dataCount1 ∗ tasksCount1);
                }
            }
        dataArray2 = new int [dataCount2];
        for ( int i = 0; i < dataCount2; i++) {
            for ( int j = 0; j < dataCount2 ; j++) {
                dataArray2[i][j] = r.Next(0 , dataCount2 ∗ tasksCount2);
                }
            }
        }
        public int [] FetchData(Task task ) { 
            Log.Print (" Клиент получил данные"); 
            int [] res = new int [ task.stop − task.start ];
            int a = 0, b = 0; 
            for ( int i = task.start; i < task.stop ; i++) { 
                for ( int j = task.start; j < task.stop ; j++) {
                    res[a][b] = dataArray1[i][j] + dataArray2[i][j]; 
                    b++; 
            }
            a++;
            }
        return res;
        }
        
        public Task GetTask() { 
            Log.Print (" Клиент запрашивает задачу ") ; 
            lock (tasksLock) { 
                if (pendingTasks.Count == 0) { 
                Log.Print ("Больше ничего нет ") ; 
                return null ; 
                } else return pendingTasks.Dequeue() ;
            }
        }
        public void Finish (double sr ) {
            Log.Print (" Клиент закончил выполнение ") ; 
            lock (dataLock) { 
                Console.Out.Write("Сумма элементов: ") ; 
                for ( int i = task.start; i < task.stop ; i++) { 
                for ( int j = task.start; j < task.stop ; j++) {
                    res[i][j] = dataArray1[i][j] + dataArray2[i][j]; 
                }
            }
                Console.Out.WriteLine () ;
            } 
            if (pendingTasks.Count == 0) {
                Console.Out.WriteLine () ; 
                Console.Out.WriteLine () ; 
                Console.Out.Write("Сумма элементов: ";
                for ( int i = task.start; i < task.stop ; i++) { 
                for ( int j = task.start; j < task.stop ; j++) {
                    Console.Out.Write("" +res[i][j]); }}
                Console.Out.WriteLine () ;
            }
        }
        }
        [ Serializable ] 
        public class Task { 
            public int start = 0; 
            public int stop = 0; 
        }
        public class Log { // вывести время и msg 
        public static void Print ( String msg) { 
        System.Console.WriteLine("[" + DateTime.Now.Hour.ToString () + " : " +
            DateTime.Now.Minute.ToString () + " : " + DateTime.Now.Second.ToString () + "] " + msg) ; }

        }
        }
