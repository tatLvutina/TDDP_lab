using System;
using SortLibrary;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace SortServer
{
    class Server
    {
        TcpChannel channel;
        public void Start()
        {
            channel = new TcpChannel(8080);
            ChannelServices.RegisterChannel(channel, false);
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(SharedObject), "Work", WellKnownObjectMode.Singleton);
            ServerConsole.Print("Сервер запущен!");
        }

        public void Stop()
        {
            ChannelServices.UnregisterChannel(channel);
            ServerConsole.Print("Сервер остановлен!");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server();
            server.Start();
            Console.In.ReadLine();
            server.Stop();
        }
    }
}
