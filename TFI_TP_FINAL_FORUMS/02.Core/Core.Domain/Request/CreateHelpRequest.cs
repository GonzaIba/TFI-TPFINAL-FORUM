using System.Collections.Generic;

namespace Core.Domain.Request
{
    public class CreateHelpRequest
    {
        public string TitleHelp { get; set; }
        public string Message { get; set; }
        public List<string> Labels { get; set; }
        public List<string> Languages { get; set; }
        public HelpTimeSlotContainer TimeSlot { get; set; }
        public string? UserId { get; set; }
    }

    public class HelpTimeSlotContainer
    {
        public List<HelpTimeSlot> Slots { get; set; }
    }

    public class HelpTimeSlot
    {
        public string Start { get; set; }
        public string End { get; set; }
    }
}
