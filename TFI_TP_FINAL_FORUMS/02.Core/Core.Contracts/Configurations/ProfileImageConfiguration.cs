using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Contracts.Configurations
{
    public class ProfileImageConfiguration
    {
        public string DefaultAvatar { get; set; }
        public string[] ValidExtensions { get; set; }
    }
}
