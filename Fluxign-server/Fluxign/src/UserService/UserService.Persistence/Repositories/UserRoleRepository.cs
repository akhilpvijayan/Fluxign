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
    public class UserRoleRepository : IUserRoleRepository
    {
        private readonly UserServiceDbContext _context;

        public UserRoleRepository(UserServiceDbContext context)
        {
            _context = context;
        }

        public async Task<UserRole?> GetByNameAsync(string roleName)
        {
            return await _context.UserRoles.FirstOrDefaultAsync(x=>x.Role == roleName);
        }
    }
}
