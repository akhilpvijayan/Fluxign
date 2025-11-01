using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Application.DTOs
{
    public class NotificationRequestDto
    {
        public int RequestType { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string? Otp { get; set; }
        public string? OtpPurpose { get; set; }
        public string? RedirectUrl { get; set; }
        public string? Title { get; set; }
    }
}
