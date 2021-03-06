﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZappChat.Core.Socket.Requests
{
    abstract class RequestWithRoomId : Request
    {
        public long room_id { get; set; }
    }

    class ChatInfoRequest : RequestWithRoomId
    {
        public ChatInfoRequest()
        {
            
        }
    }

    class TypingRequest : RequestWithRoomId
    {
        public string type { get; private set; }

        public TypingRequest()
        {
            action = "chat/message";
            type = "typing";
        }
    }

    class ReadRoomRequest : RequestWithRoomId
    {
        public string type { get; private set; }

        public ReadRoomRequest()
        {
            action = "chat/message";
            type = "read";
        }
    }

    class SendMessageRequest : RequestWithRoomId
    {
        public string type { get; private set; }
        public string hash { get; set; }
        public string message { get; set; }
        public bool system { get; set; }

        public SendMessageRequest()
        {
            action = "chat/message";
            type = "send";
        }
    }
}
