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
        char func;
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
            if (func == '/' && textBox1.Text == "0")
                label2.Text = "Dividing by zero";
            else
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
                 
                }

              
                {
                    ArrayList onlineUser = remoteObj.GetOnlineUser();
                    
                    skipCounter = 0;

                   
                }
              
            }
        }        
        private void SendMessage()
        {
            

            if (remoteObj != null && txtChatHere.Text.Trim().Length>0)
            {
               
                label2.Text = (remoteObj.divNum(float.Parse(txtChatHere.Text), float.Parse(textBox1.Text),func)).ToString();
            }
        }
         
    

        private void label2_Click(object sender, EventArgs e)
        {
          
        }

        private void txtChatHere_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtAllChat_TextChanged(object sender, EventArgs e)
        {

        }

        private void lstOnlineUser_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void frmChatWin_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            func = '+';
            textBox2.Text = "+";

        }

        private void button2_Click(object sender, EventArgs e)
        {
            func = '-';
            textBox2.Text = "-";

        }

        private void button3_Click(object sender, EventArgs e)
        {
            func = '*';
            textBox2.Text = "*";

        }

        private void button4_Click(object sender, EventArgs e)
        {
            func = '/';
            textBox2.Text = "/";
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }
    }
}

