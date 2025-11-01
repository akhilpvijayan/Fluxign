using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Application.Interfaces.Repositories
{
    public interface IUserOtpRepository
    {
        Task AddAsync(UserOtp user);
        Task<IEnumerable<UserOtp>> GetAllAsync();
        Task<UserOtp?> GetByIdAsync(Guid id);
        Task UpdateAsync(UserOtp user);
        Task<UserOtp?> ValidateOtp(Guid userId, string hasedOtp);
    }
}
