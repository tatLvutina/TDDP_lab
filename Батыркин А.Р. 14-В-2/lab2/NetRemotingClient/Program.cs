using System;
using RemoteBase;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Services;
using System.Runtime.Remoting;

namespace Client
{
    class Program
    {
        private static Lib remoteObject = null;

        static void Main(string[] args)
        {

            TcpChannel clientChannel = new TcpChannel();
            ChannelServices.RegisterChannel(clientChannel, true);

            remoteObject = (Lib)Activator.GetObject(typeof(Lib), "tcp://localhost:1111/RemoteBase");

            if (remoteObject == null)
                return;

            bool isAdmin = false;
            int clientID = remoteObject.RegisterClient(out isAdmin);

            if (isAdmin)
            {
                Console.WriteLine("Connected as [ADMIN]");

                Console.WriteLine("Send task to server...");
                remoteObject.UploadTaskToServer(clientID, 0.0, 3.0, 10000);
                Console.WriteLine("Bound A: {0}\nBound B:{1}\nSteps: {2}", 0.0, 3.0, 10000);

                Console.WriteLine("Press any key to disconnect admin client...");
                Console.ReadKey();

                Console.WriteLine("Disconnecting...");
                remoteObject.UnregisterClient(clientID);

                Console.WriteLine("Ended.");
                return;
            }

            Console.WriteLine("Current client connected as processor client.");

            double x1, x2, x3;

            while (!remoteObject.isWorkFinished())
            {
                Console.WriteLine();

                remoteObject.GetClientData(clientID, out x1, out x2, out x3);
                
                double answer = Lib.F(x1);
                answer += 4 * Lib.F(x2);
                answer += Lib.F(x3);
                Console.WriteLine("Client calculated value: {0}", answer);

                remoteObject.ReturnClientData(clientID, answer);
            }

            remoteObject.UnregisterClient(clientID);

            Console.WriteLine("Client finished.");
        }
    }
}
