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
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
namespace RemotingClient
{
    public partial class frmChatWin : Form
    {
        bool canGetTask = false;
        internal SampleObject remoteObj;
        public frmChatWin()
        {
            InitializeComponent();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {

            //SendSolution();
        }

        private long findsol(int a, int b)
        {
            return a * b;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (remoteObj != null)
            {
                label1.Text = "Client id: " + RemotingClient.frmLogin.BaseInfo.ClientId.ToString();
                if (remoteObj.CheckSrvReady() && RemotingClient.frmLogin.BaseInfo.ClientId != 0)
                {
                   if (remoteObj.CheckSrvReady())
                   {
                       string task = remoteObj.GetTaskFromSvr();
                       
                       int taskLenght = task.Length;

                       int a = 0;
                       int b = 0;

                       string numS = "";
                       int numParsed = 0;
                       int numInt = 0;
                       char c = '_';
                       for (int i = 0; i < taskLenght; i++)
                       {
                           c = task[i];
                           if (c != ' ')
                           {
                               numS = numS + c;
                           }
                           else
                           {
                               numParsed++;
                               numInt = Int32.Parse(numS);
                               if (numParsed == 1)
                               {
                                   a = numInt;
                               }
                               if (numParsed == 2)
                               {
                                   b = numInt;
                               }
                               numS = "";
                           }
                       }

                       remoteObj.SendSolutionToSvr(findsol(a, b));
                       label3.Text = findsol(a, b).ToString();
                   }
                }
            }
        }        
        
    
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (remoteObj != null)
            {
                remoteObj.LeaveChatRoom(RemotingClient.frmLogin.BaseInfo.ClientId);             
            }
            Application.Exit();
        }

        private void frmChatWin_Load(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            canGetTask = true;
        }



       
    }
}

