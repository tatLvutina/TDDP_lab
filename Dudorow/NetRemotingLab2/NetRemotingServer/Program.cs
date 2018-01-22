using System;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Open port {0}...", 11114);

            TcpChannel channel = new TcpChannel(11114);

            Console.WriteLine("Register channel...");
            ChannelServices.RegisterChannel(channel, true);

            Console.WriteLine("Load cross-server lib...");
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(Library.Lib), "equ", WellKnownObjectMode.Singleton);

            Console.WriteLine("\nPress <ENTER> to shutdown server.");
            Console.ReadLine();

            Console.WriteLine("Unregister channel...");
            ChannelServices.UnregisterChannel(channel);

            return;
        }
    }
}
