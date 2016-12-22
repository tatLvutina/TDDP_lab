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
        public frmChatWin()
        {
            InitializeComponent();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {

            SendMessage();
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
                    txtAllChat.Text = txtAllChat.Text + "\n" + tempStr;
                }

                {
                    ArrayList onlineUser = remoteObj.GetOnlineUser();
                    lstOnlineUser.DataSource = onlineUser;
                    skipCounter = 0;

                    if (onlineUser.Count < 2)
                    {
                        txtChatHere.Text = "Please wait untill atleast two user join in Chat Room.";
                        txtChatHere.Enabled = false;
                    }
                    else if(txtChatHere.Text == "Please wait untill atleast two user join in Chat Room." && txtChatHere.Enabled == false)
                    {
                        txtChatHere.Text = "";
                        txtChatHere.Enabled = true;
                    }
                }
            }
        }        
        private void SendMessage()
        {

            if (remoteObj != null && txtChatHere.Text.Trim().Length>0)
            {
                remoteObj.SendMsgToSvr(yourName + " says: " + txtChatHere.Text);
                txtChatHere.Text = "";
            }
        }
        //вычисление интеграла и отправка на сервер
        private void PerformIntegral(int a, int b, int n)
        {
            int a1, b1, n1;
            int h = 0;
            double result;
            a1 = a;
            b1 = b;
            n1 = n;
            h = (a1 + b1) / n1;
            result = h * (((Func(a) + Func(b)) / 2)*(b-a));
            MessageBox.Show("Вополнено: "+result);
            remoteObj.SetData(result);
            MessageBox.Show("Информация отправлена на сервер");
        }

        public int tNumb = 0;
        //моя функция
        private int Func(int x){
            return x * x * 2 - x + 5;
        }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (remoteObj != null)
            {
                remoteObj.LeaveChatRoom(yourName);             
                txtChatHere.Text = "";
            }
            Application.Exit();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("iexplore.exe", "http://socketprogramming.blogspot.com");
        }
        //запуск
        private void button1_Click(object sender, EventArgs e)
        {
            int a = -1;
            int b = -1;
            int n = -1;
            a = remoteObj.getA();
            b = remoteObj.getB();
            n = remoteObj.getN();
            if (b == -1)
            {
                MessageBox.Show("Все задачи выполнены!");
                return;
            }
            tNumb++;
            MessageBox.Show("Номер задачи: "+tNumb);
            MessageBox.Show(String.Format("Ваша часть a={0},b={1},n={2}", a, b, n));
            PerformIntegral(a,b,n);
        }
        //информация, находящаяся на сервере
        private void button2_Click(object sender, EventArgs e)
        {
            double data1;
            data1 = remoteObj.GetData();
            MessageBox.Show("Информация на сервере: " + data1);
    
        }
        //генерация задачи
        private void button3_Click(object sender, EventArgs e)
        {
            int flag = 0;
            flag = remoteObj.GenTask();
            if (flag == 1)
            {
                MessageBox.Show("Задача сгенерирована");
            }
            else
            {
                MessageBox.Show("Задача уже сгенерированы");
            }
        }
        //информация о интервале и номере задачи
        private void button4_Click(object sender, EventArgs e)
        {
            int gA = -1, gB = -1,gN= -1, tN = -1;
            gB = remoteObj.get_B();
            gA = remoteObj.get_A();
            tN = remoteObj.getNumTask();
            gN = remoteObj.getN();
            MessageBox.Show("Весь интервал: a=" + gA + " b=" + gB + " n=" + gN);
            MessageBox.Show("Задача выполнена: "+tNumb+" из "+tN);

        }
    }
}

