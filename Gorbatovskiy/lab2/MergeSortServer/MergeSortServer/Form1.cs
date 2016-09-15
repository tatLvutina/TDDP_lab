using System;
using System.Windows.Forms;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using MergeSortTasks;

namespace MergeSortServer
{
    public partial class Form1 : Form
    {
        private TcpChannel channel;

        public Form1()
        {
            InitializeComponent();
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            if (channel == null)
            {
                channel = new TcpChannel(8080);
                ChannelServices.RegisterChannel(channel, false);
                RemotingConfiguration.RegisterWellKnownServiceType(typeof(MergeSortTask), "MergeSort", WellKnownObjectMode.Singleton);
                label1.Text = "Merge Sort Server status: running";
                btnStart.Enabled = false;
                btnStop.Enabled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (channel != null)
            {
                ChannelServices.UnregisterChannel(channel);
                channel = null;
                label1.Text = "Merge Sort Server status: stopped";
                btnStart.Enabled = true;
                btnStop.Enabled = false;
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (channel != null)
            {
                ChannelServices.UnregisterChannel(channel);
                MessageBox.Show("Oops.. Вы забыли отключить сервер! Но, мы сделали это за Вас :)");
            }            
        }
    }
}
