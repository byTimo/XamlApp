using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZappChat.Core.Socket.Requests
{
    class AuthorizationLoginAndPasswordRequest : Request
    {
        public string email { get; set; }
        public string password { get; set; }
    }

    class AuthorizationTokenRequest : Request
    {
        public string token { get; set; }
    }
}
