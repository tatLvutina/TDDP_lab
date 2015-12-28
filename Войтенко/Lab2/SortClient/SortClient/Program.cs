using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SortLibrary;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace SortClient
{
    class Shell
    {
        TcpChannel chan;
        SharedObject obj;
        double[] arr;

        Task task;

        public Shell()
        {
            chan = new TcpChannel();
            ChannelServices.RegisterChannel(chan, false);
            obj = (SharedObject)Activator.GetObject(typeof(SortLibrary.SharedObject), "tcp://localhost:8081/DataPool");
        }

        public int sort()
        {
            //try
            //{
           
                task = obj.GetTask();
                if (task == null)
                    return 0;

                arr = obj.FetchData(task);
                Console.Out.Write("Получено");
                
                
                double x=0,dx,Pi=0;
                x = 0.0;
                if (task.start != 0) { x =  0.5; }
                dx = 1.0 / 1000000.0;
                int  outer;
 
               


                for (outer = 0; outer < task.stop - task.start; outer++)
                {
                    Pi += Math.Sqrt(1 - x * x)*4*dx;
                    x += dx;

                   
                }

                Console.Out.WriteLine(" S(rec)="+Pi);


                obj.Finish(Pi);

            return 1;
        }


    }

    class Program
    {


        static void Main(string[] args)
        {
            Shell shellObj = new Shell();
            Console.Out.WriteLine("Клиент запущен");

            while (shellObj.sort() != 0)
                Console.In.ReadLine();

            Console.Out.WriteLine("Задачи кончились, нажмите Enter...");
            Console.ReadLine();
            

        }
    }
}
