using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Application.Common;

namespace UserService.Application.Interfaces.Services
{
    public interface IOtpService
    {
        Task<ServiceResult<Guid>> GenerateAndSendOtpAsync(Guid userId, string purpose);
        Task<ServiceResult<Guid>> ValidateOtpAsync(Guid userId, string otpCode, string purpose);
    }
}
