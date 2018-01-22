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

                //if (skipCounter > 3)
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
                //else
                  //  skipCounter++;
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
    }
}

