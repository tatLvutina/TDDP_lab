using RemoteBase;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace RemotingClient
{
    public partial class frmChatWinAdmin : Form
    {
        internal SampleObject remoteObj;

        public frmChatWinAdmin()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(remoteObj != null)
                remoteObj.SendTaskForClientToSvr(textBox1.Text);
            else
                MessageBox.Show("Error!");
        }

        private void frmChatWinAdmin_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (remoteObj != null)
            {
                remoteObj.LeaveChatRoom(RemotingClient.frmLogin.BaseInfo.ClientId);
            }
            Application.Exit();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (remoteObj != null)
            {
                label3.Text = remoteObj.GetResultFromSvr().ToString();
            }
        }
    }
}
