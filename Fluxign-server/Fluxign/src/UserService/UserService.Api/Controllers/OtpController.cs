using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserService.Application.DTOs;
using UserService.Application.Interfaces.Services;
using UserService.Application.ViewModels;
using UserService.Domain.Entities;

namespace UserService.Api.Controllers
{
    [Route("api/users/[controller]")]
    [ApiController]
    public class OtpController : ControllerBase
    {
        private readonly IOtpService _otpService;

        public OtpController(IOtpService otpService)
        {
            _otpService = otpService;
        }

        [HttpPost("request")]
        public async Task<IActionResult> RequestOtp([FromBody] string purpose)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            return Ok(await _otpService.GenerateAndSendOtpAsync(Guid.Parse(userIdClaim), purpose));
        }

        [HttpPost("validate")]
        public async Task<IActionResult> ValidateOtp([FromBody] OtpValidationRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            return Ok(await _otpService.ValidateOtpAsync(Guid.Parse(userIdClaim), request.OtpCode, request.Purpose));
        }
    }
}
