using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using UserService.Application.DTOs;
using UserService.Application.Interfaces.Security;
using UserService.Application.ViewModels;
using UserService.Domain.Entities;

namespace UserService.API.Controllers
{
    [ApiController]
    [Route("api/users/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var result = await _authService.LoginAsync(request);

            if (result == null)
                return Unauthorized(new { Message = "Invalid credentials or user inactive." });

            return Ok(result);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userIdClaim = User.FindFirst("sub")?.Value;

            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var result = await _authService.LogoutAsync(userId);

            if (!result)
                return StatusCode(500, new { Message = "Logout failed" });

            return Ok(new { Message = "Logout successful" });
        }

        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
        {
            var result = await _authService.ResetPasswordAsync(model.Token, model.NewPassword);
            return Ok(result);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var response = await _authService.RefreshTokenAsync(request);

            if (response == null)
                return Unauthorized(new { Message = "Invalid token or refresh token" });

            return Ok(response);
        }

        [HttpGet("confirmpassword/{password}")]
        public async Task<IActionResult> ConfirmPassword(string password)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            var result = await _authService.ConfirmPassword(Guid.Parse(userIdClaim), password);

            if (result == null)
                return Unauthorized(new { Message = "Invalid credentials or user inactive." });

            return Ok(result);
        }
    }
}
