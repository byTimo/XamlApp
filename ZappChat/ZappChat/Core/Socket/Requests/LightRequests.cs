using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZappChat.Core.Socket.Requests
{
    class PingRequest : Request
    {
        public PingRequest()
        {
            action = "client/ping";
        }
    }

    class SystemChatsRequest : Request
    {
        public SystemChatsRequest()
        {
            action = "chat/systems";
        }
    }

}
