﻿using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Collections.ObjectModel;
using System.Linq;

namespace jasonOates_MessagingApp
{
    public class DatabaseHandler
    {
        public static MongoClient dbClient;
        public static IMongoDatabase messageDatabase;
        public static IMongoCollection<BsonDocument> msg_collection;

        public static List<BsonDocument> msgsToSend = new List<BsonDocument>();
        public static void dbSetup()
        {
            dbClient = new MongoClient("mongodb://localhost:27017");
            messageDatabase = dbClient.GetDatabase("local");
            msg_collection = messageDatabase.GetCollection<BsonDocument>("MessagingAppDB");
            if (msg_collection != null)
            {
                if (checkDB())
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
        }

        public static void msgToBson(MsgClass message)
        {
            var msgDocument = new BsonDocument {
                    { "User", message.user },
                    { "Message", message.message },
                    { "Time Sent", message.sentTime },
                };

            //var allMsgsDocument = msg_collection;
            //Debug.WriteLine(allMsgsDocument);
            //msg_collection.InsertOne(msgDocument);
            msgsToSend.Add(msgDocument);
            Debug.WriteLine(msgsToSend);
        }

        // check if blank BsonDocument is there if not return true, else false
        public static bool checkDB()
        {
            var filter = Builders<BsonDocument>.Filter.Exists("MessagesArray");
            var messagesInDB = msg_collection.Find(filter).FirstOrDefault();
            if (messagesInDB == null)
            {
                return true;
            }
            return false;
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
                var messsagesDict = messagesDocument.ToDictionary();
                var tempBArray = (object[])(messsagesDict["MessagesArray"]);
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
    }
}
