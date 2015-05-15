using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit;
using NUnit.Framework;

namespace ZappChat.Core.Socket.Requests
{
    class HistoryRequest :Request
    {
        public string from { get; set; }
        public string to { get; set; }

        public HistoryRequest()
        {
            action = "chat/history";
        }
    }
}
