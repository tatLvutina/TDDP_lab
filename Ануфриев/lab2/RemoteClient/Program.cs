using System;
using RemoteServices;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace RemoteClient
{
    class Program
    {
        private RemoteTask server;
        private TcpChannel channel;
 
        int ID;
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
                server = (RemoteTask)Activator.GetObject(typeof(RemoteTask), "tcp://localhost:8080/ShellSort");
                Console.Write("Лабораторная работа №2 \n Автор: Ануфриев И.С. \n гр. 13-В-1 \n ShellSort \n");
                server.CreateNewClient();
                ID = server.getID()-1;
                server.ShellSort(ID);                
                for (int i = 0; i < server.massive.GetLength(1); i++)
                    Console.Write(server.massive[ID, i] + " ");
                    Console.ReadKey();
            }
            catch (SystemException)
            {
                Console.WriteLine("Соединение было потеряно, завершение работы.\n" 
                        + "Нажмите для продолжения...");
                Console.ReadKey();
            }
            
        }
    }
}
