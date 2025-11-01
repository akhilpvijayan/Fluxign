using DocumentService.Application.Common;
using DocumentService.Application.ViewModels;
using DocumentService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentService.Application.Interfaces.Services
{
    public interface IDocumentService
    {
        Task<ServiceResult<Document>> GetAllDocumentsByRequestId(Guid id);
        Task<ServiceResult<string>> GetOriginalDocumentByRecipientToken(string token);
        Task<ServiceResult<Guid>> CreateDocumentAsync(AddDocument document);
    }
}
