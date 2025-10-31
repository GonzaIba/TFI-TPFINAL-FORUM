using Core.Domain.IdentityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Models
{
    public class UsersForumModel
    {
        public string IdUser { get; set; }
        public string? ShortDescriptionForum { get; set; }
        public string? LongDescriptionForum { get; set; }
        public string? ImageForum { get; set; }
        public bool Onboarded { get; set; }
        public bool HasSeenIntroPublications { get; set; }
        public bool HasSeenIntroLabels { get; set; }
        public bool HasSeenIntroUsers { get; set; }
        public bool HasSeenIntroLiveHelp { get; set; }
        public DateTime LastTimeConnectedForum { get; set; }

        public Users User { get; set; }
    }
}
