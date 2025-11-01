using RequestService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequestService.Application.Interfaces.Services
{
    public interface IDocumentServiceClient
    {
        Task<DocumentDto> GetDocumentByRequestId(Guid requestId);
        Task<bool> CreateDocumentAsync(SigningRequestDto request);
    }
}
