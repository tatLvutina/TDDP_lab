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
        String[] parts;//массив разделенный
        internal int cclient_id = 0;//id
        Boolean is_click = true;      
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
            
            if (remoteObj != null)
            {

                    while (remoteObj.mas1<1025)
                    {
                        remoteObj.adding_sinus(Math.Sin((2 * Math.PI * remoteObj.mas()) / 1024));
                    counter++;
                        System.Threading.Thread.Sleep(15);
                    }
                label2.Text = counter.ToString();
                if (cclient_id == 1)
                    {

                    for (int i = 0; i < 1025; i++)
                        {
                            textBox1.Text += remoteObj.sinus[i].ToString()+Environment.NewLine;
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
    }
}

