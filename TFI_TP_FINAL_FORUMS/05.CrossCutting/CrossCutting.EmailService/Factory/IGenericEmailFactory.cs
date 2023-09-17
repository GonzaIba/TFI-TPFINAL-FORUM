using System;
using System.Collections.Generic;
using System.Text;
using CrossCutting.EmailService.Contracts;

namespace CrossCutting.EmailService.Factory
{
    public interface IGenericEmailFactory
    {
        IGenericEmailService GetDefault();

        IGenericEmailService Get(GenericEmailTypeEnum emailType);
    }
}
