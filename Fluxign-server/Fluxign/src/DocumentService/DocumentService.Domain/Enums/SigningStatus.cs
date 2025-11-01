using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequestService.Domain.Enum
{
    public enum SigningStatus
    {
        Draft,
        Completed,
        Cancelled
    }

    public enum SigningRecipientStatus
    {
        Pending,
        Signed,
        Rejected
    }
}
