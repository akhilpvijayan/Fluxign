using DocumentService.Application.Common;
using DocumentService.Application.Interfaces.Repositories;
using DocumentService.Application.Interfaces.Services;
using DocumentService.Application.ViewModels;
using DocumentService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentService.Application.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IRequestServiceClient _requestServiceClient;
        public DocumentService(IDocumentRepository documentRepository, IRequestServiceClient requestServiceClient) { 
            _documentRepository = documentRepository;
            _requestServiceClient = requestServiceClient;
        }

        public async Task<ServiceResult<Document>> GetAllDocumentsByRequestId(Guid id)
        {
            var data = await _documentRepository.GetByRequestId(id);
            return ServiceResult<Document>.Success(data, "Document added successfully.");
        }

        public async Task<ServiceResult<Guid>> CreateDocumentAsync(AddDocument documentData)
        {
            try
            {
                var document = new Document
                {
                    Id = Guid.NewGuid(),
                    OriginalFile = documentData.PdfBase64,
                    OriginalFileName = documentData.FileName,
                    OriginalFileSize = documentData.FileSize,
                    UserId = documentData.UserId,
                    RequestId = documentData.RequestId,
                    CreatedAt = DateTimeOffset.UtcNow,
                    UpdatedAt = DateTimeOffset.UtcNow
                };

                await _documentRepository.AddAsync(document);

                return ServiceResult<Guid>.Success(document.Id, "Document added successfully.");
            }
            catch (Exception ex)
            {
                return ServiceResult<Guid>.Failure($"{ex}.");
            }
        }

        public async Task<ServiceResult<string>> GetOriginalDocumentByRecipientToken(string token)
        {
            try
            {
                var recipient = await _requestServiceClient.GetRecipientByToken(token);
                var document = await _documentRepository.GetByRequestId(recipient.RequestId);
                return ServiceResult<string>.Success(document.OriginalFile, "Document fetched successfully.");
            }
            catch (Exception ex)
            {
                return ServiceResult<string>.Failure($"{ex}.");
            }
        }
    }
}
