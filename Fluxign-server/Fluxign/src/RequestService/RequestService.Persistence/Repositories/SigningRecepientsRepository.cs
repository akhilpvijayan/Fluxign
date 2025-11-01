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
    public class SigningRecepientsRepository: ISigningRecepientsRepository
    {
        private readonly RequestServiceDbContext _context;

        public SigningRecepientsRepository(RequestServiceDbContext context)
        {
            _context = context;
        }

        public async Task<List<SigningRecipient>> GetRecepientsById(Guid id)
        {
           return await _context.SigningRecipients
                .Where(r => r.RequestId == id).OrderBy(x=>x.SigningOrder)
                .ToListAsync();
        }

        public async Task<SigningRecipient> GetRecepientByRecepientId(Guid id)
        {
            return await _context.SigningRecipients
                 .Where(r => r.Id == id).FirstOrDefaultAsync();
        }

        public async Task<SigningRecipient> GetRecipientByTokenAsync(string token)
        {
            return await _context.SigningRecipients.Where(x => x.SigningToken == token && x.IsActive == true).FirstOrDefaultAsync();
        }

        public async Task AddAsync(SigningRecipient recepients)
        {
            await _context.SigningRecipients.AddAsync(recepients);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteByRequestIdAsync(Guid id)
        {
            await _context.SigningRecipients
                .Where(r => r.RequestId == id)
                .ExecuteDeleteAsync();
        }

        public async Task UpdateAsync(SigningRecipient recepients)
        {
            _context.SigningRecipients.Update(recepients);
            await _context.SaveChangesAsync();
        }
    }
}
