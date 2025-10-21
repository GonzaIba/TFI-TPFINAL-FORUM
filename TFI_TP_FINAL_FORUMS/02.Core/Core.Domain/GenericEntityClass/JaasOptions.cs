using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.GenericEntityClass
{
    public class JaasOptions
    {
        public const string SectionName = "Jaas";
        public string AppId { get; set; } = default!;           // vpaas-magic-cookie-xxx
        public string PrivateKeyPem { get; set; } = default!;   // PEM RSA (BEGIN PRIVATE KEY/BEGIN RSA PRIVATE KEY)
        public string ServerUrl { get; set; } = "https://8x8.vc";
        public string Issuer { get; set; } = "chat";            // iss
        public int GraceSeconds { get; set; } = 30;             // para shouldCloseAt
    }
}
