using Core.Domain.Exceptions.BaseException;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Response.BaseResponse
{
    public class GenericApiResponse<T>
    {
        public Meta Meta { get; set; } = new();
        public T Data { get; set; }
        public Errors Errors { get; set; } = new();

        public bool HasErrors()
        {
            return this.Errors.ErrorsList.Any();
        }
    }

    public class Meta
    {
        public Meta() { }
        public Meta(string service)
        {
            this.Service = service;
        }
        public string Method { get; set; }
        public string Service { get; set; }
        public string ResponseCode { get; set; }
    }

    public class Errors
    {
        public List<ExceptionBase> ErrorsList { get; set; } = new();
    }
}
