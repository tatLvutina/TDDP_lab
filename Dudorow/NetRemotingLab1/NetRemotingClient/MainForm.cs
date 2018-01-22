// Connect default namespaces
using System;
using System.Windows.Forms;

// Connect SharedLibrary namespace
using Library;

// Connect namespace for .NET Remoting
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Services;
using System.Runtime.Remoting;

namespace Client
{
    public partial class MainForm : Form
    {
        /// <summary>
        /// Contructor
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
        }

        // Connection form string, like for printf function
        const string connectFormatString = "tcp://localhost:{0}/{1}";

        // Default server connection port
        const int defaultPort = 2222;

        // Default server name for instance of SharedLibrary
        const string defaultClassName = "Library";

        // Variable for storing reference of SharedLibrary on server
        private Lib remoteObject = null;


        // Calculate button event handler
        private void buttonCalculate_Click(object sender, EventArgs e)
        {
            // Connect to server if not already
            if (remoteObject == null)
            {
                // Opening channel
                TcpChannel channel = new TcpChannel();

                // Registering channel
                ChannelServices.RegisterChannel(channel, true);

                // Building connection string
                string connectionString = string.Format(connectFormatString, defaultPort, defaultClassName);

                // Getting reference for instance of library
                remoteObject = (Lib)Activator.GetObject(typeof(Lib), connectionString);
            }

            double argument = 0.0;
            double result = 0.0;

            // Trying to parse number in text field, if not success then display error
            if (!double.TryParse(textBoxArgument.Text, out argument))
            {
                MessageBox.Show("Enter DOUBLE number in correct format. \nWARNING: separator in ',' symbol", "Error");
                return;
            }
            else
            {
                // Call server object for cos calculating
                result = remoteObject.Calculate(argument);

                // Display answer
                labelAnswer.Text = string.Format("Answer: {0}", result);
            }

        }


      
    }
}
