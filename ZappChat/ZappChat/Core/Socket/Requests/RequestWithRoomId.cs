using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZappChat.Core.Socket.Requests
{
    abstract class RequestWithRoomId : Request
    {
        public uint room_id { get; set; }
    }

    class ChatInfoRequest : RequestWithRoomId
    {
    }

    class TypingRequest : RequestWithRoomId
    {
        public string type { get; set; }
    }

    class SendMessageRequest : RequestWithRoomId
    {
        public string type { get; set; }
        public string hash { get; set; }
        public string text { get; set; }
        public bool system { get; set; }
    }
}
