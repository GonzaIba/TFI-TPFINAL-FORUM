using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Response
{
    public class PublicationResponse
    {
        public int CodePublication { get; set; }
        public string CodeUser { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int Reward { get; set; }
        public int Visits { get; set; }
        public int Answers { get; set; }
        public bool Answered { get; set; }
        public bool Closed { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ClosedDate { get; set; }
        public bool IsSaved { get; set; }

        public List<string> Tags { get; set; }
    }
}
