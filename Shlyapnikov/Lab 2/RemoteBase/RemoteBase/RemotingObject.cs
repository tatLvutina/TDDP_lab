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

        List<TaskForClient> tasks = new List<TaskForClient>();
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

        public static Mutex mtx = new Mutex();
        public TaskForClient GetTaskFromSvr()
        {
            /*mtx.WaitOne();
            if (tasks_processed != tasks.Count)
            {
                mtx.ReleaseMutex();
                return tasks[tasks_processed++];
            }
            else
            {
                server_ready = false;
                mtx.ReleaseMutex();
                return new TaskForClient();
            }*/
            return new TaskForClient();
        }

        public bool CheckSrvReady()
        {
            return server_ready;
        }
        public void SendTaskToClient(String s)
        {
            taskForClient = s;

            if (alOnlineUser.Count != 0)
            {
                int taskLenght = taskForClient.Length;

                TaskForClient t = new TaskForClient();
                string numS = "";
                int numParsed = 0;
                int numInt = 0;
                int aa = 0, bb = 0;
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
                        numParsed++;
                        numInt = Int32.Parse(numS);
                        numS = "";
                        if (numParsed % 2 == 1)
                            aa = numInt;
                        else
                        {
                            bb = numInt;

                            tasks.Add(new TaskForClient(aa, bb));
                        }
                    }
                }
            }
            server_ready = true;
            taskForClient = "";
        }
    }
}
