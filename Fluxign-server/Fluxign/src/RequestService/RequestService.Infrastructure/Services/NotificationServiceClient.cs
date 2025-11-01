using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using RequestService.Application.DTOs;
using RequestService.Application.Interfaces.Services;
using RequestService.Infrastructure.External;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace RequestService.Infrastructure.Services
{
    public class NotificationServiceClient : INotificationServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public NotificationServiceClient(HttpClient httpClient, IOptions<ExternalServiceOptions> options, IHttpContextAccessor httpContextAccessor)
        {
            var config = options.Value;

            httpClient.BaseAddress = new Uri(config.ApiGatewayBaseUrl);

            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> CreateNotification(NotificationRequestDto request)
        {
            var userToken = GetUserToken();

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, "notification");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userToken);
            httpRequest.Content = JsonContent.Create(request);

            var response = await _httpClient.SendAsync(httpRequest);

            return response.IsSuccessStatusCode;
        }


        private string GetUserToken()
        {
            var authHeader = _httpContextAccessor.HttpContext?
                .Request.Headers["Authorization"].FirstOrDefault();
            return authHeader?.Replace("Bearer ", "");
        }
    }
}
