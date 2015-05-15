using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZappChat.Core.Socket.Requests
{
    abstract class RequestWithId : Request
    {
        public uint id { get; set; }
    }

    class QueryInfoRequest : RequestWithId
    {
        public QueryInfoRequest()
        {
            action = "request/info";
        }
    }

    class AnswerRequest  : RequestWithId
    {
        public bool selling { get; set; }
        public bool on_stock { get; set; }

        public AnswerRequest()
        {
            action = "request/answer";
        }
    }

    class VinRequest : RequestWithId
    {
        public VinRequest()
        {
            action = "request/need_vin";
        }
    }
}
