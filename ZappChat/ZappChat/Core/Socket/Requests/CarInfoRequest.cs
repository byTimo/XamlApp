using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZappChat.Core.Socket.Requests
{
    class CarInfoRequest : Request
    {
        public ulong request_id { get; set; }

        public CarInfoRequest()
        {
            action = "request/car";
        }
    }
}
