using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SignatureService.Application.Interfaces.Services;
using System.Runtime;

namespace SignatureService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignatureController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ISignatureService _signatureService;

        public SignatureController(ISignatureService signatureService, IConfiguration configuration)
        {
            _signatureService = signatureService;
            _configuration = configuration;
        }

        [HttpGet("VerifyUser/{token}")]
        public async Task<IActionResult> VerifyUser([FromQuery] string code, [FromQuery] string state, string token)
        {
            if (string.IsNullOrEmpty(code))
                return BadRequest("Code is Empty");

            string baseUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
            string fullUrl = $"{baseUrl}/api/Signature/VerifyUser/{token}";

            var response = await _signatureService.VerifyUser(code, fullUrl, token);

            var frontendRedirectUrl = string.Empty;
            if (response.IsSuccess)
            {
                frontendRedirectUrl = $"{_configuration["FrontEnd:BaseUrl"]}/sign-in/{token}?message={Uri.EscapeDataString("User verified successfully")}";
            }
            else
            {
                frontendRedirectUrl = $"{_configuration["FrontEnd.BaseUrl"]}/verify-user/{token}?message={Uri.EscapeDataString("Not a registered user")}";
            }

            return Redirect(frontendRedirectUrl);
        }
    }
}
