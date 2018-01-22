using System;
using RBase;
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

            remoteObject = (Lib)Activator.GetObject(typeof(Lib), "tcp://localhost:2222/RemoteBase");

            if (remoteObject == null)
                return;

            bool isAdmin = false;
            int clientID = remoteObject.RegisterClient(out isAdmin);

            if (isAdmin)
            {
                Console.WriteLine("Connected as [ADMIN]");

                const int SIZE = 200024;
                int[] array = new int[SIZE];

                int genMax, genMin;

                Random rand = new Random();
                for (int i = 0; i < SIZE; ++i)
                    array[i] = rand.Next();

                Lib.F(array, out genMax, out genMin);
                Console.WriteLine("Tested result: [max][min] [{0}][{1}]", genMax, genMin);

                Console.WriteLine("Send task to server...");
                remoteObject.UploadTaskToServer(clientID, array);
                Console.WriteLine("Task sended");

                Console.WriteLine("Press any key to disconnect admin client...");
                Console.ReadKey();

                Console.WriteLine("Disconnecting...");
                remoteObject.UnregisterClient(clientID);

                Console.WriteLine("Ended.");
                return;
            }

            Console.WriteLine("Current client connected as processor client.");

            int[] inArray;
            int max, min;
            while (!remoteObject.isWorkFinished())
            {
                Console.WriteLine();

                inArray = remoteObject.GetClientData(clientID);

                Lib.F(inArray, out max, out min);
                Console.WriteLine("Client found [max][min] value: [{0}][{1}]", max, min);

                remoteObject.ReturnClientData(clientID, max, min);
            }

            remoteObject.UnregisterClient(clientID);

            Console.WriteLine("Client finished.");
        }
    }
}
