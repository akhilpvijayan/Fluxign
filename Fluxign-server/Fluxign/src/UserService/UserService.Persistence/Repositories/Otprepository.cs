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
    public class Otprepository: IUserOtpRepository
    {
        private readonly UserServiceDbContext _context;

        public Otprepository(UserServiceDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(UserOtp user)
        {
            await _context.UserOtp.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<UserOtp>> GetAllAsync()
        {
            return await _context.UserOtp.ToListAsync();
        }

        public async Task<UserOtp?> GetByIdAsync(Guid id)
        {
            return await _context.UserOtp.FindAsync(id);
        }

        public async Task UpdateAsync(UserOtp user)
        {
            _context.UserOtp.Update(user);
            await _context.SaveChangesAsync();
        }

        public Task<UserOtp?> ValidateOtp(Guid userId, string hasedOtp)
        {
           return _context.UserOtp
            .Where(o => o.UserId == userId && !o.IsUsed && o.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync(o => o.OtpCode == hasedOtp);
        }
    }
}
