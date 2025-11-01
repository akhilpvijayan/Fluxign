using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Domain.Enums
{
    public enum OtpPurposeEnum
    {
        [Display(Name = "Login")]
        Login,

        [Display(Name = "Reset Password")]
        ResetPassword,

        [Display(Name = "Email Verification")]
        EmailVerification,

        [Display(Name = "Mobile Number Update")]
        MobileUpdate
    }
}
