using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Domain.Entities;

namespace UserService.Application.Interfaces.Repositories
{
    public interface IUserRoleRepository
    {
        Task<UserRole?> GetByNameAsync(string roleName);
    }
}
