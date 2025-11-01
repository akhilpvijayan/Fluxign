using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Application.Interfaces.Repositories;
using UserService.Domain.Entities;
using UserService.Persistence.Data;

namespace UserService.Persistence.Repositories
{
    public class ResetRequestRepository : IResetRequestRepository
    {
        private readonly UserServiceDbContext _dbContext;

        public ResetRequestRepository(UserServiceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CreateAsync(PasswordResetRequest passwordResetRequest)
        {
            await _dbContext.PasswordResetRequest.AddAsync(passwordResetRequest);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(PasswordResetRequest passwordResetRequest)
        {
            _dbContext.PasswordResetRequest.Update(passwordResetRequest);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<PasswordResetRequest> FindAsync(string hashToken)
        {
           return await _dbContext.PasswordResetRequest
            .Include(r => r.User)
            .Where(r => r.TokenHash == hashToken && !r.Used && r.ExpiresAt > DateTime.UtcNow)
            .FirstOrDefaultAsync();
        }
    }
}
