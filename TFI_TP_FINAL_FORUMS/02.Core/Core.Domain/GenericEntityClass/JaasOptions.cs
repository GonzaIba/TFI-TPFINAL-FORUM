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
        public string AppId { get; set; } = default!;
        public string ApiKeyId { get; set; } = default!;
        public string PrivateKeyPath { get; set; } = default!;
        public string ServerUrl { get; set; } = "https://8x8.vc";
        public string Issuer { get; set; } = "chat";
        public int GraceSeconds { get; set; } = 30;
    }
}
