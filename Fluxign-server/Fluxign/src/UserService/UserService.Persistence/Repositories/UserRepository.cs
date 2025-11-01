using Microsoft.EntityFrameworkCore;
using UserService.Application.Interfaces.Repositories;
using UserService.Domain.Entities;
using UserService.Persistence.Data;

namespace UserService.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserServiceDbContext _context;

        public UserRepository(UserServiceDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User> GetById(Guid Id)
        {
            return await _context.Users.Where(x=>x.Id == Id).FirstOrDefaultAsync();

        }
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UserEmailExists(string email)
        {
            return await _context.Users
                .AnyAsync(u => u.UserEmail.ToLower() == email.ToLower());
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.UserEmail.ToLower() == email.ToLower());
        }
    }
}
