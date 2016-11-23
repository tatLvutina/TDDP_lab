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

        private Lib remoteObject = null;

        private void buttonCalculate_Click(object sender, EventArgs e)
        {
            if (remoteObject == null)
            {
                // Opening channel
                TcpChannel channel = new TcpChannel();

                // Registering channel
                ChannelServices.RegisterChannel(channel, true);

                // Getting reference for instance of library
                remoteObject = (Lib)Activator.GetObject(typeof(Lib), "tcp://localhost:11114/equ");
            }


            string result;

            double a = 0;
            double b = 0;
            double c = 0;

            bool parsingOk = double.TryParse(textBox0.Text, out a);
            parsingOk &= double.TryParse(textBox1.Text, out b);
            parsingOk &= double.TryParse(textBox2.Text, out c);

            if (parsingOk == false)
            {
                MessageBox.Show("One or more args in wrong format. Check flields. Separator ','", "Error", MessageBoxButtons.OK);
                return;
            }

            result = remoteObject.Calculate(a, b, c);

            labelAnswer.Text = result;
        }


      
    }
}
