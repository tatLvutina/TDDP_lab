using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Services;
using System.Runtime.Remoting;

namespace NetRemotingClient
{
    public partial class EnteringForm : Form
    {
        public EnteringForm()
        {
            InitializeComponent();
        }


        const int SYS_PORT = 9232;
        private NetRemotingLibrary.RemotingLibrary remotingClass = null;



        private void Connect()
        {
            if (remotingClass == null)
            {
                TcpChannel clientChannel = new TcpChannel();
                ChannelServices.RegisterChannel(clientChannel, true);
                remotingClass = (NetRemotingLibrary.RemotingLibrary)Activator.GetObject(typeof(NetRemotingLibrary.RemotingLibrary), string.Format("tcp://localhost:{0}/TestClass", SYS_PORT));
            }
        }



        private void buttonSend_Click(object sender, EventArgs e)
        {
            try
            {
                double a = double.Parse(textBoxA.Text.Replace('.', ','));
                double b = double.Parse(textBoxB.Text.Replace('.', ','));
                double c = double.Parse(textBoxC.Text.Replace('.', ','));
                double d = double.Parse(textBoxD.Text.Replace('.', ','));

                if (remotingClass == null)
                    Connect();

                double root = remotingClass.Calculate(a, b, c, d);

                labelResult.Text = string.Format("Result: {0}", root.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

        }

        private string Filter(string text)
        {
            string result = "";
            bool separatorFound = false;
            for (int i = 0; i < text.Length; ++i)
            {
                if (separatorFound)
                {
                    if (text[i] == '.')
                        return result;

                    if ((text[i] >= '0') && (text[i] <= '9'))
                        result += text[i];
                }
                else
                {
                    if ((text[i] >= '0') && (text[i] <= '9'))
                        result += text[i];

                    if (text[i] == '.')
                    {
                        result += text[i];
                        separatorFound = true;
                    }
                }
            }

            return result;
        }

        private void textBoxA_TextChanged(object sender, EventArgs e)
        {
            textBoxA.Text = Filter(textBoxA.Text);
        }

        private void textBoxB_TextChanged(object sender, EventArgs e)
        {
            textBoxB.Text = Filter(textBoxB.Text);
        }

        private void textBoxC_TextChanged(object sender, EventArgs e)
        {
            textBoxC.Text = Filter(textBoxC.Text);
        }

        private void textBoxD_TextChanged(object sender, EventArgs e)
        {
            textBoxD.Text = Filter(textBoxD.Text);
        }
    }
}
