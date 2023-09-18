using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Enum
{
    public enum ResponseCodeEnum
    {
        Success = 0,
        Duplicated = 1,
        NonExistent = 2,
        NotProcessed = 3,
        FailToSaveChanges = 4,
        Unathorized = 5,
        BlockedUser = 6,
        Expired = 7,
        LockedOut = 8,
        NotAllowed = 9,
        RequiresTwoFactor = 10,
        RequiresMembership = 11,
        NoHandleException = 99
    }
}
