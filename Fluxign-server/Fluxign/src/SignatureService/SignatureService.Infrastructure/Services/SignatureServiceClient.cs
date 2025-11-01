using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using SignatureService.Application.Common;
using SignatureService.Application.Interfaces.Services;
using SignatureService.Infrastructure.External;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SignatureService.Infrastructure.Services
{
    public class SignatureServiceClient : ISignatureServiceClient
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _configuration;
        public SignatureServiceClient(HttpClient http, IOptions<ExternalServiceOptions> options, IConfiguration configuration)
        {
            _http = http;
            var config = options.Value;
            _http.BaseAddress = new Uri(config.ApiGatewayBaseUrl);
            _configuration = configuration;
        }

        public async Task<string?> GetTokenRequest(string requestUrl, string code)
        {
            var url = $"{_configuration["BaseUrl"]}/idshub/token";
            var request = new HttpRequestMessage(HttpMethod.Post, url);

            var authToken = Encoding.ASCII.GetBytes($"{_configuration["GetSigningAccessTokenAPI:Username"]}:{_configuration["GetSigningAccessTokenAPI:Password"]}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));

            var content = new Dictionary<string, string>
            {
                ["grant_type"] = "authorization_code",
                ["redirect_uri"] = requestUrl,
                ["code"] = code
            };

            request.Content = new FormUrlEncodedContent(content);
            var response = await _http.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetUserInfo(string accessToken)
        {
            string url = string.Concat(_configuration["BaseUrl"], "/idshub/userinfo");
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _http.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var jsonString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            var json = JObject.Parse(jsonString);
            var userMobile = json["mobile"]?.ToString();

            return userMobile;
        }
    }
}
