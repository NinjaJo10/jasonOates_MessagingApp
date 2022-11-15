using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Diagnostics;

namespace jasonOates_MessagingApp
{
    public class DatabaseHandler
    {
        public static MongoClient dbClient = null;
        public static IMongoDatabase messageDatabase = null;
        public static IMongoCollection<BsonDocument> msg_collection = null;

        public static List<BsonDocument> msgsToSend = new List<BsonDocument>();
        public static void dbSetup()
        {
            dbClient = new MongoClient("mongodb://localhost:27017");
            messageDatabase = dbClient.GetDatabase("local");
            msg_collection = messageDatabase.GetCollection<BsonDocument>("MessagingAppDB");
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

        public static bool checkDB()
        {
            var filter = Builders<BsonDocument>.Filter.Exists("Messages");
            var messagesInDB = msg_collection.Find(filter).FirstOrDefault();
            Debug.WriteLine(messagesInDB);
            if (messagesInDB == null)
            {
                return true;
            }
            return false;
        }

        public static void sendMsg()
        {
            if (checkDB())
            {
                var messagesDoc = new BsonDocument {
                    { "Messages", "Messages" }
                };
                //foreach (var msg in msgsToSend)
                //{
                //    if (msg["User"] == "Jason")
                //    {
                //        Debug.WriteLine("Hey Jason");
                //    }
                //}
                msg_collection.InsertOne(messagesDoc);
                Debug.WriteLine("I have added a messages BsonDoc");
            }
            else
            {
                Debug.WriteLine("Messages already there");
            }
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
