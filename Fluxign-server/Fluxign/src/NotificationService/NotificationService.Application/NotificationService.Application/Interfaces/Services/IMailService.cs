using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Application.Interfaces.Services
{
    public interface IMailService
    {
        Task SendOtp(string otpCode, string otpPurpose, string email, string username);
        Task SendPasswordReset(string resetLink, string email);
        Task SendSigningLink(string requestTitle, string email, string firstName, string signingUrl);
    }
}
