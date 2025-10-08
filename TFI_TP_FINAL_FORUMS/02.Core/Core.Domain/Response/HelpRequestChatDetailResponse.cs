using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Response
{
    public sealed class HelpRequestChatDetailResponse
    {
        public int ChatCode { get; set; }
        public int RequestCode { get; set; }
        public string State { get; set; } = "Abierto";
        public DateTime CreatedAt { get; set; }
        public bool Active { get; set; }

        public UsersForumPreviewResponse Other { get; set; } = null!;

        public int UnreadCount { get; set; }
        public IEnumerable<MessageView> Messages { get; set; } = Array.Empty<MessageView>();

        public sealed class MessageView
        {
            public int CodeMessage { get; set; }
            public string Text { get; set; } = "";
            public DateTime At { get; set; }
            public bool FromMe { get; set; }
            public bool ReadByOther { get; set; }
        }
    }
}
