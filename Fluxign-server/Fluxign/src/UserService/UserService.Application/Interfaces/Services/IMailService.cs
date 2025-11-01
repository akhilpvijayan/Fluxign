using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Domain.Enums;

namespace UserService.Application.Interfaces.Services
{
    public interface IMailService
    {
        Task SendOtp(string otpCode, string otpPurpose, string email, string username);
        Task SendPasswordReset(string resetLink, string email);
    }
}
