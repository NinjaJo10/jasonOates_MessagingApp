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
        bool continueRepeatingTask = true;

        public MainPage()
        {
            InitializeComponent();
            DatabaseHandler.dbSetup();
            sendRepeater();
            receiveRepeater();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            continueRepeatingTask = false;
        }

        public void sendRepeater()
        {
            Device.StartTimer(TimeSpan.FromSeconds(30), () =>
            {
                if (continueRepeatingTask == true)
                {
                    Debug.WriteLine("Checking if MsgList has anything in it");
                    if (DatabaseHandler.msg_collection == null)
                    {
                        if (DatabaseHandler.tryReconnectToDB())
                        {
                            if (DatabaseHandler.msgsToSend.Count != 0)
                            {
                                DatabaseHandler.sendMsg();
                            }
                        }
                    }
                }
                return continueRepeatingTask;
            });
        }

        public void receiveRepeater()
        {
            Device.StartTimer(TimeSpan.FromSeconds(10), () =>
            {
                if (continueRepeatingTask == true)
                {
                    Debug.WriteLine("Checking if MsgList has anything in it");
                    if (DatabaseHandler.msg_collection != null)
                    {
                        DatabaseHandler.getMsgsFromDB();
                    }
                }
                return continueRepeatingTask;
            });
        }

        private void sendMsgButton_Clicked(object sender, EventArgs e)
        {
            string msgText = messageInput.Text.ToString();
            MsgClass thisMsg = new MsgClass(ThisUser, msgText);

            Debug.WriteLine(thisMsg.user + " " + thisMsg.message + " " + thisMsg.sentTime);
            DatabaseHandler.msgToBsonList(thisMsg);
            messageInput.Text = "";

            DatabaseHandler.sendMsg();
        }
    }
}
