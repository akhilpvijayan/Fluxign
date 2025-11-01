using Microsoft.Extensions.Options;
using SignatureService.Application.Common;
using SignatureService.Application.DTOs;
using SignatureService.Application.Interfaces.Services;
using SignatureService.Infrastructure.External;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SignatureService.Infrastructure.Services
{
    public class RequestServiceClient: IRequestServiceClient
    {
        private readonly HttpClient _http;

        public RequestServiceClient(HttpClient http, IOptions<ExternalServiceOptions> options)
        {
            _http = http;
            var config = options.Value;
            _http.BaseAddress = new Uri(config.ApiGatewayBaseUrl);
        }

        public async Task<SigningRecipientDto?> GetUserByUserToken(string token)
        {
            var response = await _http.GetAsync($"/api/signingrequest/token?token={WebUtility.UrlEncode(token)}");

            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            var user = JsonSerializer.Deserialize<ServiceResult<SigningRecipientDto>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return user.Data;
        }
    }
}
