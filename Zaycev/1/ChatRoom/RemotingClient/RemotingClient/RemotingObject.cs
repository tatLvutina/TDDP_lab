using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Collections;

namespace RemoteBase
{
    public class SampleObject : MarshalByRefObject
    {
       
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
            hTChatMsg.Add(++key, chatMsgFromUsr);
        }
        public string GetMsgFromSvr(int lastKey)
        {
            if (key > lastKey)
                return hTChatMsg[lastKey + 1].ToString();
            else
                return "";
        }

        public double Calculation(double num1, double num2, string str)
        {
            switch (str)
            {
                case "+":
                    return (num1 + num2);

                case "-":
                    return (num1 - num2);

                case "*":
                    return (num1 * num2);

                case "/":
                    if (num2 != 0)
                    {
                        return (num1 / num2);

                    }
                    else { return 0; }
                case "sin":
                    return (Math.Sin(num1)); 

                case "cos":
                    return (Math.Cos(num1));

                default:
                    return 0;

            }
        }

    }
}
