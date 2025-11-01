using DocumentService.Application.Common;
using DocumentService.Application.DTOs;
using DocumentService.Application.Interfaces.Services;
using DocumentService.Infrastructure.External;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DocumentService.Infrastructure.Services
{
    internal class RequestServiceClient : IRequestServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RequestServiceClient(HttpClient httpClient, IOptions<ExternalServiceOptions> options, IHttpContextAccessor httpContextAccessor)
        {
            var config = options.Value;

            httpClient.BaseAddress = new Uri(config.ApiGatewayBaseUrl);

            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<SigningRecipientDto> GetRecipientByToken(string token)
        {
            var response = await _httpClient.GetAsync($"/api/document/requestId?token={token}");

            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<ServiceResult<SigningRecipientDto>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return data.Data;
        }
    }
}
