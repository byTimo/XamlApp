﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZappChat.Core.Socket.Requests
{
    class ListQueryRequest : Request
    {
        public int from_id { get; set; }
    }
}