using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using RemoteBase;

namespace Wrox.ProCSharp.Remoting
{
    class Program
    {
        static void Main()
        {
            //Создание и регистрирование канала TCP
            var channel = new TcpServerChannel(8086);
            ChannelServices.RegisterChannel(channel, true);
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(Box), "Hi", WellKnownObjectMode.Singleton);

            //Задержка работы сервера
            Console.WriteLine("Сервер запущен");
            Console.WriteLine("Для завершения нажмите Enter");
            Console.ReadLine();
        }
    }
}