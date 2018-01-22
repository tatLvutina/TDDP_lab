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
        frmChatWinAdmin objChatWinAdmin;
        
        public frmLogin()
        {
            InitializeComponent();
        }

        public static class BaseInfo
        {
            // переменная статического класса будет доступна откуда угодно в пределах проекта
            public static int ClientId;
        }

        private void btnJoin_Click(object sender, EventArgs e)
        {
            JoinToServer();
        }
        private void JoinToServer()
        {
            if (chan == null)
            {
                chan = new TcpChannel();
                ChannelServices.RegisterChannel(chan,false);

                // Create an instance of the remote object
                objChatWin = new frmChatWin();
                objChatWin.remoteObj = (SampleObject)Activator.GetObject(typeof(RemoteBase.SampleObject), txtServerAdd.Text);

                objChatWinAdmin = new frmChatWinAdmin();
                objChatWinAdmin.remoteObj = (SampleObject)Activator.GetObject(typeof(RemoteBase.SampleObject), txtServerAdd.Text);

                try
                {
                    BaseInfo.ClientId = objChatWin.remoteObj.JoinToChatRoom();
                
                    if (BaseInfo.ClientId == -1)
                    {
                        MessageBox.Show(BaseInfo.ClientId + " already joined, please try with different id");
                        ChannelServices.UnregisterChannel(chan);
                        chan = null;
                        objChatWin.Dispose();
                        return;
                    }

                    if (BaseInfo.ClientId == 0)
                    {
                        //MessageBox.Show("You are admin!");
                        this.Hide();
                        objChatWinAdmin.Show();
                    }
                    else { 
                        this.Hide();
                        objChatWin.Show();
                    }
                }
                catch
                {
                    MessageBox.Show("Error! Try restart the server");
                    ChannelServices.UnregisterChannel(chan);
                    chan = null;
                    objChatWin.Dispose();
                }
                
            }
        }

    }
}