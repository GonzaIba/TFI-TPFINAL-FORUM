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
using System.Threading.Tasks;
using System.IO;

namespace Core.Business.Services
{
    public class JaasTokenService : IJaasTokenService
    {
        private readonly JaasOptions _opts;
        public JaasTokenService(IOptions<JaasOptions> options) => _opts = options.Value;

        public string CreateToken(string appId, string room, DateTimeOffset nbf, DateTimeOffset exp,
                                  string? userId = null, string? displayName = null, string? email = null, string? avatarUrl = null,
                                  bool isModerator = false, string? issuerOverride = null)
        {
            using var rsa = LoadRsaFromOptions();
            var keyParams = rsa.ExportParameters(true); // clona material

            // si guardaste solo el sufijo (p.ej. "97b4f7"):
            var kid = _opts.ApiKeyId.Contains('/') ? _opts.ApiKeyId
                                                   : $"{_opts.AppId}/{_opts.ApiKeyId}";

            var rsaKey = new RsaSecurityKey(keyParams) { KeyId = kid };
            var creds = new SigningCredentials(rsaKey, SecurityAlgorithms.RsaSha256);

            // SOLO 'sub' y 'room' como claims personalizados
            var claims = new List<Claim>
            {
                new("sub", appId),
                new("room", room)
            };

            var context = new
            {
                user = new { id = userId, name = displayName, email, avatar = avatarUrl, moderator = isModerator },
                features = new { livestreaming = true, transcription = false, recording = false, outbound_call = false }
            };

            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateJwtSecurityToken(
                issuer: issuerOverride ?? _opts.Issuer, // 'iss'
                audience: "jitsi",                     // 'aud'
                subject: new ClaimsIdentity(claims),
                notBefore: nbf.UtcDateTime,
                expires: exp.UtcDateTime,
                issuedAt: DateTime.UtcNow,
                signingCredentials: creds
            );

            // Inyectamos 'context' en el payload
            var payload = token.Payload;
            payload["context"] = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(
                System.Text.Json.JsonSerializer.Serialize(context));

            var final = new JwtSecurityToken(token.Header, payload);
            return handler.WriteToken(final);
        }

        // ===== Loader: path, PEM/DER, PKCS#8 / PKCS#1 =====
        private RSA LoadRsaFromOptions()
        {
            if (string.IsNullOrWhiteSpace(_opts.PrivateKeyPath))
                throw new InvalidOperationException("Jaas: configure PrivateKeyPath o PrivateKeyPem.");

            var text = TryReadText(_opts.PrivateKeyPath);
            if (text != null) return ImportFromPemText(text);

            var der = File.ReadAllBytes(_opts.PrivateKeyPath);
            return ImportFromDer(der);
        }

        private static string? TryReadText(string path)
        {
            var content = File.ReadAllText(path);
            return (content.Contains("BEGIN PRIVATE KEY") || content.Contains("BEGIN RSA PRIVATE KEY")) ? content : null;
        }

        private static RSA ImportFromPemText(string pem)
        {
            var rsa = RSA.Create();
            pem = pem.Trim();

            if (pem.Contains("BEGIN PRIVATE KEY")) // PKCS#8
            {
                rsa.ImportFromPem(pem);
                return rsa;
            }
            if (pem.Contains("BEGIN RSA PRIVATE KEY")) // PKCS#1
            {
                var base64 = ExtractBase64(pem, "BEGIN RSA PRIVATE KEY", "END RSA PRIVATE KEY");
                var der = Convert.FromBase64String(base64);
                rsa.ImportRSAPrivateKey(der, out _);
                return rsa;
            }

            throw new ArgumentException("Jaas: Unsupported key format in PEM.");
        }

        private static RSA ImportFromDer(byte[] der)
        {
            var rsa = RSA.Create();
            if (Try(() => { rsa.ImportPkcs8PrivateKey(der, out _); })) return rsa;   // PKCS#8
            if (Try(() => { rsa.ImportRSAPrivateKey(der, out _); })) return rsa;     // PKCS#1
            throw new ArgumentException("Jaas: DER no reconocido (ni PKCS#8 ni PKCS#1). Convertí a PEM.");
        }

        private static bool Try(Action a) { try { a(); return true; } catch { return false; } }

        private static string ExtractBase64(string pem, string begin, string end)
        {
            var startTag = $"-----{begin}-----";
            var endTag = $"-----{end}-----";
            var start = pem.IndexOf(startTag, StringComparison.Ordinal);
            var stop = pem.IndexOf(endTag, StringComparison.Ordinal);
            if (start < 0 || stop < 0 || stop <= start) throw new ArgumentException("PEM inválido.");
            var body = pem[(start + startTag.Length)..stop];
            return new string(body.Where(c => !char.IsWhiteSpace(c)).ToArray());
        }
    }
}
