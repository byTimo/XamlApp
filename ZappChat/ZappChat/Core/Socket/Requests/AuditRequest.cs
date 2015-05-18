using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZappChat.Core.Socket.Requests
{
    class AuditRequest : Request
    {
        public ulong log_id { get; set; }

        public AuditRequest()
        {
            action = "client/audit";
        }
    }
}
