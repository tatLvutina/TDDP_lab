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

        Hashtable hTChatMsg = new Hashtable();
        ArrayList alOnlineUser = new ArrayList();
        private int key = 0;
        String mas = "-4 15 -2 0 13 11 36 -4 15 -2 0 13 11 36 -4 15 -2 0 13 11 36 -4 15 -2 0 13 11 36 -4 15 -2 0 13 11 36 -4 15 -2 0 13 11 36 -4 15 -2 0 13 11 36 -4 15 -2 0 13 11 36 -4 15 -2 0 13 11 36";
        int client_id = 0;
        public int count = 0;
        public String[] parts = new String[10];
        public Boolean flag = true;
        public int count_m;
        public double kol;
        public List<int> mul= new List<int>();
        object locker = new object();
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
        public String take_mas()
        {
            return mas;
        }
        public int get_client_id()
        {
            return client_id;
        }


        public String take_parts()
        {
            lock (locker)
            {
                String temp;
                temp = parts[count];
                count++;
                return temp;
            }
        }

        public int getCount()
        {
            return count;
        }
        public void adding_mul(int item)
        {
            lock (locker)
            {
                mul.Add(item);
            }
        }
    }
}
