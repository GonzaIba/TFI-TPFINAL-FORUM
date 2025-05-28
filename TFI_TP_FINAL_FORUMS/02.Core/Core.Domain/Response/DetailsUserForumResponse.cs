using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Response
{
    public class DetailsUserForumResponse
    {
        public string Name { get; set; }
        public string? LastName { get; set; }
        public string? LanguagePreference { get; set; }
        public string? Email { get; set; }
        public DateTime CreatedDate { get; set; }
        public decimal Score { get; set; }
        public int QuantityResponses { get; set; }
        public int NumberPostsCreated { get; set; }


        public string? ShortDescriptionForum { get; set; }
        public string? LongDescriptionForum { get; set; }
        public string? ImageForum { get; set; }
        public DateTime LastTimeConnectedForum { get; set; }
        public List<Medalla> Medals { get; set; }
    }

    public class Medalla
    {
        public string NameMedal { get; set; }
        public string Description { get; set; }
        public string ImageMedal { get; set; }
        public DateTime DateObtained { get; set; }
    }
}
