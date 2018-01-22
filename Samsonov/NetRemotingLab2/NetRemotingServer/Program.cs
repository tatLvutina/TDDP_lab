using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Services;
using System.Runtime.Remoting;

namespace NetRemotingServer
{
    class Program
    {
        const int SYS_PORT = 9232;
        const string SYS_LIB_NAME = "Lib";

        static void Main(string[] args)
        {
            Console.WriteLine("Server start...");

            Console.WriteLine("Opening {0} port for listening...", SYS_PORT);
            
            TcpChannel channel = new TcpChannel(SYS_PORT);

            Console.WriteLine("Registering channel with security ensuring");
            ChannelServices.RegisterChannel(channel, true);

            Console.WriteLine("Pass library to memory...");
            RemotingConfiguration.RegisterWellKnownServiceType(
             typeof(NetRemotingLibrary.RemotingLibrary), SYS_LIB_NAME, WellKnownObjectMode.Singleton);

            Console.WriteLine("Press <ENTER> to shutdown server.");
            Console.ReadLine();

        }
    }
}
