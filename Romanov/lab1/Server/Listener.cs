using System;
using System.Windows.Forms;
using System.Configuration;
//RemoteOject is defined in this namespace
using Sample.RemoteObject;
//Used for remoting
using System.Runtime;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;


namespace Server
{
    /// <summary>
    /// Used as a Server where the remote component is registered and made available to the clients
    /// </summary>
    public partial class Listener : Form
    {
        //Tcp Channel is being used for communicating with the clients
        private TcpChannel _serverChannel;
        private int _port;
        public Listener()
        {
            InitializeComponent();
            //Getting the port defined in the configuration file
            _port = Int32.Parse(ConfigurationManager.AppSettings["Port"]);
            lblPort.Text = Convert.ToString(_port);
        }

        private void btnListen_Click(object sender, EventArgs e)
        {
            if (_serverChannel == null)
            {
                //Registering the tcp channel
                _serverChannel = new TcpChannel(_port);
                ChannelServices.RegisterChannel(_serverChannel);
                //Registering the server component as a server activated object (SOA)
                RemotingConfiguration.RegisterWellKnownServiceType(typeof(RemoteCalculator), "RemoteCalculator", WellKnownObjectMode.Singleton);
                btnListen.Text = "Stop Listening";
            }
            else
            {
                btnListen.Text = "Start Listening";
                //Unregistering the tcp channel so that server will not be available here after
                ChannelServices.UnregisterChannel(_serverChannel);
                _serverChannel = null;
            }
        }
    }
}