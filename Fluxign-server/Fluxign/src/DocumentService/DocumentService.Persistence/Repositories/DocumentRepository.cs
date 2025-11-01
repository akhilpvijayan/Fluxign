using DocumentService.Application.Interfaces.Repositories;
using DocumentService.Domain.Entities;
using DocumentService.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentService.Persistence.Repositories
{
    public class DocumentRepository: IDocumentRepository
    {
        private readonly DocumentServiceDbContext _context;

        public DocumentRepository(DocumentServiceDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Document document)
        {
            await _context.Documents.AddAsync(document);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Document>> GetAllAsync()
        {
            return await _context.Documents.ToListAsync();
        }

        public async Task<Document?> GetByIdAsync(Guid id)
        {
            return await _context.Documents.FindAsync(id);
        }

        public async Task UpdateAsync(Document user)
        {
            _context.Documents.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Document>> GetByUserId(Guid Id)
        {
            return await _context.Documents.Where(x => x.UserId == Id).ToListAsync();

        }

        public async Task<Document> GetByRequestId(Guid Id)
        {
            return await _context.Documents.Where(x => x.RequestId == Id).FirstOrDefaultAsync();

        }
    }
}
