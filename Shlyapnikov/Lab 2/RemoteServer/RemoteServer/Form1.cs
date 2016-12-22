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

namespace RemoteServer
{
    public partial class Form1 : Form
    {
        internal SampleObject remoteObj;

        public Form1()
        {
            InitializeComponent();
        }
        TcpChannel channel;
        private void btnStart_Click(object sender, EventArgs e)
        {
            if (channel == null)
            {
                channel = new TcpChannel(8080);
                ChannelServices.RegisterChannel(channel, false);
                RemotingConfiguration.RegisterWellKnownServiceType(typeof(SampleObject), "ChatRoom", WellKnownObjectMode.Singleton);

                lblStatus.Text = "Running...";
                btnStart.Enabled = false;
                btnStop.Enabled = true;

                remoteObj = (SampleObject)Activator.GetObject(typeof(RemoteBase.SampleObject), "tcp://localhost:8080/ChatRoom");

                //remoteObj.SendTaskToClient("This is TASK for id =");
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (channel != null)
            {
                ChannelServices.UnregisterChannel(channel);
                channel = null;
                lblStatus.Text = "Stopped.";
                btnStart.Enabled = true;
                btnStop.Enabled = false;
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (remoteObj != null)
            {
                ArrayList onlineClients = remoteObj.GetOnlineUser();
                label3.Text = onlineClients.Count.ToString();
                listBox1.DataSource = onlineClients;
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

    }
}

