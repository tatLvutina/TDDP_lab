using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using RemoteServices;

namespace RemoteServer
{
    class Program
    {
        private TcpChannel channel;

        static void Main(string[] args)
        {
            new Program().start();
        }

        public void start()
        {
            channel = new TcpChannel(8080);
            ChannelServices.RegisterChannel(channel, false);
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(RemoteTask), "RemoteTask", WellKnownObjectMode.Singleton);
            Console.WriteLine("Сервер успешно запущен.\nНажмите любую клавишу для завершения...");
            Console.ReadKey(true);
            Console.WriteLine("Завершение работы сервера...");
        }

        ~Program()
        {
            if (channel != null)
            {
                ChannelServices.UnregisterChannel(channel);
            }
        }
    }
}
