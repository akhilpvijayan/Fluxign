using SignatureService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignatureService.Application.Interfaces.Services
{
    public interface IRequestServiceClient
    {
        Task<SigningRecipientDto?> GetUserByUserToken(string token);
    }
}
