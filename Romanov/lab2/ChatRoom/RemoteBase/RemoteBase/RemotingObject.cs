using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Collections;
using System.Collections.Generic;

namespace RemoteBase
{
    public class SampleObject : MarshalByRefObject
    {
        int counter = 0;
        Hashtable hTChatMsg = new Hashtable();
        ArrayList alOnlineUser = new ArrayList();
        private int key = 0;
        public int mas1 = 0;
        int client_id = 0;
        public int count = 0;
        public int[] parts = new int[10];
        public Boolean flag = true;
        public int count_m;
        public double[] sinus = new double[1025];
        object locker = new object();
        public double kol;
        public Boolean is_first = false;
        public bool JoinToChatRoom(string name)
        {
            if (alOnlineUser.IndexOf(name) > -1)
                return false;
            else
            {
                alOnlineUser.Add(name);
                SendMsgToSvr(name + " has joined into chat room.");
                client_id++;
                return true;
            }

        }
        public int mas()
        {
            lock (locker)
            {
                int t;
                t = mas1;
                mas1++;
                return t;
            }
        }
        public void LeaveChatRoom(string name)
        {
            alOnlineUser.Remove(name);
            SendMsgToSvr(name + " has left the chat room.");
            client_id--;
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

            hTChatMsg.Add(++key, chatMsgFromUsr);
        }
        public string GetMsgFromSvr(int lastKey)
        {
            if (key > lastKey)
                return hTChatMsg[lastKey + 1].ToString();
            else
                return "";
        }

        public int get_client_id()
        {
            return client_id;
        }

        public void adding_sinus(double item)
        {
            lock (locker)
            {
                sinus[counter] = item;
                counter++;
            }
        }
    }
}
