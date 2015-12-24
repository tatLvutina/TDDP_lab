using System;
using RemoteServices;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Threading;


namespace RemoteClient
{
    class Program
    {
        private RemoteTask server;
        private TcpChannel channel;
        private SubTask myTask;
        private long taskCount;
        private long id;

        static void Main(string[] args)
        {
            new Program().start();
        }

        public void start()
        {
            try
            {
                channel = new TcpChannel();
                ChannelServices.RegisterChannel(channel, false);
                Console.WriteLine("Введите адрес сервера (tcp://localhost:8080/RemoteTask)");
                string addr = Console.ReadLine();
                server = (RemoteTask)Activator.GetObject(typeof(RemoteTask), addr);
                id = server.joinToServer();
                while (true)
                {
                    myTask = server.getTask(id);
                    if (myTask != null)
                    {
                        myTask.execute();
                        server.complete(id, myTask);
                        taskCount++;
                        Console.Clear();
                        Console.WriteLine("Выполнено задач: " + taskCount);
                    }
                    else
                    {
                        Thread.Sleep(1000);
                        Console.Clear();
                        Console.WriteLine("Ожидаю задачу...");
                    }
                }
            }
            catch (SystemException)
            {
                Console.WriteLine("Соединение было потеряно, завершение работы.\n" 
                        + "Нажмите для продолжения...");
            }
            
        }
    }
}
