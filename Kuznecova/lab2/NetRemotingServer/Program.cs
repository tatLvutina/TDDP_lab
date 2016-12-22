using System;
using RBase;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Services;
using System.Runtime.Remoting;

namespace NetRemotingServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Server start...");          
            TcpChannel channel = new TcpChannel(2222);

            ChannelServices.RegisterChannel(channel, true);
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(Lib), "RemoteBase", WellKnownObjectMode.Singleton);

            Console.WriteLine("Server started.");

            Console.WriteLine("Press <ENTER> to shutdown server.");
            Console.ReadLine();
        }
    }
}
