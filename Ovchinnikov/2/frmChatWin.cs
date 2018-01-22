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
        internal int item = 0;
        internal string yourName;
        String str;//массив чисел
        internal int IDcl = 0;//id
        double skolko = 0;
        Boolean is_click = true;
        String str_parts;
        double sum = 0;
        object locker = new object();
        List<int> int_mas = new List<int>();
        double sred = 0;


        public frmChatWin()
        { InitializeComponent(); }

        private void btnSend_Click(object sender, EventArgs e)
        { if (is_click) { SendMessage(); is_click = false; } }

        //int skipCounter = 4;
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (remoteObj != null)
            {
                string tempStr = remoteObj.GetMsgFromSvr(item);
                if (tempStr.Trim().Length > 0)
                { item++; }
                ArrayList onlineUser = remoteObj.GetOnlineUser();
            }
        }

        private void SendMessage()
        {
            if (remoteObj != null)
            {
                if (IDcl > 0)
                {
                    str = remoteObj.take_mas();
                    //skolko=эл-в
                    for (int i = 0; i < str.Length; i++)
                    { if (str[i] == ' ') { skolko++; } }

                    skolko++; remoteObj.skolko = skolko;
                    String t = "";
                    double n = 0;
                }
                // str_parts = remoteObj.take_parts();
                lock (locker) { }
                func(str);
                System.Threading.Thread.Sleep(500);

                if (IDcl > 0)
                {
                    label2.Text = sred.ToString();
                    label4.Text = sum.ToString();
                }
            }
        }
        //лив
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (remoteObj != null) { remoteObj.LeaveChatRoom(yourName); }
            Application.Exit(); IDcl--;
        }
        //ага
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        { System.Diagnostics.Process.Start("iexplore.exe", "http://socketprogramming.blogspot.com"); }
        //умрём без этого
        private void label2_Click(object sender, EventArgs e)
        { }
        //id вверху
        private void frmChatWin_Load(object sender, EventArgs e)
        { label1.Text = IDcl.ToString(); }

        private void func(String mas)
        {
            int_mas.Clear();
            //если в начале пробел, пнуть дворника
            if (mas[0] == ' ')
            { mas = mas.Remove(0, 1); }
            //если в конце пробел, пнуть дворника
            if (mas[mas.Length - 1] == ' ')
            { mas = mas.Remove(mas.Length - 1, 1); }

            String temp = "";
            for (int i = 0; i < mas.Length; i++)
            {
                if (mas[i] == ' ')
                { int_mas.Add(int.Parse(temp)); temp = temp.Remove(0, temp.Length); }
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
            for (int i = 0; i < skolko; i++) { sum += int_mas[i]; }
            sred = (sum / skolko);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }
    }
}

