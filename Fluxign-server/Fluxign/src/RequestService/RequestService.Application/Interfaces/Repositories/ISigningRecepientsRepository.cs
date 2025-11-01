using RequestService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequestService.Application.Interfaces.Repositories
{
    public interface ISigningRecepientsRepository
    {
        Task<List<SigningRecipient>> GetRecepientsById(Guid id);
        Task<SigningRecipient> GetRecipientByTokenAsync(string token);
        Task<SigningRecipient> GetRecepientByRecepientId(Guid id);
        Task AddAsync(SigningRecipient recepients);
        Task DeleteByRequestIdAsync(Guid id);
        Task UpdateAsync(SigningRecipient recepients);
    }
}
