using System;
using System.Windows.Forms;

using NetRemotingLibrary;

using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Services;
using System.Runtime.Remoting;

namespace NetRemotingClient
{
    class Program
    {
        const int SYS_PORT = 9232;
        const string SYS_LIB_NAME = "Lib";


        private static RemotingLibrary remotingClass = null;


        static void Main(string[] args)
        {

            TcpChannel clientChannel = new TcpChannel();
            ChannelServices.RegisterChannel(clientChannel, true);

            string connectingString = string.Format("tcp://localhost:{0}/{1}", SYS_PORT,  SYS_LIB_NAME);
            remotingClass = (RemotingLibrary)Activator.GetObject(typeof(RemotingLibrary), connectingString);

            if (remotingClass == null)
            {
                Console.WriteLine("Client not connected! Abort!");
                return;
            }

            bool isAdmin = false;
            int clientID = remotingClass.RegisterClient(out isAdmin);

            // admin
            if (isAdmin)
            {
                Console.WriteLine("Current client connected as admin.");

                const int rows = 200;
                const int cols = 200;

                int[,] task = new int[rows, cols];
                Random r = new Random();
                for (int i = 0; i < rows; ++i)
                    for (int j = 0; j < cols; ++j)
                        task[i, j] = r.Next(10);

                Console.WriteLine("Uploading task...");
                remotingClass.UploadTaskToServer(clientID, task);

                Console.WriteLine("Task uploaded.");

                Console.WriteLine("Press any key to disconnect admin client...");
                Console.ReadKey();

                Console.WriteLine("Disconnecting...");
                remotingClass.UnregisterClient(clientID);

                Console.WriteLine("Ended.");
                return;
            }

            Console.WriteLine("Current client connected as processor client.");

            while (!remotingClass.isWorkFinished())
            {
                Console.WriteLine();

                int[] array = remotingClass.GetClientData(clientID);

                Console.WriteLine("Client received data: ");
                for (int i = 0; i < array.GetLength(0); ++i)
                    Console.Write("{0} ", array[i]);
                Console.WriteLine();

                Console.WriteLine("Sorting...");

                for (int i = 0; i < array.GetLength(0); ++i)
                {
                    for (int j = i; j < array.GetLength(0); ++j)
                    {
                        if (array[i] > array[j])
                        {
                            int a = array[i];

                            array[i] = array[j];
                            array[j] = a;
                        }
                    }
                }

                Console.WriteLine("Client sorted data: ");
                for (int i = 0; i < array.GetLength(0); ++i)
                    Console.Write("{0} ", array[i]);
                Console.WriteLine();

                remotingClass.ReturnClientData(clientID, array);
            }

            remotingClass.UnregisterClient(clientID);

            Console.WriteLine("Client finished.");
        }
    }
}
