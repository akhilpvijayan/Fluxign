using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Application.ViewModels
{
    public class OtpValidationRequest
    {
        public string OtpCode { get; set; }
        public string Purpose { get; set; }
    }

}
