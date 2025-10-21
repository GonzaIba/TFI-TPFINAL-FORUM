using Core.Contracts.Services;
using Core.Domain.GenericEntityClass;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Core.Business.Services
{
    public class JaasTokenService : IJaasTokenService
    {
        private readonly JaasOptions _opts;
        public JaasTokenService(IOptions<JaasOptions> options) => _opts = options.Value;

        public string CreateToken(string appId, string room, DateTimeOffset nbf, DateTimeOffset exp,
                                  string? userId = null, string? displayName = null, string? email = null, string? avatarUrl = null,
                                  string? issuerOverride = null)
        {
            // Cargar clave privada RSA desde PEM
            using var rsa = RSA.Create();
            var pem = _opts.PrivateKeyPem.Replace("\\n", "\n"); // por si viene con \n
            rsa.ImportFromPem(pem.AsSpan());

            var creds = new SigningCredentials(new RsaSecurityKey(rsa) { KeyId = appId }, SecurityAlgorithms.RsaSha256);

            // Claims básicos JaaS
            var claims = new List<Claim>
        {
            new("aud","jitsi"),
            new("iss", issuerOverride ?? _opts.Issuer),
            new("sub", appId),
            new("room", room) // room completo: <appId>/<roomName>
        };

            // context: user y features
            var context = new
            {
                user = new
                {
                    id = userId,
                    name = displayName,
                    email,
                    avatar = avatarUrl
                },
                features = new
                {
                    livestreaming = true,
                    transcription = false,
                    recording = false,
                    outbound_call = false
                }
            };

            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateJwtSecurityToken(
                issuer: issuerOverride ?? _opts.Issuer,
                audience: "jitsi",
                subject: new ClaimsIdentity(claims),
                notBefore: nbf.UtcDateTime,
                expires: exp.UtcDateTime,
                issuedAt: DateTime.UtcNow,
                signingCredentials: creds
                // JaaS acepta custom payload via "context"
                // Agregamos "context" al payload usando JwtPayload directamente
                // (CreateJwtSecurityToken ya creó payload; lo completamos):
                // Nota: handler no expone payload mutable, así que recreamos el token con payload extendido:
                // -> alternativa: crear SecurityTokenDescriptor + IDictionary
                // Para simplicidad, regeneramos:
                //additionalHeaderClaims: null
            );

            // Hack simple: volvemos a codificar con "context" usando WriteToken de un JwtSecurityToken nuevo
            var payload = token.Payload;
            payload["context"] = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(
                System.Text.Json.JsonSerializer.Serialize(context));

            var final = new JwtSecurityToken(token.Header, payload);
            return handler.WriteToken(final);
        }
    }
}
