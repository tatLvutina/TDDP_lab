using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Collections;
using RemoteBase;
namespace RemotingClient
{
    public partial class frmChatWin : Form
    {
        internal SampleObject remoteObj;
        internal int key = 0;
        internal string yourName;
        ArrayList alOnlineUser = new ArrayList();
        String str;//массив чисел
        String[] parts;//массив разделенный
        internal int cclient_id = 0;//id
        double kol = 0;//количество чисел в массиве 
        Boolean is_click = true;      
        String str_parts;
        List<int> int_mas = new List<int>();
        int max = 0, min = 0;
        List<int> Max = new List<int>();
        List<int> Min = new List<int>();
        int counter = 0;

        public frmChatWin()
        {
            InitializeComponent();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (is_click)
            {
                SendMessage();//GO
                is_click = false;
            }
        }
        int skipCounter = 4;
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (remoteObj != null)
            {
                string tempStr = remoteObj.GetMsgFromSvr(key);
                if (tempStr.Trim().Length > 0)
                {
                    key++;
                }
                ArrayList onlineUser = remoteObj.GetOnlineUser();
                skipCounter = 0;
            }
        }
        private void SendMessage()
        {
            textBox1.Text = "Hello";
            if (remoteObj != null)
            {
                                        
                if (cclient_id == 1)
                {
                    str = remoteObj.take_mas();
                    for (int i = 0; i < str.Length; i++)
                    {
                        if (str[i] == ' ')
                        {
                            kol++;
                        }
                    }
                    kol++;
                    remoteObj.kol = kol;
                    parts = new String[(int)(Math.Ceiling(kol / 2))];//разделенный по частям массив str(исходный)

                    String t = "";
                    double n = 0;
                    int j = 0;

                    for (int i = 0; i < str.Length; i++)
                    {
                        if (str[i] == ' ')
                        {
                            n++;
                            if ((n % 2 == 0) && (n > 0))
                            {
                                parts[j] = t;
                                j++;
                                t = t.Remove(0);
                            }
                        }

                        t += str[i];
                        if (i == (str.Length - 1))
                        {
                            parts[j] = t;
                            t = t.Remove(0);
                        }
                    }
                    
                    remoteObj.parts = parts;
                   

                    remoteObj.is_first = true;//разделен ли массив
                }
                
                if (remoteObj.is_first)
                {
                    textBox1.Text = "Hello!!!!!!!!";                   
                    textBox1.Text = ((int)(Math.Ceiling(remoteObj.kol / 2))).ToString();
                    while (remoteObj.getCount() < ((int)(Math.Ceiling(remoteObj.kol / 2))))
                    {
                        str_parts = remoteObj.take_parts();
                        func(str_parts);
                        counter++;
                        textBox1.Text = counter.ToString();
                        System.Threading.Thread.Sleep(500);
                    }    
                    
                    if (cclient_id == 1)
                    {
                        Max.AddRange(remoteObj.max);
                        Min.AddRange(remoteObj.min);
                        Max.Sort();
                        Min.Sort();
                        textBox1.Text = counter.ToString();
                        label2.Text = Min[0].ToString();
                        label4.Text =Max[Max.Count - 1].ToString();
                    }
                }

            }
        }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (remoteObj != null)
            {
                remoteObj.LeaveChatRoom(yourName);
               
            }
            Application.Exit();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("iexplore.exe", "http://socketprogramming.blogspot.com");
        }

        private void label2_Click(object sender, EventArgs e)
        {
            
        }
        private void frmChatWin_Load(object sender, EventArgs e)
        {
            label1.Text = cclient_id.ToString();
        }
        private void func(String mas)
        {
            int_mas.Clear();
            if (mas[0] == ' ')
            {
                mas = mas.Remove(0, 1);
            }
            if (mas[mas.Length - 1] == ' ')
            {
                mas = mas.Remove(mas.Length - 1, 1);
            }
            String temp = "";
            for (int i = 0; i < mas.Length; i++)
            {
                if (mas[i] == ' ')
                {
                    int_mas.Add(int.Parse(temp));
                    temp = temp.Remove(0, temp.Length);
                }
                else
                {
                    temp += mas[i];
                    if (i == mas.Length - 1)
                    {
                        int_mas.Add(int.Parse(temp));
                        temp = temp.Remove(0, temp.Length);
                    }
                }
            }
            int_mas.Sort();
            max = int_mas[int_mas.Count - 1];
            min = int_mas[0];
            remoteObj.adding_max(max);                   
            remoteObj.adding_min(min);
        }
    }
}

