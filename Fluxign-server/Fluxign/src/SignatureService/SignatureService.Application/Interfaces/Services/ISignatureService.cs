using SignatureService.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignatureService.Application.Interfaces.Services
{
    public interface ISignatureService
    {
        Task<ServiceResult<bool>> VerifyUser(string code, string requestUrl, string token);
    }
}
