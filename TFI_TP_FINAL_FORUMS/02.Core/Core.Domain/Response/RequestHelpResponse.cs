using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Response
{
    public class RequestHelpResponse
    {
        public UsersForumPreviewResponse UserCreator { get; set; }
        public string TitleHelp { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
        public List<string> Languages { get; set; }
        public List<string> Labels { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal Regard { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
