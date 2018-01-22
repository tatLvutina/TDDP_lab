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
        char funckey;

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
            if (textBox2.Text == "0" && funckey == '/')
            {
                label2.Text = "Diveded by zero!";
            }
            else
            {
                SendMessage();
            }

            if (textBox1.Text == "")
                label1.Text = "Choose action!";

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

                //if (skipCounter > 3)
                {
                    ArrayList onlineUser = remoteObj.GetOnlineUser();
                    skipCounter = 0;

                }
                //else
                  //  skipCounter++;
            }
        }        
        private void SendMessage()
        {

            if (remoteObj != null && txtChatHere.Text.Trim().Length > 0 && textBox2.Text.Trim().Length > 0 && textBox1.Text.Trim().Length > 0)
            {
                //remoteObj.SendMsgToSvr(yourName + " says: " + txtChatHere.Text);
                //txtChatHere.Text = "";
                label2.Text = (remoteObj.sum(float.Parse(textBox2.Text), float.Parse(txtChatHere.Text), funckey)).ToString();
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

        private void button1_Click(object sender, EventArgs e)
        {
            funckey = '+';
            textBox1.Text = "+";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            funckey = '-';
            textBox1.Text = "-";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            funckey = '/';
            textBox1.Text = "/";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            funckey = '*';
            textBox1.Text = "*";            
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}

