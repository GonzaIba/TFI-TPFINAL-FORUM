using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CrossCutting.EmailService.Contracts;
using CrossCutting.Helpers.ResultClasses;

namespace CrossCutting.EmailService.Services
{
    public abstract class GenericEmailService : IGenericEmailService
    {
        public abstract void SendEmail(Message message);
        public abstract Task<IGenericResult<string>> SendEmailAsync(Message message);
    }
}
