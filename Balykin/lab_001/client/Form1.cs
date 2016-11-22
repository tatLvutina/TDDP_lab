using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using NetRemotingLibrary;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Services;
using System.Runtime.Remoting;

namespace client
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        const int SYS_PORT = 9232;
        private lib remotingClass = null;

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
            textBox3.Text = Filter(textBox3.Text);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
