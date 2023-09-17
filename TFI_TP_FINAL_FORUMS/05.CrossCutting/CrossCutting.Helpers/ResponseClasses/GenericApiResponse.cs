using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossCutting.Helpers.ResponseClasses
{
    public class GenericApiResponse<T>
    {
        public GenericApiResponse() { }
        public GenericApiResponse(string service)
        {
            this.Service = service;
        }
        public string Service { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public T Data { get; set; }
    }
}
