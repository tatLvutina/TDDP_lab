using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteBase
{
    public class Lib : MarshalByRefObject
    {
        public static double F(double x)
        {
            return x*x*x;
        }

        private static int globalID = 0;
        private static List<Client> clients = new List<Client>();

        private double boundA = 0;
        private double boundB = 0;
        private int sepN = 0;
        List<int> processedN = new List<int>();
        private double intResult = 0;


        private System.Timers.Timer timer;

        public Lib()
        {
            timer = new System.Timers.Timer();
            timer.Elapsed += timer_Elapsed;
            timer.Interval = 1000;
            timer.Start();
        }

        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            lock ("timer_elapsed")
            {
                for (int i = clients.Count - 1; i >= 0; --i)
                {
                    if ((clients[i].GetStatus() == Client.ClientStatus.BUSY) && (DateTime.Now - clients[i].GetTimeSinceLastInteraction()).TotalMilliseconds > 3000)
                    {
                        Console.WriteLine("Client [CLIENT ID: {0}] timed out.", clients[i].GetID());

                        int rowID = clients[i].GetMeta();
                        processedN.Add(rowID);

                        clients.RemoveAt(i);
                    }
                }
            }
        }

        private Client GetClientByID(int clientID)
        {
            Client client = null;
            for (int i = 0; i < clients.Count; ++i)
                if (clients[i].GetID() == clientID)
                    client = clients[i];

            return client;
        }

        public int RegisterClient(out bool isAdmin)
        {
            lock ("connect")
            {
                globalID++;

                Client client = new Client(globalID);

                bool hasAdmin = false;
                for (int i = 0; i < clients.Count; ++i)
                    hasAdmin |= clients[i].GetAdmin();

                hasAdmin |= !isWorkFinished();

                if (!hasAdmin)
                {
                    client.SetAdmin(true);
                    isAdmin = true;
                }
                else
                    isAdmin = false;
            

                clients.Add(client);

                return globalID;
            }
        }

        public void UnregisterClient(int clientID)
        {
            lock ("disconnect")
            {
                for (int i = 0; i < clients.Count; ++i)
                    if (clients[i].GetID() == clientID)
                        clients.RemoveAt(i);
            }
        }

        public void UploadTaskToServer(int clientID, double a, double b, int n)
        {
            lock ("admin_upload_task")
            {
                Client client = GetClientByID(clientID);

                if ((client == null) || (!client.GetAdmin()))
                {
                    Console.WriteLine("[ERROR] Only admin can give task's to server.");
                    return;
                }

                if ((processedN.Count != 0) && (!isWorkFinished()))
                {
                    Console.WriteLine("Server not fully completed previous work. Aborting.");
                    return;
                }

                Console.WriteLine();
                Console.WriteLine("Admin [client ID: {0}] upload task to server...", clientID);

                Console.WriteLine("Receiving task...");

                boundA = a;
                boundB = b;

                sepN = n;

                intResult = 0;

                processedN.Clear();

                for (int i = 1; i < n; i += 2)
                    processedN.Add(i);


                Console.WriteLine("Task received.");
                Console.WriteLine("Bound A: {0}\nBound B: {1}\nSteps: {2}", a, b, n);
            }
        }


        public void GetClientData(int clientID, out double x1, out double x2, out double x3)
        {
            lock ("client_data_out")
            {
                Client client = GetClientByID(clientID);

                int j = processedN[0];
                processedN.RemoveAt(0);

                client.SetStatus(Client.ClientStatus.BUSY);
                client.SetMeta(j);

                double h = (boundB - boundA) / sepN;

                x1 = (boundA) + (j - 1) * h;
                x2 = (boundA) + (j) * h;
                x3 = (boundA) + (j + 1) * h;
            }

        }

        public void ReturnClientData(int clientID, double f)
        {
            lock ("client_data_in")
            {
                Client client = GetClientByID(clientID);
                intResult += f;
                client.SetStatus(Client.ClientStatus.FREE);


                // Checkinh end
                bool flag = true;
                for (int i = 0; i < clients.Count; ++i)
                    flag &= (clients[i].GetStatus() == Client.ClientStatus.FREE);

                // Server ended
                if (flag && isWorkFinished())
                {
                    intResult /= 3;
                    intResult *= ((boundB - boundA) / (sepN));

                    Console.WriteLine("Server solved task");
                    Console.WriteLine("Result is: {0}", intResult);
                }


            }
        }


        public bool isWorkFinished()
        {
            lock ("finished")
            {
                return (processedN.Count == 0);
            }
        }
    }
}
