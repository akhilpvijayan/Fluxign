using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Application.Common;
using UserService.Application.DTOs;
using UserService.Application.ViewModels;

namespace UserService.Application.Interfaces.Security
{
    public interface IAuthService
    {
        Task<ServiceResult<LoginResponse>> LoginAsync(LoginRequestDto request);
        Task<RefreshTokenResponse?> RefreshTokenAsync(RefreshTokenRequest request);
        Task<bool> LogoutAsync(Guid userId, string? token = null);
        Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordRequest request);
        Task<ServiceResult<bool>> ResetPasswordAsync(string rawToken, string newPassword);
        Task<string> GenerateJwtTokenAsync(UserResponse user);
        //Task<UserResponse?> ValidateTokenAsync(string token);
        Task<ServiceResult<bool>> ConfirmPassword(Guid userId, string password);
    }

}
