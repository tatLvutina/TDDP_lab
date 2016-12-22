using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Collections;
using System.Collections.Generic;

namespace RemoteBase
{
    /// <remarks>
    /// Sample object to demonstrate the use of .NET Remoting.
    /// </remarks>
    public class SampleObject : MarshalByRefObject
    {

        Hashtable hTChatMsg = new Hashtable();
        ArrayList alOnlineUser = new ArrayList();
        private int item = 0;
        String mas = "1 2 3 -1 5 10 20 30 10 0";
        int IDc = 0;
        public int count = 0;
        public String[] parts = new String[10];
        public Boolean flag = true;
        public int count_m;
        public List<int> max = new List<int>();
        public List<int> min = new List<int>();
        object locker = new object();
        public double skolko;
        public Boolean is_first = false;
        public bool JoinToChatRoom(string name)
        {
            if (alOnlineUser.IndexOf(name) > -1)
                return false;
            else
            {
                alOnlineUser.Add(name);
                SendMsgToSvr(name + " has joined into chat room.");
                IDc++;
                return true;
            }

        }
        public void LeaveChatRoom(string name)
        {
            alOnlineUser.Remove(name);
            SendMsgToSvr(name + " has left the chat room.");
            IDc--;
        }
        public ArrayList GetOnlineUser()
        {
            return alOnlineUser;
        }

        public int CurrentItemNo()
        {
            return item;
        }
        public void SendMsgToSvr(string chatMsgFromUsr)
        {

            hTChatMsg.Add(++item, chatMsgFromUsr);
        }
        public string GetMsgFromSvr(int lastItem)
        {
            if (item > lastItem)
                return hTChatMsg[lastItem + 1].ToString();
            else
                return "";
        }
        public String take_mas()
        {
            return mas;
        }
        public int get_IDc()
        {
            return IDc;
        }
    }
}
