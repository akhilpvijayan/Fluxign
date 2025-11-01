using RequestService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequestService.Application.Interfaces.Repositories
{
    public interface ISignaturePositionsRepository
    {
        Task<SignaturePosition> GetPositionsByRecepientId(Guid id, Guid requestId);
        Task AddAsync(SignaturePosition positions);
        Task DeleteByRequestIdAsync(Guid id);
        Task UpdateAsync(SignaturePosition positions);
    }
}
