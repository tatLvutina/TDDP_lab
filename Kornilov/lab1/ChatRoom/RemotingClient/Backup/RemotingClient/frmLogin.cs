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
namespace RemotingClient
{
    public partial class frmLogin : Form
    {
        TcpChannel chan;        
        ArrayList alOnlineUser = new ArrayList();
        frmChatWin objChatWin;
        public frmLogin()
        {
            InitializeComponent();
        }

        private void btnJoin_Click(object sender, EventArgs e)
        {
            JoinToChatRoom();
        }
        private void JoinToChatRoom()
        {
            if (chan == null && txtName.Text.Trim().Length != 0)
            {
                chan = new TcpChannel();
                ChannelServices.RegisterChannel(chan,false);

                // Create an instance of the remote object
                objChatWin = new frmChatWin();
                objChatWin.remoteObj = (SampleObject)Activator.GetObject(typeof(RemoteBase.SampleObject), txtServerAdd.Text);

                if (!objChatWin.remoteObj.JoinToChatRoom(txtName.Text))
                {
                    MessageBox.Show(txtName.Text+ " already joined, please try with different name");
                    ChannelServices.UnregisterChannel(chan);
                    chan = null;
                    objChatWin.Dispose();
                    return;
                }
                objChatWin.key = objChatWin.remoteObj.CurrentKeyNo();
                
                objChatWin.yourName= txtName.Text;

                this.Hide();
                objChatWin.Show();
                
            }
        }
    }
}