using RequestService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequestService.Application.Interfaces.Services
{
    public interface IUserServiceClient
    {
        Task<UserDto?> GetUserByEmailAsync(string email);
    }
}
