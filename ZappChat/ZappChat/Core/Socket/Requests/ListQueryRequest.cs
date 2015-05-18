using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZappChat.Core.Socket.Requests
{
    class ListQueryRequest : Request
    {
        public ulong from_id { get; set; }

        public ListQueryRequest()
        {
            action = "request/list";
        }
    }
}
