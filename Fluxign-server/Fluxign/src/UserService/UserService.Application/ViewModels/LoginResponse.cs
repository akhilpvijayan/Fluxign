using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Application.ViewModels
{
    public class LoginResponse
    {
        public string Token { get; }
        public UserResponse User { get; }
        public DateTimeOffset ExpiresAt { get; }
        public string RefreshToken { get; set; }

        public LoginResponse(string token, UserResponse user, DateTimeOffset expiresAt)
        {
            Token = token;
            User = user;
            ExpiresAt = expiresAt;
        }
    }

}
