using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Collections;

namespace RemoteBase
{
    /// <remarks>
    /// Sample object to demonstrate the use of .NET Remoting.
    /// </remarks>
    public class SampleObject : MarshalByRefObject
    {
        public int N = 1000;
        public int M = 2000;
        public int i, j, k, A;
        int[,] TaskA;
        int[] TaskB;
        int[,] TaskC;
        public int[,] Task()
        {
            TaskC = new int[N, M];
            InitArrayA();
            InitArrayB();
            for (i=0;i< N;i++)
            {
                for (j = 0; j < M; j++)
                {
                    for (k = 0; k < M; k++)
                    { A = A + TaskA[i, k] * TaskB[k]; }
                    TaskC[i, j] = A;
                }  
            }
            data = TaskC;
            return TaskC;
        }

        public void InitArrayA()
        {
            TaskA = new int[N, M];

            Random r = new Random();

            for ( i = 0; i < N; i++)
            {
                for (j = 0; j < M; j++)
                {
                    TaskA[i,j] = r.Next();
                }
            }
        }
        public void InitArrayB()
        {
            TaskB = new int[M];

            Random r = new Random();

            for (i = 0; i < M; i++)
            {
                    TaskB[i] = r.Next();
            }
        }
        public int[,] data;

        public int[,] GetData()
        {
            return data;
        }

        public void SetData(int[,] _data)
        {
            data = _data;
        }
        
        /// <summary>
        /// //////
        /// </summary>
        /// 
        Hashtable hTChatMsg=new Hashtable ();
        ArrayList alOnlineUser = new ArrayList();
        private int key = 0;
        
        public bool JoinToChatRoom(string name)
        {
            if (alOnlineUser.IndexOf(name) > -1)
                return false;
            else
            {
                alOnlineUser.Add(name);
                SendMsgToSvr(name + " has joined into chat room.");
                return true;
            }
            
        }
        public void LeaveChatRoom(string name)
        {
            alOnlineUser.Remove(name);
            SendMsgToSvr(name + " has left the chat room.");
        }
        public ArrayList GetOnlineUser()
        {
            return alOnlineUser;
        }

        public int CurrentKeyNo()
        {
            return key;
        }
        public void SendMsgToSvr(string chatMsgFromUsr)
        {
            //chatMsg = chatMsgFromUsr;
            hTChatMsg.Add(++key, chatMsgFromUsr);
        }
        public string GetMsgFromSvr(int lastKey)
        {
            if (key > lastKey)
                return hTChatMsg[lastKey + 1].ToString();
            else
                return "";
        }
    }
}
