using NotificationService.Application.Common;
using NotificationService.Application.DTOs;
using NotificationService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Application.Interfaces.Services
{
    public interface INotificationService
    {
        Task SendNotificationAsync(NotificationRequestDto request);
    }
}
