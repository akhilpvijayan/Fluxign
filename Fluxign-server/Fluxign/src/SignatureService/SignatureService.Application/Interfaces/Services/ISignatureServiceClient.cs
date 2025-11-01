using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignatureService.Application.Interfaces.Services
{
    public interface ISignatureServiceClient
    {
        Task<string?> GetTokenRequest(string requestUrl, string code);
        Task<string> GetUserInfo(string accessToken);
    }
}
