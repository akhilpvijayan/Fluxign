using RequestService.Application.DTOs;
using RequestService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequestService.Application.Interfaces.Repositories
{
    public interface ISigningRequestsRepository
    {
        Task AddAsync(SigningRequest request);
        Task<SigningRequest> GetRequestByIdAsync(Guid userId, Guid Id);
        Task DeleteAsync(Guid id);
        Task<List<SigningRequest>> GetAllRequestsByUserIdAsync(Guid Id);
        Task UpdateAsync(SigningRequest request);
        Task<List<RequestDashboardDto>> GetDashboardByUserIdAsync(Guid userId, DashboardQueryParameterDto pagingParams);
    }
}
