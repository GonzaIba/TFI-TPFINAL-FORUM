using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Response
{
    public class PublicationDetailResponse
    {
        public int CodePublication { get; set; }
        public UsersForumPreviewResponse User { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int Reward { get; set; }
        public int Visits { get; set; }
        public int Votes { get; set; }
        public bool? VotedPositive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ClosedDate { get; set; }
        public bool IsAuthor { get; set; }

        public List<AnswerResponse> Answers { get; set; } = new();
        public List<FilesResponse> Files { get; set; } = new();
    }

    public class FilesResponse
    {
        public string FileName { get; set; }
        public string TypeFile { get; set; }
        public byte[] File { get; set; }
    }

    public class AnswerResponse
    {
        public int CodeAnswer { get; set; }
        public UsersForumPreviewResponse User { get; set; }
        public string TextResponse { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool CorrectAnswer { get; set; }
        public int Votes { get; set; }
        public bool? VotedPositive { get; set; }
        public bool IsAuthor { get; set; }
        public List<FilesResponse> Files { get; set; } = new();
    }
}