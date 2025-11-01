using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Application.Common;
using UserService.Application.DTOs;
using UserService.Domain.Entities;

namespace UserService.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<ServiceResult<User>> GetUserByEmailAsync(string email);
        Task<ServiceResult<User>> GetUserByIdAsync(Guid id);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<ServiceResult<Guid>> CreateUserAsync(UserRegisterDto request);
        Task<ServiceResult<Guid>> UpdateUserAsync(UserDto user);
        Task<ServiceResult<bool>> RequestPasswordReset(string email);
        string HashToken(string token);
    }
}
