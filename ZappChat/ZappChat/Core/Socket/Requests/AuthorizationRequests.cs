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

        public AuthorizationLoginAndPasswordRequest()
        {
            action = "client/auth";
        }
    }

    class AuthorizationTokenRequest : Request
    {
        public string token { get; set; }

        public AuthorizationTokenRequest()
        {
            action = "client/token";
        }
    }
}
