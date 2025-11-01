using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequestService.Domain.Enum
{
    public enum SigningStatus
    {
        Draft = 0,
        Pending = 1,
        Completed = 2,
        Cancelled = 3
    }

    public enum SigningRecipientStatus
    {
        Signed,
        Rejected,
        Pending
    }
}
