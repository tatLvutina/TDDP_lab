using System;
using System.Windows.Forms;

using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Services;
using System.Runtime.Remoting;

namespace NetRemotingClient
{
    class Program
    {
        const int SYS_PORT = 9232;

        [STAThread]
        static void Main(string[] args)
        {
            if ((args.Length > 0) && (args[0] == "/gui"))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new EnteringForm());
                return;
            }


            Console.WriteLine("Client started.");

            TcpChannel clientChannel = new TcpChannel();
            ChannelServices.RegisterChannel(clientChannel, true);
            NetRemotingLibrary.RemotingLibrary calcClass = (NetRemotingLibrary.RemotingLibrary)Activator.GetObject(typeof(NetRemotingLibrary.RemotingLibrary), string.Format("tcp://localhost:{0}/TestClass", SYS_PORT));
            if (calcClass == null)
            {
                Console.WriteLine("could not locate server");
                return;
            }

            Console.WriteLine("Calling remote object...");

            double a = 3;
            double b = 2;
            double c = 1;
            double d = 5;

            double result = calcClass.Calculate(a, b, c, d);


            Console.WriteLine("PASS");
            Console.ReadLine();
        }
    }
}
