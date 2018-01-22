using System;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting;

namespace Server
{
    class Program
    {
        // Define default class name for client connecting
        const string defaultClassName = "Library";

        static void Main(string[] args)
        {

            int port;

            // Check arguments if we want change port;
            // To change port start server instance with /port-<number> argument
            // Example: '/port-2345' will run server on 2345 port

            if ((args.Length > 0) && (args[0].StartsWith("/port-")))
            {
                string needPort = args[0].Replace("/port-", "");
                if (!int.TryParse(needPort, out port))
                    port = 2222;
            }
            else
                port = 2222;

            Console.WriteLine("Starting");
            Console.WriteLine("Open port {0}...", port);

            TcpChannel channel = new TcpChannel(port);

            Console.WriteLine("Register channel...");
            ChannelServices.RegisterChannel(channel, true);

            Console.WriteLine("Load cross-server lib...");
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(Library.Lib), defaultClassName, WellKnownObjectMode.Singleton);

            Console.WriteLine("\nPress <ENTER> to shutdown server.");
            Console.ReadLine();

            Console.WriteLine("Unregister channel...");
            ChannelServices.UnregisterChannel(channel);

            return;
        }
    }
}
