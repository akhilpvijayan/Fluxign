using NotificationService.Application.Common;
using NotificationService.Application.DTOs;
using NotificationService.Application.Interfaces.Services;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IMailService _mailService;
        public NotificationService(IMailService mailService)
        {
            _mailService = mailService;
        }

        public async Task SendNotificationAsync(NotificationRequestDto request)
        {
            if (!Enum.IsDefined(typeof(NotificationTypeEnum), request.RequestType))
            {
                throw new ArgumentException($"Invalid status value: {request.RequestType}");
            }

            var typeEnum = (NotificationTypeEnum)request.RequestType;

            if (typeEnum == NotificationTypeEnum.Otp)
            {
                await _mailService.SendOtp(request.Otp, request.OtpPurpose, request.Email, request.Name);
            }
            else if(typeEnum == NotificationTypeEnum.ResetPassword)
            {
                await _mailService.SendPasswordReset(request.RedirectUrl, request.Email);
            }
            else if(typeEnum == NotificationTypeEnum.SigningRequest)
            {
                await _mailService.SendSigningLink(request.Title, request.Email, request.Name, request.RedirectUrl);
            }
        }
    }
}
