using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Collections;
using System.Threading;
using System.Collections.Generic;

namespace RemoteBase
{
    public class SampleObject : MarshalByRefObject
    {
        Hashtable hTChatMsg = new Hashtable();
        ArrayList alOnlineUser = new ArrayList();
        public int clientId = 0;
        string taskForClient = "";

        List<string> tasks = new List<string>();
        int tasks_processed = 0;
        long result = 0;
        bool server_ready = false;

        public static Mutex mtx4 = new Mutex();
        public int JoinToChatRoom()
        {
            mtx4.WaitOne(5);
            alOnlineUser.Add(clientId);
            clientId++;
            mtx4.ReleaseMutex();
            return clientId - 1;
        }
        public static Mutex mtx3 = new Mutex();
        public void LeaveChatRoom(int id)
        {
            mtx3.WaitOne();
            alOnlineUser.Remove(id);
            if (alOnlineUser.Count == 0)
                clientId = 0;
            mtx3.ReleaseMutex();
        }
        public ArrayList GetOnlineUser()
        {
            return alOnlineUser;
        }

        public static Mutex mtx2 = new Mutex();
        public void SendSolutionToSvr(long res)
        {
            mtx2.WaitOne();
            result += res;
            mtx2.ReleaseMutex();
        }

        public long GetResultFromSvr()
        {
            return result;
        }


        public static Mutex mtx = new Mutex();
        public string GetTaskFromSvr()
        {
            int num_of_task = 0;
            mtx.WaitOne();
            if (tasks_processed != tasks.Count)
            {
                num_of_task = tasks_processed;
                tasks_processed++;
                mtx.ReleaseMutex();
                return tasks[num_of_task];
            }
            else
            {
                server_ready = false;
                mtx.ReleaseMutex();
                return "";
            }
        }

        public bool CheckSrvReady()
        {
            return server_ready;
        }
        public void SendTaskForClientToSvr(String s)
        {
            taskForClient = s;

            if (alOnlineUser.Count != 0)
            {
                int taskLenght = taskForClient.Length;

                string task = "";
                string numS = "";
                int numParsed = 0;
                int numInt = 0;
                char c = '_';
                for (int i = 0; i < taskLenght; i++)
                {
                    c = taskForClient[i];
                    if (c != ' ')
                    {
                        numS = numS + c;
                    }
                    else
                    {
                        if (numParsed % 2 != 0)
                        {
                            numParsed++;
                            task = task + " " + numS + " ";
                            tasks.Add(task);
                            task = "";
                            numS = "";
                            continue;
                        }
                        numParsed++;
                        //numInt = Int32.Parse(numS);
                        task = numS;
                        numS = "";
                    }
                }
            }
            server_ready = true;
            taskForClient = "";
        }
    }
}
