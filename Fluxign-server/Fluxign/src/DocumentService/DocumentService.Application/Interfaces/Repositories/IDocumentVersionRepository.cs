using DocumentService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentService.Application.Interfaces.Repositories
{
    public interface IDocumentVersionRepository
    {
        Task AddAsync(DocumentVersion document);
        Task<DocumentVersion> GetById(Guid Id);
        Task<DocumentVersion?> GetByIdAsync(Guid id);
        Task UpdateAsync(DocumentVersion user);
    }
}
