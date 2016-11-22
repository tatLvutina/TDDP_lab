using System;
using System.Windows.Forms;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;

namespace client
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        const int SYS_PORT = 9232;
        private lib.lib remotingClass = null;

        private void Connect()
        {
            if (remotingClass == null)
            {
                TcpChannel clientChannel = new TcpChannel();
                ChannelServices.RegisterChannel(clientChannel, true);
                remotingClass = (lib.lib)Activator.GetObject(typeof(lib.lib),
                    string.Format("tcp://localhost:{0}/TestClass", SYS_PORT));
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

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

                    if ((i == 0) && text[i] == '-')
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

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                double a = double.Parse(textBox1.Text.Replace('.', ','));
                double b = double.Parse(textBox2.Text.Replace('.', ','));
 
                if (remotingClass == null)
                    Connect();

                double root = remotingClass.Sum(a,b);

                textBox3.Text = string.Format("{0}", root.ToString());
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox1.Text = Filter(textBox1.Text);
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            textBox2.Text = Filter(textBox2.Text);
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
        
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
