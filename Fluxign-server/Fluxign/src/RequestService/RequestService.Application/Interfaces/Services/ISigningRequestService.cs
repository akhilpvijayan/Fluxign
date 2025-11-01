using RequestService.Application.DTOs;
using RequestService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RequestService.Application.Common;

namespace RequestService.Application.Interfaces.Services
{
    public interface ISigningRequestService
    {
        Task<ServiceResult<List<SigningRequest>>> GetAllRequestsByUserIdAsync(Guid userId);
        Task<ServiceResult<SigningRecipient>> GetRecipientByToken(string token);
        Task<ServiceResult<SigningRequestDto>> GetRequestByIdAsync(Guid userId, Guid requestId);
        Task<ServiceResult<Guid>> CreateSigningRequestAsync(SigningRequestDto request);
        Task<ServiceResult<Guid>> UpdateSigningRequestAsync(SigningRequestDto request);
        Task<ServiceResult<List<RequestDashboardDto>>> GetDashboardByUserIdAsync(Guid userId, DashboardQueryParameterDto pagingparams);
    }
}
