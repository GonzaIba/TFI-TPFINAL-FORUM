using System;

namespace Core.Domain.Response
{
    public class HelpRequestChatsResponse
    {
        public int ChatId { get; set; }
        public int RequestId { get; set; }
        public string State { get; set; } = "Abierto";
        public DateTime CreatedAt { get; set; }
        public bool Active { get; set; }

        public UsersForumPreviewResponse Other { get; set; } = null!;

        public LastMessageView? LastMessage { get; set; }
        public int UnreadCount { get; set; }

        public sealed class LastMessageView
        {
            public string Preview { get; set; } = null!;
            public DateTime At { get; set; }
            public bool FromMe { get; set; }
        }
    }
}
