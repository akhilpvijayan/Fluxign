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
    public class DocumentVersionRepository: IDocumentVersionRepository
    {
        private readonly DocumentServiceDbContext _context;

        public DocumentVersionRepository(DocumentServiceDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(DocumentVersion document)
        {
            await _context.DocumentVersions.AddAsync(document);
            await _context.SaveChangesAsync();
        }

        public async Task<DocumentVersion> GetById(Guid Id)
        {
            return await _context.DocumentVersions.Where(x => x.Id == Id).FirstOrDefaultAsync();

        }
        public async Task<IEnumerable<DocumentVersion>> GetAllAsync()
        {
            return await _context.DocumentVersions.ToListAsync();
        }

        public async Task<DocumentVersion?> GetByIdAsync(Guid id)
        {
            return await _context.DocumentVersions.FindAsync(id);
        }

        public async Task UpdateAsync(DocumentVersion user)
        {
            _context.DocumentVersions.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
