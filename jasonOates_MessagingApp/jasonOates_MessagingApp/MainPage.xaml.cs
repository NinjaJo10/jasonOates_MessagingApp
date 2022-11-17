using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Diagnostics;

namespace jasonOates_MessagingApp
{
    public partial class MainPage : ContentPage
    {
        string ThisUser = "Jason";

        public MainPage()
        {
            InitializeComponent();
            DatabaseHandler.dbSetup();
        }

        private void sendMsgButton_Clicked(object sender, EventArgs e)
        {
            string msgText = messageInput.Text.ToString();
            MsgClass thisMsg = new MsgClass(ThisUser, msgText);

            Debug.WriteLine(thisMsg.user + " " + thisMsg.message + " " + thisMsg.sentTime);
            DatabaseHandler.msgToBson(thisMsg);
            messageInput.Text = "";

            
            DatabaseHandler.sendMsg();
        }
    }
}
