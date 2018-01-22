using System;
using System.Windows.Forms;
using System.Configuration;
using Sample.RemoteObject;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace Sample.Client
{
    public enum Operation : int    {
        None,
        Division
    }

    public partial class Client : Form    {
        private string _result = "0";
        private int _number ;
        private Operation _operation;
        //Proxy
        private RemoteCalculator _remoteCalculator;
        private string _server = "localhost";
        private string _port = "1235";
        private TcpChannel _clientChannel;

        public Client()        {
            InitializeComponent();
            txtResult.Text = "0";;
            lblServer.Text = _server;
            lblPort.Text = _port;
        }

        private void btn_Click(object sender, EventArgs e)       {
            _result = _result + ((Button)sender).Text;
            txtResult.Text = Convert.ToString(Int32.Parse(_result));
        }

        //private void btnMultiply_Click(object sender, EventArgs e)
        private void btnDivision_Click(object sender, EventArgs e)       {
            _operation = Operation.Division;
            _number = Int32.Parse(_result);
            _result = "";
            txtResult.Text = "";
        }

        private void btnEquals_Click(object sender, EventArgs e)        {
            try            {
                if (_operation.Equals(Operation.Division))                {
                    _result = Calculate(Operation.Division, Int32.Parse(_result), _number);
                }
                txtResult.Text = _result;
            }
            catch (Exception ex)            {
                MessageBox.Show(ex.Message,this.Text);
            }

        }

        private void btnCancel_Click(object sender, EventArgs e)        {
            txtResult.Text = "";
            _result = "";
            _number = 0;
            _operation = Operation.None;
            txtResult.Text = "0";
        }

        /// <divis>
        /// Division is done at the server through remoting
        /// </divis>
        private string Calculate(Operation operation, int number1, int number2)        {
            //check whether channel is created
            if (_clientChannel == null)            {
                //Create the channel
                _clientChannel = new TcpChannel();
                //Register  the channel 
                ChannelServices.RegisterChannel(_clientChannel, true);
            }
            //Create a proxy object to access the remote calculator
            _remoteCalculator = (RemoteCalculator)Activator.GetObject(typeof(RemoteCalculator), "tcp://" + _server + ":" + _port + "/RemoteCalculator");
            
            return Convert.ToString(_remoteCalculator.Division(number1, number2));
        }

        private void Client_Load(object sender, EventArgs e)       {

        }
    }
}