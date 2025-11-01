using DocumentService.Application.Interfaces.Services;
using DocumentService.Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DocumentService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _documentService;

        public DocumentController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        [HttpGet("requestId")]
        public async Task<IActionResult> GetDocumentByRequestId([FromQuery] Guid requestId)
        {
            var user = await _documentService.GetAllDocumentsByRequestId(requestId);
            return Ok(user);
        }

        [HttpGet("original-document/token")]
        public async Task<IActionResult> GetOriginalDocumentByRecipientToken([FromQuery]string token)
        {
            var user = await _documentService.GetOriginalDocumentByRecipientToken(token);
            return Ok(user);
        }

        //[Authorize]
        //[HttpGet]
        //[Route("user-documents")]
        //public async Task<IActionResult> GetAllDocumentsByUserId()
        //{
        //    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        //    if (string.IsNullOrEmpty(userIdClaim))
        //        return Unauthorized();

        //    var userId = Guid.Parse(userIdClaim);
        //    var data = _documentService.GetAllDocumentsByRequestId(userId);
        //    return Ok();
        //}

        [HttpPost]
        [Route("add-document")]
        public async Task<IActionResult> AddDocument([FromBody] AddDocument document)
        {
            var data = await _documentService.CreateDocumentAsync(document);
            if (!data.IsSuccess)
                return StatusCode(500, data.Message);

            return Ok(data);
        }
    }
}
