using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Response
{
    public class UsersForumPreviewResponse
    {
        public string CompleteName { get; set; }
        public string Initials { get; set; }
        public string? ShortDescription { get; set; }
        public string? LongDescription { get; set; }
        public string? Image { get; set; }
        public string? DateFrom { get; set; }
        public int Score { get; set; }
        public DateTime LastTimeOnline { get; set; }
    }
}
