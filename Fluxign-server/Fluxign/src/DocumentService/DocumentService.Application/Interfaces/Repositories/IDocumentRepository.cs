using DocumentService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentService.Application.Interfaces.Repositories
{
    public interface IDocumentRepository
    {
        Task AddAsync(Document document);
        Task<Document> GetByRequestId(Guid Id);
        Task UpdateAsync(Document user);
    }
}
