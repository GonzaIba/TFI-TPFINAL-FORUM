using System;
using System.Collections.Generic;

namespace Core.Domain.Request
{
    public class MarkChatReadRequest
    {
        public string UserId { get; set; } = null!;
        public int CodeChat { get; set; }
        public DateTime? UpToUtc { get; set; }
        public List<int>? MessageIds { get; set; }
    }
}
