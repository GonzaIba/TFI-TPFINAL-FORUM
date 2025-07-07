using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Request
{
    public class ParsedQueryRequest
    {
        public string Text { get; set; } = "";
        public SearchFilters Filters { get; set; } = new();
    }

    public class SearchFilters
    {
        public List<string> Tags { get; set; } = new();
        public string? UserName { get; set; }
        public string? Date { get; set; }
        public int? MinScore { get; set; }
    }
}
