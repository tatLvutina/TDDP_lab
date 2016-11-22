using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetRemotingLibrary
{
    public class Client
    {
        public enum ClientStatus { BUSY, FREE }

        private int id = 0;
        private ClientStatus status = ClientStatus.FREE;
        private bool hadAdminRights = false;
        private DateTime lastInteraction = DateTime.Now;
        private int meta = 0;

        public Client(int clientID)
        {
            id = clientID;
            status = ClientStatus.FREE;
            hadAdminRights = false;
        }

        public void SetAdmin(bool admin)
        {
            hadAdminRights = admin;
        }

        public bool GetAdmin()
        {
            return hadAdminRights;
        }

        public int GetID()
        {
            return id;
        }

        public void SetStatus(ClientStatus clientStatus)
        {
            status = clientStatus;
            lastInteraction = DateTime.Now;
        }

        public DateTime GetTimeSinceLastInteraction()
        {
            return lastInteraction;
        }

        public ClientStatus GetStatus()
        {
            return status;
        }

        public int GetMeta()
        {
            return meta;
        }

        public void SetMeta(int clientMeta)
        {
            meta = clientMeta;
        }
    }


    public class RemotingLibrary : MarshalByRefObject
    {
        public static int[] Sort(int[] array)
        {
            return array;
        }


        private static int globalID = 0;
        private static List<Client> clients = new List<Client>();

        private int[,] serverTask;
        private bool[] rowProcessed;
        private int[] clientData;


        private System.Timers.Timer timer = new System.Timers.Timer();

        public RemotingLibrary()
        {
            timer.Elapsed += timer_Elapsed;
            timer.Interval = 1000;
            timer.Start();
        }

        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            lock ("timer")
            {
                for (int i = clients.Count - 1; i >= 0; --i)
                {
                    if ((clients[i].GetStatus() == Client.ClientStatus.BUSY) && (DateTime.Now - clients[i].GetTimeSinceLastInteraction()).TotalMilliseconds > 3000)
                    {
                        Console.WriteLine("Client {0} disconnects!", clients[i].GetID());

                        int rowID = clients[i].GetMeta();
                        rowProcessed[rowID] = false;

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
            lock ("client_connect_request")
            {
                globalID++;

                Client client = new Client(globalID);

                bool hasAdmin = false;
                for (int i = 0; i < clients.Count; ++i)
                    hasAdmin |= clients[i].GetAdmin();

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
            lock ("unregister_client")
            {
                for (int i = 0; i < clients.Count; ++i)
                    if (clients[i].GetID() == clientID)
                        clients.RemoveAt(i);
            }
        }

        public void UploadTaskToServer(int clientID, int[,] task)
        {
            lock ("admin_upload_task")
            {
                Client client = GetClientByID(clientID);

                if ((client == null) || (!client.GetAdmin()))
                {
                    Console.WriteLine("[ERROR] Only admin can give task's to server.");
                    return;
                }

                if ((rowProcessed != null) && (!isWorkFinished()))
                {
                    Console.WriteLine("Server not fully completed previous work. Aborting.");
                    return;
                }

                Console.WriteLine();
                Console.WriteLine("Admin [client ID: {0}] upload task to server...", clientID);

                serverTask = new int[task.GetLength(0), task.GetLength(1)];
                rowProcessed = new bool[task.GetLength(0)];
                clientData = new int[task.GetLength(1)];

                Console.WriteLine("Copying task...");

                for (int i = 0; i < task.GetLength(0); ++i)
                {
                    for (int j = 0; j < task.GetLength(1); ++j)
                        serverTask[i, j] = task[i, j];

                    rowProcessed[i] = false;
                }

                Console.WriteLine("Task copied");
            }
        }


        public int[] GetClientData(int clientID)
        {
            lock ("data_request")
            {
                Client client = GetClientByID(clientID);
                int i = 0;
                for (; i < rowProcessed.Length; ++i)
                    if (rowProcessed[i] == false)
                        break;

                for (int j = 0; j < serverTask.GetLength(1); ++j)
                    clientData[j] = serverTask[i, j];

                rowProcessed[i] = true;
                client.SetStatus(Client.ClientStatus.BUSY);
                client.SetMeta(i);

                return clientData;
            }

        }

        public void ReturnClientData(int clientID, int[] data)
        {
            lock ("data_return")
            {
                Client client = GetClientByID(clientID);
                int i = client.GetMeta();

                for (int j = 0; j < serverTask.GetLength(1); ++j)
                    serverTask[i, j] = data[j];

                client.SetStatus(Client.ClientStatus.FREE);

                bool flag = true;
                for (i = 0; i < clients.Count; ++i)
                    flag &= (clients[i].GetStatus() == Client.ClientStatus.FREE);

                // Server ended
                if (flag && isWorkFinished())
                {
                    Console.WriteLine("Server ended task");
                    Console.WriteLine("Result is: ");
                    for (i = 0; i < serverTask.GetLength(0); ++i)
                    {
                        for (int j = 0; j < serverTask.GetLength(1); ++j)
                            Console.Write("{0} ", serverTask[i, j]);

                        Console.WriteLine();
                    }
                }
            }
        }


        public bool isWorkFinished()
        {
            lock ("check_work_finished")
            {
                bool flag = true;
                for (int i = 0; i < rowProcessed.Length; ++i)
                    flag &= rowProcessed[i];

                return flag;
            }
        }
    }
}
