using Microsoft.Extensions.Configuration;
using SignatureService.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using SignatureService.Application.Common;
using Newtonsoft.Json.Linq;

namespace SignatureService.Application.Services
{
    public class SignatureService: ISignatureService
    {
        private readonly IConfiguration _configuration;
        private readonly ISignatureServiceClient _signatureServiceClient;
        private readonly IRequestServiceClient _requestServiceClient;
        public SignatureService(IConfiguration configuration, ISignatureServiceClient signatureServiceClient, IRequestServiceClient requestServiceClient)
        {
            _configuration = configuration;
            _signatureServiceClient = signatureServiceClient;
            _requestServiceClient = requestServiceClient;
        }

        public async Task<ServiceResult<bool>> VerifyUser(string code, string requestUrl, string token)
        {
            try
            {
                var response = await _signatureServiceClient.GetTokenRequest(requestUrl, code);

                if (response == null)
                {
                    return ServiceResult<bool>.Failure("Token request failed.");
                }

                var json = JObject.Parse(response);
                var accessToken = json["access_token"]?.ToString();

                var loggedUserMobile = await _signatureServiceClient.GetUserInfo(accessToken);
                var user = await _requestServiceClient.GetUserByUserToken(token);
                if(user.RecipientPhone == loggedUserMobile)
                {
                    return ServiceResult<bool>.Success(true, "User Verified Successfully.");
                }
                return ServiceResult<bool>.Failure("User verification failed");
            }
            catch (Exception ex) {
                return ServiceResult<bool>.Failure($"User verification failed - {ex}");
            }
        }
    }
}
