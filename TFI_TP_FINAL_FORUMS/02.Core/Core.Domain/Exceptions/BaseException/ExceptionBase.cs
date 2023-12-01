using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Exceptions.BaseException
{
    [Serializable]
    public class ExceptionBase : Exception
    {
        [JsonIgnore]
        public new virtual System.Reflection.MethodBase TargetSite { get; }

        public ExceptionBase(string code)
        {
            Code = code;
        }

        public ExceptionBase(string code, string description, string title, string image)
        {
            Code = code;
            Description = description;
            Title = title;
            Image = image;
        }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
    }
}
