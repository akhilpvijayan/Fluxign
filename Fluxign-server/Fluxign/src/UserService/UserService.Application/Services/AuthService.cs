using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using UserService.Application.Common;
using UserService.Application.DTOs;
using UserService.Application.Interfaces.Repositories;
using UserService.Application.Interfaces.Security;
using UserService.Application.Interfaces.Services;
using UserService.Application.ViewModels;
using UserService.Domain.Entities;

namespace UserService.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserRoleRepository _roleRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUserService _userService;
    private readonly IResetRequestRepository _resetRequestRepository;

    public AuthService(
        IUserRepository userRepository,
        IUserRoleRepository roleRepository,
        IPasswordHasher passwordHasher,
        IConfiguration configuration,
        ILogger<AuthService> logger,
        IRefreshTokenRepository refreshTokenRepository,
        IUserService userService,
        IResetRequestRepository resetRequestRepository)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
        _logger = logger;
        _refreshTokenRepository = refreshTokenRepository;
        _userService = userService;
        _resetRequestRepository = resetRequestRepository;
    }

    public async Task<ServiceResult<LoginResponse>> LoginAsync(LoginRequestDto request)
    {
        try
        {
            _logger.LogInformation("Login attempt for email: {Email}", request.Email);

            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null || !user.IsActive || user.IsDelete)
            {
                _logger.LogWarning("Login failed: Invalid user status for email: {Email}", request.Email);
                return ServiceResult<LoginResponse>.Failure("User Not Exists");
            }

            if (!_passwordHasher.VerifyPassword(user.Password, request.Password))
            {
                _logger.LogWarning("Login failed: Invalid password for email: {Email}", request.Email);
                return ServiceResult<LoginResponse>.Failure("Invalid Password");
            }

            user.LastLogin = DateTimeOffset.UtcNow;
            user.UpdatedAt = DateTimeOffset.UtcNow;
            await _userRepository.UpdateAsync(user);

            var userResponse = MapToUserResponse(user);
            var token = await GenerateJwtTokenAsync(userResponse);
            var refreshToken = GenerateRefreshToken(user.Id);
            await _refreshTokenRepository.CreateAsync(refreshToken);
            int expiryMinutes = _configuration.GetValue<int?>("JwtSettings:ExpiryMinutes") ?? 60;

            var expiresAt = DateTimeOffset.UtcNow.AddMinutes(expiryMinutes);


            await CreateUserSessionAsync(user.Id, token, expiresAt, GetClientIpAddress());

            return ServiceResult<LoginResponse>.Success(new LoginResponse(token, userResponse, expiresAt)
            {
                RefreshToken = refreshToken.Token
            }, "Login Success.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for email: {Email}", request.Email);
            return ServiceResult<LoginResponse>.Failure("An error occurred during login.");
        }
    }

    public async Task<RefreshTokenResponse?> RefreshTokenAsync(RefreshTokenRequest request)
    {
        try
        {
            var principal = GetPrincipalFromExpiredToken(request.Token);
            if (principal == null) return null;

            var jwtToken = new JwtSecurityToken(request.Token);
            foreach (var claim in principal.Claims)
            {
                Console.WriteLine($"Claim type: {claim.Type}, value: {claim.Value}");
            }
            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId)) return null;

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || !user.IsActive) return null;

            if (!await ValidateRefreshTokenAsync(userId, request.RefreshToken)) return null;

            var userResponse = MapToUserResponse(user);
            var newToken = await GenerateJwtTokenAsync(userResponse);
            var newRefreshToken = GenerateRefreshToken(user.Id);
            await _refreshTokenRepository.CreateAsync(newRefreshToken);
            var expiresAt = DateTimeOffset.UtcNow.AddMinutes(
                _configuration.GetValue<int>("JwtSettings:ExpiryMinutes", 60));

            await CreateUserSessionAsync(user.Id, newToken, expiresAt, GetClientIpAddress());

            return new RefreshTokenResponse(newToken, newRefreshToken.Token, expiresAt);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");
            return null;
        }
    }

    public async Task<bool> LogoutAsync(Guid userId, string? token = null)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user != null)
            {
                user.UpdatedAt = DateTimeOffset.UtcNow;
                await _userRepository.UpdateAsync(user);

                var tokens = await _refreshTokenRepository.GetActiveTokensByUserIdAsync(userId);
                foreach (var item in tokens)
                {
                    item.IsRevoked = true;
                    await _refreshTokenRepository.UpdateAsync(item);
                }

            }

            _logger.LogInformation("User logged out: {UserId}", userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout for user: {UserId}", userId);
            return false;
        }
    }

    public async Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordRequest request)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return false;

        if (!_passwordHasher.VerifyPassword(request.CurrentPassword, user.Password)) return false;

        user.Password = _passwordHasher.HashPassword(request.NewPassword);
        user.UpdatedAt = DateTimeOffset.UtcNow;
        await _userRepository.UpdateAsync(user);

        _logger.LogInformation("Password changed for user: {UserId}", userId);
        return true;
    }

    public async Task<ServiceResult<bool>> ResetPasswordAsync(string rawToken, string newPassword)
    {
        var tokenHash = _userService.HashToken(rawToken);

        var resetRequest = await _resetRequestRepository.FindAsync(tokenHash);

        if (resetRequest == null)
            return ServiceResult<bool>.Failure("Invalid or expired token.");

        var user = await _userRepository.GetById(resetRequest.UserId);
        if (user == null)
            return ServiceResult<bool>.Failure("User not found.");

        user.Password = _passwordHasher.HashPassword(newPassword);
        user.UpdatedAt = DateTimeOffset.UtcNow;

        resetRequest.Used = true;

        await _resetRequestRepository.UpdateAsync(resetRequest);
        await _userRepository.UpdateAsync(user);

        return ServiceResult<bool>.Success(true, "Password successfully reset.");
    }


    public async Task<string> GenerateJwtTokenAsync(UserResponse user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.GivenName, user.FirstName ?? ""),
            new(JwtRegisteredClaimNames.FamilyName, user.LastName ?? ""),
            new(ClaimTypes.Role, user.Role),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new("emirates_id", user.EmiratesId),
            new("phone", user.Phone ?? ""),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        var token = new JwtSecurityToken(
            //issuer: jwtSettings["Issuer"],
            //audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(jwtSettings.GetValue<int>("ExpiryMinutes", 60)),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    //public async Task<UserResponse?> ValidateTokenAsync(string token)
    //{
    //    try
    //    {
    //        var tokenHandler = new JwtSecurityTokenHandler();
    //        var jwtSettings = _configuration.GetSection("JwtSettings");
    //        var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

    //        var validationParameters = new TokenValidationParameters
    //        {
    //            ValidateIssuer = true,
    //            ValidateAudience = true,
    //            ValidateLifetime = true,
    //            ValidateIssuerSigningKey = true,
    //            ValidIssuer = jwtSettings["Issuer"],
    //            ValidAudience = jwtSettings["Audience"],
    //            IssuerSigningKey = new SymmetricSecurityKey(key),
    //            ClockSkew = TimeSpan.Zero
    //        };

    //        var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
    //        var userIdClaim = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

    //        if (Guid.TryParse(userIdClaim, out var userId))
    //        {
    //            var user = await _userRepository.GetByIdAsync(userId);
    //            return user != null ? MapToUserResponse(user) : null;
    //        }

    //        return null;
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogWarning(ex, "Token validation failed");
    //        return null;
    //    }
    //}

    #region Private Helpers

    private static UserResponse MapToUserResponse(User user)
    {
        return new UserResponse(
            user.Id,
            user.EmiratesId,
            user.UserEmail,
            user.UserPhone,
            user.FirstName,
            user.LastName,
            user.Role?.Role ?? "Unknown",
            user.IsActive,
            user.IsEmailVerified,
            user.CreatedAt
        );
    }

    private RefreshToken GenerateRefreshToken(Guid userId)
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        var token = Convert.ToBase64String(randomNumber);

        return new RefreshToken
        {
            UserId = userId,
            Token = token,
            Expires = DateTime.UtcNow.AddDays(7),
            Created = DateTime.UtcNow,
            IsRevoked = false
        };
    }


    private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false,
            ClockSkew = TimeSpan.Zero
        };

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
        }
        catch
        {
            return null;
        }
    }

    private async Task<bool> ValidateRefreshTokenAsync(Guid userId, string refreshToken)
    {
        var storedToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken);

        if (storedToken == null || storedToken.UserId != userId || storedToken.IsRevoked || storedToken.Expires < DateTime.UtcNow)
            return false;

        return true;
    }

    private async Task<Guid?> ValidatePasswordResetTokenAsync(string token)
    {
        // TODO: Implement actual reset token validation
        return await Task.FromResult((Guid?)null);
    }

    private async Task CreateUserSessionAsync(Guid userId, string token, DateTimeOffset expiresAt, string? ipAddress)
    {
        var session = new UserSession
        {
            UserId = userId,
            SessionToken = GetTokenFingerprint(token),
            ExpiresAt = expiresAt,
            IpAddress = ipAddress,
            CreatedAt = DateTimeOffset.UtcNow
        };

        // TODO: Save session to DB
        _logger.LogInformation("Session created for user: {UserId}", userId);
    }

    private string GetTokenFingerprint(string token)
    {
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(hash)[..16];
    }

    private string? GetClientIpAddress()
    {
        return "127.0.0.1"; // Placeholder
    }

    public async Task<ServiceResult<bool>> ConfirmPassword(Guid userId, string password)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null || !user.IsActive || user.IsDelete)
        {
            return ServiceResult<bool>.Failure("User Not Exists");
        }

        if (!_passwordHasher.VerifyPassword(user.Password, password))
        {
            return ServiceResult<bool>.Failure("Invalid Password");
        }
        return ServiceResult<bool>.Success(true, "Password Verified Successfully");
    }

    #endregion
}
