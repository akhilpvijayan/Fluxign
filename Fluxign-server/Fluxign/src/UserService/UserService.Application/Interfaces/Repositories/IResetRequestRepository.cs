using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Domain.Entities;

namespace UserService.Application.Interfaces.Repositories
{
    public interface IResetRequestRepository
    {
        Task CreateAsync(PasswordResetRequest passwordResetRequest);
        Task UpdateAsync(PasswordResetRequest passwordResetRequest);
        Task<PasswordResetRequest> FindAsync(string hashToken);
    }
}
