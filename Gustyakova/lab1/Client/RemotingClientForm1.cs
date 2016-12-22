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
   
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (remoteObj != null)
            {
                string tempStr = remoteObj.GetMsgFromSvr(key);
                if (tempStr.Trim().Length > 0)
                {
                    key++;
                }

                //if (skipCounter > 3)
                {
                    ArrayList onlineUser = remoteObj.GetOnlineUser();

                }
     
            }
        }        
        private void SendMessage()
        {

            if (remoteObj != null && txtChatHere.Text.Trim().Length > 0 && textBox2.Text.Trim().Length > 0)
            {
              
                label2.Text = (remoteObj.razn(float.Parse(textBox2.Text), float.Parse(txtChatHere.Text))).ToString();
            }
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

        private void txtChatHere_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}

