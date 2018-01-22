using System;

using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace server
{
    class server
    {
        const int SYS_PORT = 9232;

        static void Main(string[] args)
        {
            Console.WriteLine("Запуск сервера...");

            Console.WriteLine("Открываем {0} порт", SYS_PORT);

            TcpChannel channel = new TcpChannel(SYS_PORT);

            Console.WriteLine("Открываем канал");
            ChannelServices.RegisterChannel(channel, true);

            Console.WriteLine("Загружаем библиотеку в памать...");
            RemotingConfiguration.RegisterWellKnownServiceType(
             typeof(lib.lib), "TestClass", WellKnownObjectMode.SingleCall);

            //channel.StartListening(null);

            Console.WriteLine("Сервер запущен.");
            Console.ReadLine();

        }
    }
}
