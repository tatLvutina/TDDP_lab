using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Collections;
namespace RemoteBase
{
    public class SampleObject : MarshalByRefObject
    {
        Hashtable hTChatMsg = new Hashtable();
        ArrayList alOnlineUser = new ArrayList();
        private int key = 0;
        public bool JoinToChatRoom(string name)
        {
            if (alOnlineUser.IndexOf(name) > -1)
                return false;
            else
            {
                alOnlineUser.Add(name);
                return true;
            }
        }
        public void LeaveChatRoom(string name)
        {
            alOnlineUser.Remove(name);
            SendMsgToSvr(name);
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
        public double Sinus(double a)
        {
            return (Math.Sin(a));
        }
    }}