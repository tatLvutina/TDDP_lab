using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBase
{
    public class Lib : MarshalByRefObject
    {
        public static void F(int[] array, out int max, out int min)
        {
            int findMax = array[0];
            int findMin = array[0];
            for (int i = 0; i < array.GetLength(0); ++i)
            {
                if (findMax < array[i])
                    findMax = array[i];

                if (findMin > array[i])
                    findMin = array[i];
            }

            max = findMax;
            min = findMin;
        }

        private static int globalID = 0;
        private static List<Client> clients = new List<Client>();

        private int[] serverArray;
        private int[] sendArray;
        int separatedBy;
        List<int> processedParts = new List<int>();

        int resultMax = 0;
        int resultMin = 0;

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
                        processedParts.Add(rowID);

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

        public void UploadTaskToServer(int clientID, int[] array)
        {
            lock ("admin_upload_task")
            {
                Client client = GetClientByID(clientID);

                if ((client == null) || (!client.GetAdmin()))
                {
                    Console.WriteLine("[ERROR] Only admin can give task's to server.");
                    return;
                }

                if ((processedParts.Count != 0) && (!isWorkFinished()))
                {
                    Console.WriteLine("Server not fully completed previous work. Aborting.");
                    return;
                }

                Console.WriteLine();
                Console.WriteLine("Admin [client ID: {0}] upload task to server...", clientID);

                Console.WriteLine("Receiving task...");

                serverArray = new int[array.GetLength(0)];

                for (int i = 0; i < array.GetLength(0); ++i)
                    serverArray[i] = array[i];

                for (int i = 10; i > 2; i--)
                {
                    if (array.GetLength(0) % i == 0)
                    {
                        separatedBy = i;
                        break;
                    }
                }

                for (int i = 0; i < array.GetLength(0); i += separatedBy)
                    processedParts.Add(i);

                sendArray = new int[separatedBy];

                resultMax = serverArray[0];
                resultMin = serverArray[0];

                Console.WriteLine("Task received.");
                Console.WriteLine("Received array len: {0}\nDivided by: {1}", array.GetLength(0), separatedBy);
            }
        }


        public int[] GetClientData(int clientID)
        {
            lock ("client_data_out")
            {
                Client client = GetClientByID(clientID);

                int j = processedParts[0];
                processedParts.RemoveAt(0);

                client.SetStatus(Client.ClientStatus.BUSY);
                client.SetMeta(j);

                for (int i = 0; i < separatedBy; ++i)
                    sendArray[i] = serverArray[j + i];

                return sendArray;
            }

        }

        public void ReturnClientData(int clientID, int max, int min)
        {
            lock ("client_data_in")
            {
                Client client = GetClientByID(clientID);

                if (max > resultMax)
                    resultMax = max;

                if (min < resultMin)
                    resultMin = min;
                
                client.SetStatus(Client.ClientStatus.FREE);


                // Checkinh end
                bool flag = true;
                for (int i = 0; i < clients.Count; ++i)
                    flag &= (clients[i].GetStatus() == Client.ClientStatus.FREE);

                // Server ended
                if (flag && isWorkFinished())
                {
                    Console.WriteLine("Server solved task");
                    Console.WriteLine("Array max: {0}\nArray min: {1}", resultMax, resultMin);
                }


            }
        }

        public bool isWorkFinished()
        {
            lock ("finished")
            {
                return (processedParts.Count == 0);
            }
        }
    }
}
