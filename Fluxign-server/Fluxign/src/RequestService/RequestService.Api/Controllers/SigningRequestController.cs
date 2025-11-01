using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RequestService.Application.DTOs;
using RequestService.Application.Interfaces.Services;
using System.Security.Claims;

namespace RequestService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SigningRequestController : ControllerBase
    {
        private readonly ISigningRequestService _signingRequestservice;

        public SigningRequestController(ISigningRequestService signingRequestservice)
        {
            _signingRequestservice = signingRequestservice;
        }

        [Authorize]
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard([FromQuery] DashboardQueryParameterDto pagingparams)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            var dashboard = await _signingRequestservice.GetDashboardByUserIdAsync(Guid.Parse(userIdClaim), pagingparams);
            return Ok(dashboard);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            return Ok(await _signingRequestservice.GetAllRequestsByUserIdAsync(Guid.Parse(userIdClaim)));
        }

        [HttpGet("token")]
        public async Task<IActionResult> GetRecipientByToken([FromQuery]string token)
        {
            return Ok(await _signingRequestservice.GetRecipientByToken(token));
        }

        [Authorize]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            var request = await _signingRequestservice.GetRequestByIdAsync(Guid.Parse(userIdClaim), id);

            if (request == null)
                return NotFound();

            return Ok(request);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateSigningRequestAsync(SigningRequestDto request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            request.UserId = Guid.Parse(userIdClaim);

            var result = await _signingRequestservice.CreateSigningRequestAsync(request);

            return Ok(result);
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdateSigningRequestAsync(SigningRequestDto request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            request.UserId = Guid.Parse(userIdClaim);

            var result = await _signingRequestservice.UpdateSigningRequestAsync(request);

            return Ok(result);
        }
    }
}
