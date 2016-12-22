using System;
using System.Collections.Generic ;
using System.ComponentModel ;
using System.Data ;
using System.Drawing ;
using System.Text ;
using System.Windows.Forms;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Collections ;
using RemoteBase ;
namespace RemotingClient
{
public partial class frmChatWin : Form
{
internal SampleObject remoteObj ;
internal int key = 0;
internal string yourName;
ArrayList alOnlineUser = new ArrayList () ;
public frmChatWin ()
{
InitializeComponent () ;
}
private void btnSend_Click ( object sender , EventArgs e)
{
SendMessage(1) ; //minus
}
int skipCounter = 4;
private void timer1_Tick ( object sender , EventArgs e)
{
if (remoteObj != null )
{
string tempStr = remoteObj .GetMsgFromSvr(key) ;
if (tempStr . Trim () . Length > 0)
{key++;
}
ArrayList onlineUser = remoteObj . GetOnlineUser () ;
skipCounter = 0;
}
}
private void SendMessage( int f )
{
if (remoteObj != null && txtChatHere . Text . Trim () . Length>0)
{

switch ( f )
{
case 1:
label2 . Text = (remoteObj . minus( float . Parse (txtChatHere . Text) , float . Parse ( textBox1 .Text) ) ) . ToString () ;
break ;
case 2:
label2 . Text = (remoteObj .mul( float . Parse (txtChatHere . Text) , float . Parse ( textBox1 .Text) ) ) . ToString () ;
break ;
case 3:
label2 . Text = (remoteObj . plus ( float . Parse (txtChatHere . Text) , float . Parse ( textBox1 .Text) ) ) . ToString () ;
break ;
case 4:
label2 . Text = (remoteObj . div ( float . Parse (txtChatHere . Text) , float . Parse ( textBox1 .Text) ) ) . ToString () ;
break ;
}}
}

    private void Form1_FormClosed( object sender ,
FormClosedEventArgs e)
{
if (remoteObj != null )
{
remoteObj . LeaveChatRoom(yourName) ;
txtChatHere . Text = "";
}
Application . Exit () ;
}

private void linkLabel1_LinkClicked ( object sender ,
LinkLabelLinkClickedEventArgs e)
{
System . Diagnostics . Process . Start ( " iexplore . exe" , " http ://socketprogramming . blogspot .com" ) ;
}
private void label2_Click ( object sender , EventArgs e)
{
// return SendMessage() ;
}
private void txtChatHere_TextChanged( object sender , EventArgs
e)
{
}
private void textBox1_TextChanged( object sender , EventArgs e)
{
}
private void txtAllChat_TextChanged ( object sender , EventArgs e
)
{}
private void lstOnlineUser_SelectedIndexChanged ( object sender ,
EventArgs e)
{
}
private void button2_Click ( object sender , EventArgs e)
{
SendMessage(2) ; //umnozh
}
private void button1_Click ( object sender , EventArgs e)
{
SendMessage(3) ; // plus
}
private void button3_Click ( object sender , EventArgs e)
{
SendMessage(4) ; // div
}
private void frmChatWin_Load( object sender , EventArgs e)
{
}
}
}