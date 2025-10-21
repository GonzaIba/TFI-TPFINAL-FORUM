using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Response
{
    public class SessionResponse
    {
        public string CodeSession { get; set; }
        public string Domain { get; set; }
        public string RoomName { get; set; }
        public DateTime InitAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsOwner { get; set; }

        // JaaS
        public string Provider { get; set; } = "jaas";
        public string AppId { get; set; } = default!;
        public string Room { get; set; } = default!;            // <appId>/<roomName>
        public string Jwt { get; set; } = default!;
        public string ServerUrl { get; set; } = default!;
        public string Role { get; set; } = "participant";
        public DateTime ShouldCloseAt { get; set; }

        public UiSettings Ui { get; set; } = new();
    }

    public class UiSettings
    {
        public string? DisplayName { get; set; }
        public string? AvatarUrl { get; set; }
        public bool StartWithAudioMuted { get; set; } = true;
        public bool StartWithVideoMuted { get; set; } = true;
    }
}
