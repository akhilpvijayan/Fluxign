using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Application.ViewModels
{
    public class RefreshTokenResponse
    {
        public string Token { get; }
        public string RefreshToken { get; }
        public DateTimeOffset ExpiresAt { get; }

        public RefreshTokenResponse(string token, string refreshToken, DateTimeOffset expiresAt)
        {
            Token = token;
            RefreshToken = refreshToken;
            ExpiresAt = expiresAt;
        }
    }

}
