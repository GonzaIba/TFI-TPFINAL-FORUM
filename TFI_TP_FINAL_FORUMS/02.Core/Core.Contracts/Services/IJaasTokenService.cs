using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Contracts.Services
{
    public interface IJaasTokenService
    {
        string CreateToken(string appId, string room, DateTimeOffset notBefore, DateTimeOffset expiresAt,
                   string? userId = null, string? displayName = null, string? email = null, string? avatarUrl = null,
                   string? issuerOverride = null);
    }
}
