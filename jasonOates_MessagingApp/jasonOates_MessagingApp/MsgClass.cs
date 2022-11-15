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
        public string sentTime;

        public MsgClass(string user, string message)
        {
            this.user = user;
            this.message = message;
            this.sentTime = DateTime.Now.ToString("HH:mm dd/MM/yyyy");
        }
    }
}
