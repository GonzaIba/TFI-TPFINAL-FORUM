using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.GenericEntityClass
{
    public sealed class CursorPage<T>
    {
        public IEnumerable<T> Items { get; set; } = Array.Empty<T>();
        public string? NextCursor { get; set; }
        public bool HasNext { get; set; }
        public DateTime? AnchorUtc { get; set; }
    }
}
