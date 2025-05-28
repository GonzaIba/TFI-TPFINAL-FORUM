using CrossCutting.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Response
{
    public class UserForumResponse
    {
        public string Name { get; set; }
        public decimal Score { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Email { get; set; }
    }
}
