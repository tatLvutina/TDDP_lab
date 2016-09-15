using System;
using SearchClasses;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace SearchServer
{
    class Server
    {
        static void Main(string[] args)
        {
            TcpChannel channel;
            channel = new TcpChannel(8080);
            ChannelServices.RegisterChannel(channel, false);
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(RemoteObject), "Search", WellKnownObjectMode.Singleton);
            TimerOutput.Print("Старт");
            Console.In.ReadLine();
            ChannelServices.UnregisterChannel(channel);
            TimerOutput.Print("Стоп");
        }
    }
}
