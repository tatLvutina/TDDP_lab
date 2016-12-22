using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Collections;


namespace RemoteBase
{

    [Serializable]
    public class SampleObject : MarshalByRefObject
    {     
        public double[] array;
        public double data = 0;
        public int a = 0, b = 0, n = 0,a1=0;
        public int flag = 0;
        public int NumTask = 0;
        public int i2 = 0;
        public Object thisLock = new Object();
        public int GenTask()        //функция генерации задачи
        {
            lock (thisLock) {
               if (flag == 0) { 
            Random r = new Random();
            a = r.Next(0, 5);
            a1 = a;
            b = r.Next(5, 100);
            n = r.Next(1, 4);
            NumTask = r.Next(1, 10);
            flag++;
            return flag;
               }
               else
               {
                   flag = 2;
                   return flag;  
               }
            }
        }

        public int getA()
        {
            lock (thisLock)
            {
                return a1;
                }
        }
        public int getB()  
        {
            lock (thisLock)
            {
                if (i2 < NumTask)
                {
                    i2++;
                    a1 = b * i2 / NumTask;
                    return a1;
                }
                else { return -1; }
            }
        }

        public int get_B() //Функция для возвращения главного значения b
        {
            lock (thisLock)
            {
                return b;
            }
        }
    
        public int get_A() //Функция для возвращения главного значения а
        {
            lock (thisLock)
            {
                return a;
                }
        }

        public int getNumTask() //Функция для возвращения главного значения количества заданий всего
        {
            lock (thisLock)
            {
                return NumTask;
            }
        }

        public int getN() //Функция для возвращения главного значения n
        {
            lock (thisLock)
            {
                return n;
            }
        }

        public void SetData(double _data)
        {
            lock (thisLock)
            {
                data = data+_data;
            }
        }
        public double GetData()
        {
            lock (thisLock)
            {
                return data;
            }
        }

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
