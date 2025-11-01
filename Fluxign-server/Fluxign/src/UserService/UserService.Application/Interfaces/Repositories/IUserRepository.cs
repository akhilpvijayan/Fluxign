using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserService.Domain.Entities;

namespace UserService.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id);
        Task<IEnumerable<User>> GetAllAsync();
        Task<User> GetById(Guid Id);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task<bool> UserEmailExists(string email);
        Task<User> GetByEmailAsync(string email);
    }
}
