using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace jasonOates_MessagingApp
{
    public class MsgClass
    {
        public string user;
        public string message;
        public DateTime sentTime;


        public MsgClass(string user, string message, DateTime sentTime)
        {
            this.user = user;
            this.message = message;
            this.sentTime = sentTime;
        }
    }
}
