using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Exceptions.BaseException
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
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
        //json property ignore if is null
        [JsonProperty]
        public string Code { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string StatusCode { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int HttpCode { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string NameError { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Trace { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Image { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CustomMessage { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CustomTitle { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CustomImage { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CustomNameError { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CustomStatusCode { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int CustomHttpCode { get; set; }
    }
}
