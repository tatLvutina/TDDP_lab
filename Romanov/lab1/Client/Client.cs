using System;
using System.Windows.Forms;
using System.Configuration;
//Remote component is defined in the below namespace
using Sample.RemoteObject;
//Used for remoting
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;


namespace Sample.Client
{
    public enum Operation:int
    {
        None,
        Addition,
        Multiplication
    }

    public partial class Client : Form
    {
        private string _result = "0";
        private int _number ;
        private Operation _operation;
        //Proxy
        private RemoteCalculator _remoteCalculator;
        private string _server;
        private string _port;
        private TcpChannel _clientChannel;

        public Client()
        {
            InitializeComponent();
            txtResult.Text = "0";
            //Get the server name and server port from the configuration file
            _server = ConfigurationManager.AppSettings["Server"];
            _port = ConfigurationManager.AppSettings["ServerPort"];
            lblServer.Text = _server;
            lblPort.Text = _port;
        }


        private void btn_Click(object sender, EventArgs e)
        {
            _result = _result + ((Button)sender).Text;
            txtResult.Text = Convert.ToString(Int32.Parse(_result));
        }

        private void btnMultiply_Click(object sender, EventArgs e)
        {
            _operation =Operation.Multiplication;
            _number = Int32.Parse(_result);
            _result = "";
            txtResult.Text = "";

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            _operation = Operation.Addition;
            _number = Int32.Parse(_result);
            txtResult.Text = "";
            _result = "";
        }

        private void btnEquals_Click(object sender, EventArgs e)
        {
            try
            {
                if (_operation.Equals(Operation.Multiplication))
                {
                    _result = Calculate(Operation.Multiplication, Int32.Parse(_result), _number);
                }
                else if (_operation.Equals(Operation.Addition))
                {
                    _result = Calculate(Operation.Addition, Int32.Parse(_result), _number);
                }
                txtResult.Text = _result;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,this.Text);
            }

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            txtResult.Text = "";
            _result = "";
            _number = 0;
            _operation = Operation.None;
            txtResult.Text = "0";
        }

        /// <summary>
        /// Addition and Multipication is done at the server through remoting
        /// </summary>
        private string Calculate(Operation operation,int number1, int number2)
        {
            int result ;
            //check whether channel is created
            if (_clientChannel == null)
            {
                //Create the channel
                _clientChannel = new TcpChannel();
                //Register  the channel 
                ChannelServices.RegisterChannel(_clientChannel);
            }
            //Create a proxy object to access the remote calculator
            _remoteCalculator = (RemoteCalculator)Activator.GetObject(typeof(RemoteCalculator), "tcp://" + _server + ":" + _port + "/RemoteCalculator");
            if (operation.Equals(Operation.Addition))
            {
                result = _remoteCalculator.Add(number1, number2);
            }
            else
            {
                result = _remoteCalculator.Multiply(number1, number2);
            }
            return Convert.ToString(result);
        }



    }
}