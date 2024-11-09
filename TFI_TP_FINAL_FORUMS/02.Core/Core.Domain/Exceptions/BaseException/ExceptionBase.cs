using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

        public ExceptionBase(string code, string message, string title, string image, string nameError, HttpStatusCode statusCode)
        {
            Code = code;
            CustomMessage = message;
            CustomTitle = title;
            CustomImage = image;
            CustomNameError = nameError;
            CustomStatusCode = statusCode.ToString();
            CustomHttpCode = (int)statusCode;
        }

        public string Code { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string StatusCode { get; set; }
        public int HttpCode { get; set; }
        public string NameError { get; set; }
        public string Trace { get; set; }
        public string Image { get; set; }

        public string CustomMessage { get; set; }
        public string CustomTitle { get; set; }
        public string CustomImage { get; set; }
        public string CustomNameError { get; set; }
        public string CustomStatusCode { get; set; }
        public int CustomHttpCode { get; set; }
    }
}
