using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using RequestService.Application.Common;
using RequestService.Application.DTOs;
using RequestService.Application.Interfaces.Services;
using RequestService.Infrastructure.External;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RequestService.Infrastructure.Services
{
    public class UserServiceClient : IUserServiceClient
    {
        private readonly HttpClient _http;
        private readonly IMemoryCache _cache;


        public UserServiceClient(HttpClient http, IOptions<ExternalServiceOptions> options, IMemoryCache cache)
        {
            _http = http;
            var config = options.Value;
            _http.BaseAddress = new Uri(config.ApiGatewayBaseUrl);
            _cache = cache;
        }

        public async Task<UserDto?> GetUserByEmailAsync(string email)
        {
            var response = await _http.GetAsync($"/api/users/email?email={WebUtility.UrlEncode(email)}");

            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            var user = JsonSerializer.Deserialize<ServiceResult<UserDto>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return user.Data;
        }
    }
}
