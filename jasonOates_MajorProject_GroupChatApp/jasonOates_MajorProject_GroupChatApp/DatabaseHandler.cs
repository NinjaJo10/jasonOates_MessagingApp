using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;

namespace jasonOates_MajorProject_GroupChatApp
{
    public class DatabaseHandler
    {
        public static MongoClient dbClient;
        public static IMongoDatabase messageDatabase;
        public static IMongoCollection<BsonDocument> msg_collection;

        public static List<BsonDocument> msgsToSend = new List<BsonDocument>();
        public static List<MsgClass> msgsReceived = new List<MsgClass>();
        public static List<MsgClass> displayMsgList = new List<MsgClass>();

        public static void dbSetup()
        {
            dbClient = new MongoClient("mongodb://localhost:27017");
            messageDatabase = dbClient.GetDatabase("local");
            msg_collection = messageDatabase.GetCollection<BsonDocument>("MessagingAppDB");
            if (msg_collection != null)
            {
                checkDB();
                getMsgsFromDB();
            }
        }

        public static bool tryReconnectToDB()
        {
            if (msg_collection == null)
            {
                try
                {
                    dbClient = new MongoClient("mongodb://localhost:27017");
                    messageDatabase = dbClient.GetDatabase("local");
                    msg_collection = messageDatabase.GetCollection<BsonDocument>("MessagingAppDB");
                    checkDB();
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }
            return false;
        }

        public static void msgToBsonList(MsgClass message)
        {
            var msgDocument = new BsonDocument {
                    { "User", message.user },
                    { "Message", message.message },
                    { "Time Sent", new BsonDateTime(message.sentTime) },
                };

            //var allMsgsDocument = msg_collection;
            //Debug.WriteLine(allMsgsDocument);
            //msg_collection.InsertOne(msgDocument);
            msgsToSend.Add(msgDocument);
            Debug.WriteLine(msgsToSend);
        }

        // check if blank BsonDocument is there if not return true, else false
        public static void checkDB()
        {
            var filter = Builders<BsonDocument>.Filter.Exists("MessagesArray");
            var messagesInDB = msg_collection.Find(filter).FirstOrDefault();
            if (messagesInDB == null)
            {
                var messagesDoc = new BsonDocument {
                    { "MessagesArray", new BsonArray { } }
                };
                msg_collection.InsertOne(messagesDoc);
                Debug.WriteLine("I have added the blank Bson Document");
            }
            else
            {
                Debug.WriteLine("The Bson Document is already there");
            }
        }

        public static void sendMsg()
        {
            //check if collection DB is there and if it is we will add to the array
            if (msg_collection != null)
            {
                Debug.WriteLine("Theoretical DB is Connected");

                var filter = Builders<BsonDocument>.Filter.Exists("MessagesArray");
                var messagesDocument = msg_collection.Find(filter).FirstOrDefault();
                Debug.WriteLine(messagesDocument);
                var messagesDict = messagesDocument.ToDictionary();
                var tempBArray = (object[])(messagesDict["MessagesArray"]);
                Debug.WriteLine(tempBArray);
                BsonArray tempB = new BsonArray(tempBArray);
                BsonArray msgArray = new BsonArray();
                //foreach (var msg in msgArray)
                //{
                //    tempBArray.Add((BsonValue)msg);
                //}
                object[] newBArray = addFromListToArray();
                try
                {
                    foreach (var message in tempB)
                    {
                        Debug.WriteLine("From the array! " + message.ToString());
                        msgArray.Add((BsonValue)message);
                    }
                    foreach (var msg in newBArray)
                    {
                        Debug.WriteLine("From the array! " + msg.ToString());
                        Debug.WriteLine(messagesDocument["MessagesArray"]);
                        msgArray.Add((BsonValue)msg);
                    }
                    var messagesDocument2 = msg_collection.Find(filter).ToList();
                    var modelUpdate = Builders<BsonDocument>.Update.Set("MessagesArray", msgArray);
                    msg_collection.UpdateOne(filter, modelUpdate);
                    msgsToSend.Clear();
                    //Debug.WriteLine(messagesDocument);
                }
                catch (Exception e)
                { Debug.WriteLine(e.ToString()); }
            }
            else
            {
                Debug.WriteLine("Theoretical DB is NOT Connected");
            }
        }

        public static BsonDocument[] addFromListToArray()
        {
            BsonDocument[] msgSending = new BsonDocument[0];
            List<BsonDocument> tempMsgList = new List<BsonDocument>();
            foreach (var clientMsg in msgsToSend)
            {
                Debug.WriteLine(clientMsg);
                tempMsgList.Add(clientMsg);
            }
            if (tempMsgList.Count > 0)
            {
                return tempMsgList.ToArray();
            }
            return msgSending;
        }


        /// collection
        /// |
        /// |
        /// ---> document "messages"
        ///             |
        ///             |
        ///             ---> bson array of all messages          

        public static void getMsgsFromDB()
        {
            if (msg_collection != null)
            {
                Debug.WriteLine("Theoretical DB is Connected");

                msgsReceived.Clear();
                var filter = Builders<BsonDocument>.Filter.Exists("MessagesArray");
                //var filter = Builders<BsonDocument>.Filter.Eq("MessagesArray", "Array");
                var sort = Builders<BsonDocument>.Sort.Descending("Time Sent");
                var messagesDocument = msg_collection.Find(filter).Sort(sort).ToList();
                Debug.WriteLine(messagesDocument);
                foreach (var message in messagesDocument)
                {
                    var msgDict = message.ToDictionary();
                    var mobjArray = (Object[])(msgDict["MessagesArray"]);
                    foreach (Object mObj in mobjArray)
                    {
                        var deviceDictionary = (Dictionary<String, Object>)mObj;
                        
                        Debug.WriteLine(deviceDictionary["User"]);
                        string user = deviceDictionary["User"].ToString();
                        string Message = deviceDictionary["Message"].ToString();
                        DateTime time = (DateTime)deviceDictionary["Time Sent"]; // need to get this to work but it receives the Message and User properly

                        MsgClass msg = new MsgClass(user, Message, time);
                        msgsReceived.Add(msg);
                    }
                }
                Debug.WriteLine(msgsReceived);
                displayMsg();
            }
        }

        public static void displayMsg()
        {
            MainPage.msgdisplay.Text = "";
            // displayMessageEditor
            // MainPage
            // messageMainPage

            // MainPage.msgdisplay
            foreach (var msg in msgsReceived)
            {
                string tempMsg = msg.message + " was sent by " + msg.user + " at " + msg.sentTime + ".";
                MainPage.msgdisplay.Text += tempMsg + "\n";
            }
        }
    }
}
