using System;
using System.Windows.Forms;

using Library;

// Connect namespace for .NET Remoting
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Services;
using System.Runtime.Remoting;


namespace Client
{
    class Program
    {
        private static Lib remoteObject = null;

        public static int SuperMethod(int par1, int par2)
        {
            return par1 * par2;
        }

        static void Main(string[] args)
        {
            // Opening channel
            TcpChannel channel = new TcpChannel();

            // Registering channel
            ChannelServices.RegisterChannel(channel, true);

            // Getting reference for instance of library
            remoteObject = (Lib)Activator.GetObject(typeof(Lib), "tcp://localhost:11114/equ");
           
            Console.WriteLine("Entered main routine...");

            int id = remoteObject.Connect();

            while (!remoteObject.isFinished())
            {
                int[] parameters = remoteObject.GetParameters();
                Console.WriteLine("Client multiplying values: {0}, {1}", parameters[0], parameters[1]);
                remoteObject.ReturnResultFromClient(SuperMethod(parameters[0], parameters[1]));

               // Console.ReadKey();
            }

            remoteObject.Disconnect(id);

            Console.WriteLine("Client exited.");
            Console.ReadKey();

        }
    }
}
