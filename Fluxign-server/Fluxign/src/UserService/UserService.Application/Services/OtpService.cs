using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using UserService.Application.Common;
using UserService.Application.Interfaces.Repositories;
using UserService.Application.Interfaces.Services;
using UserService.Application.ViewModels;
using UserService.Domain.Enums;
using static System.Net.WebRequestMethods;

public class OtpService : IOtpService
{
    private readonly IUserOtpRepository _otpRepository;
    private readonly IMailService _mailService;
    private readonly IUserRepository _userRepository;
    private readonly byte[] _hashKey;

    public OtpService(IConfiguration configuration, IUserOtpRepository otpRepository, IMailService mailService, IUserRepository userRepository)
    {
        _otpRepository = otpRepository;
        _mailService = mailService;
        _hashKey = Encoding.UTF8.GetBytes(configuration["OtpSettings:HashKey"]);
        _userRepository = userRepository;
    }

    public async Task<ServiceResult<Guid>> GenerateAndSendOtpAsync(Guid userId, string purpose)
    {
        try
        {
            var otpCode = GenerateRandomOtp(6);
            var now = DateTime.UtcNow;
            var expiresAt = now.AddMinutes(5);

            var hashedOtp = HashOtp(otpCode);

            var userOtp = new UserOtp
            {
                UserId = userId,
                OtpCode = hashedOtp,
                CreatedAt = now,
                ExpiresAt = expiresAt,
                IsUsed = false,
                Purpose = purpose
            };

            var user = await _userRepository.GetById(userId);

            await _otpRepository.AddAsync(userOtp);

            if (OtpPurposeMapper.TryGetEnum(purpose, out var otpPurpose))
            {
                string otpPurposeEnum = GetDisplayName(otpPurpose);
                await _mailService.SendOtp(otpCode, otpPurposeEnum, user.UserEmail, user.FirstName + user.LastName);
                return ServiceResult<Guid>.Success(userId, "OTP Sent successfully.");
            }
            else
            {
                return ServiceResult<Guid>.Failure("Invalid Otp purpose Key.");
            }
        }
        catch (Exception ex) {
            return ServiceResult<Guid>.Failure($"OTP sending failed. {ex}");
        }
    }

    public async Task<ServiceResult<Guid>> ValidateOtpAsync(Guid userId, string otpCode, string purpose)
    {
        var hashedInput = HashOtp(otpCode);

        var otp = await _otpRepository.ValidateOtp(userId, hashedInput);
        if (otp == null) return ServiceResult<Guid>.Failure("Invalid OTP.");

        otp.IsUsed = true;
        await _otpRepository.UpdateAsync(otp);

        OtpPurposeMapper.TryGetEnum(purpose, out var otpPurpose);
        if (otpPurpose == OtpPurposeEnum.EmailVerification)
        {
            var user = await _userRepository.GetById(userId);
            user.IsEmailVerified = true;
            await _userRepository.UpdateAsync(user);

            return ServiceResult<Guid>.Success(otp.UserId, "Email Verified Successfully.");
        }
        return ServiceResult<Guid>.Success(otp.UserId, "OTP Verification successful.");
    }

    private string HashOtp(string otp)
    {
        using var hmac = new HMACSHA256(_hashKey);
        var otpBytes = Encoding.UTF8.GetBytes(otp);
        var hashBytes = hmac.ComputeHash(otpBytes);
        return Convert.ToBase64String(hashBytes);
    }

    private string GenerateRandomOtp(int length)
    {
        var random = new Random();
        return string.Concat(Enumerable.Range(0, length).Select(_ => random.Next(0, 10).ToString()));
    }

    public static class OtpPurposeMapper
    {
        private static readonly Dictionary<string, OtpPurposeEnum> _map = new(StringComparer.OrdinalIgnoreCase)
    {
        { "L", OtpPurposeEnum.Login },
        { "RP", OtpPurposeEnum.ResetPassword },
        { "EV", OtpPurposeEnum.EmailVerification }
    };

        public static bool TryGetEnum(string key, out OtpPurposeEnum value)
            => _map.TryGetValue(key, out value);
    }


    public static string GetDisplayName(OtpPurposeEnum enumValue)
    {
        var displayAttribute = enumValue.GetType()
            .GetField(enumValue.ToString())
            ?.GetCustomAttribute<DisplayAttribute>();

        return displayAttribute?.Name ?? enumValue.ToString();
    }
}
