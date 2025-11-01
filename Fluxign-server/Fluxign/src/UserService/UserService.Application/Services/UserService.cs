using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UserService.Application.Common;
using UserService.Application.DTOs;
using UserService.Application.Interfaces.Repositories;
using UserService.Application.Interfaces.Security;
using UserService.Application.Interfaces.Services;
using UserService.Domain.Entities;

namespace UserService.Application.Services
{
    public class UserService: IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IMailService _mailService;
        private readonly IResetRequestRepository _resetRequestRepository;
        private readonly string _frontendUrl;

        public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher, IUserRoleRepository userRoleRepository, IMailService mailService, IResetRequestRepository resetRequestRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _userRoleRepository = userRoleRepository;
            _mailService = mailService;
            _resetRequestRepository = resetRequestRepository;
            _frontendUrl = configuration["Frontend:BaseUrl"];
        }

        public async Task<ServiceResult<User>> GetUserByIdAsync(Guid id)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user != null)
                {
                    return ServiceResult<User>.Success(user, "User Details fetched successfully.");
                }
                else
                {
                    return ServiceResult<User>.Failure("User Not Found.");
                }
            }
            catch (Exception ex) {
                return ServiceResult<User>.Failure($"{ex}.");
            }
        }

        public async Task<ServiceResult<User>> GetUserByEmailAsync(string email)
        {
            try
            {
                var user = await _userRepository.GetByEmailAsync(email);
                if (user != null)
                {
                    return ServiceResult<User>.Success(user, "User Details fetched successfully.");
                }
                else
                {
                    return ServiceResult<User>.Failure("User Not Found.");
                }
            }
            catch (Exception ex)
            {
                return ServiceResult<User>.Failure($"{ex}.");
            }
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<ServiceResult<Guid>> CreateUserAsync(UserRegisterDto request)
        {
            try
            {
                if (_userRepository.UserEmailExists(request.UserEmail.ToLower()).Result)
                {
                    return ServiceResult<Guid>.Failure("A user with this email already exists.");
                }

                // Check if role is valid
                var role = _userRoleRepository.GetByNameAsync(request.Role);
                if (request.Role == null)
                {
                    return ServiceResult<Guid>.Failure("Invalid Role.");
                }

                var user = new User
                {
                    Id = Guid.NewGuid(),
                    EmiratesId = request.EmiratesId,
                    UserEmail = request.UserEmail,
                    UserPhone = request.UserPhone,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    RoleId = role.Result.Id,
                    Password = _passwordHasher.HashPassword(request.Password),
                    CreatedAt = DateTimeOffset.UtcNow,
                    UpdatedAt = DateTimeOffset.UtcNow
                };

                await _userRepository.AddAsync(user);

                return ServiceResult<Guid>.Success(user.Id, "User Registered successfully.");
            }
            catch (Exception ex) {
                return ServiceResult<Guid>.Failure($"{ex}.");
            }
        }

        public async Task<ServiceResult<Guid>> UpdateUserAsync(UserDto user)
        {
            var userDetails = _userRepository.GetByIdAsync((Guid)user.Id).Result;

            userDetails.FirstName = user.FirstName;
            userDetails.LastName = user.LastName;
            userDetails.EmiratesId = user.EmiratesId;
            userDetails.UserEmail = user.UserEmail;
            userDetails.UserPhone = user.UserPhone;
            userDetails.AvatarImage = user.AvatarImage;

            await _userRepository.UpdateAsync(userDetails);
            return ServiceResult<Guid>.Success(userDetails.Id, "User Updated successfully.");
        }

        public async Task<ServiceResult<bool>> RequestPasswordReset(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null) return ServiceResult<bool>.Failure("User Not Found.");

            var rawToken = Guid.NewGuid().ToString();
            var tokenHash = HashToken(rawToken);

            var resetRequest = new PasswordResetRequest
            {
                UserId = user.Id,
                TokenHash = tokenHash,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(30)
            };

            await _resetRequestRepository.CreateAsync(resetRequest);

            var resetLink = $"{_frontendUrl}/reset-password?token={Uri.EscapeDataString(rawToken)}";
            await _mailService.SendPasswordReset(resetLink, email);
            return ServiceResult<bool>.Success(true, "Password reset link sent to your email.");
        }

        public string HashToken(string token)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
            return Convert.ToBase64String(bytes);
        }

    }
}
