using DocumentService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentService.Application.Interfaces.Services
{
    public interface IRequestServiceClient
    {
        Task<SigningRecipientDto> GetRecipientByToken(string token);
    }
}
