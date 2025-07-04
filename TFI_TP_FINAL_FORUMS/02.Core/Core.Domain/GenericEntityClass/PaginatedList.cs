using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.GenericEntityClass
{
    public class PaginatedList<T>
    {
        public PaginatedList()
        {
            List = new List<T>();
        }
        public PaginatedList(IEnumerable<T> list, int pageIndex, int pageCount, int totalCount, int totalPages)
        {
            List = list;
            PageIndex = pageIndex;
            PageCount = pageCount;
            TotalCount = totalCount;
            TotalPages = totalPages;
        }
        public int PageIndex { get; set; }
        public int PageCount { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public IEnumerable<T> List { get; set; }
    }
}
