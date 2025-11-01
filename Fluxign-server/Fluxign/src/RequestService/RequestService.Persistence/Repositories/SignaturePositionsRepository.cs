using Microsoft.EntityFrameworkCore;
using RequestService.Application.Interfaces.Repositories;
using RequestService.Domain.Entities;
using RequestService.Persistence.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequestService.Persistence.Repositories
{
    public class SignaturePositionsRepository: ISignaturePositionsRepository
    {
        private readonly RequestServiceDbContext _context;

        public SignaturePositionsRepository(RequestServiceDbContext context)
        {
            _context = context;
        }

        public async Task<SignaturePosition> GetPositionsByRecepientId(Guid id, Guid requestId)
        {
            return await _context.SignaturePositions
                .Where(r => r.RecipientId == id && r.RequestId == requestId)
                .FirstOrDefaultAsync();
        }

        public async Task AddAsync(SignaturePosition positions)
        {
            await _context.SignaturePositions.AddAsync(positions);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteByRequestIdAsync(Guid id)
        {
            await _context.SignaturePositions
                .Where(r => r.RequestId == id)
                .ExecuteDeleteAsync();
        }

        public async Task UpdateAsync(SignaturePosition positions)
        {
            _context.SignaturePositions.Update(positions);
            await _context.SaveChangesAsync();
        }
    }
}
