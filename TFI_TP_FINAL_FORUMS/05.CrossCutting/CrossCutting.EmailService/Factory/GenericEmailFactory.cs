using System;
using System.Collections.Generic;
using System.Text;
using CrossCutting.EmailService.Configurations;
using CrossCutting.EmailService.Contracts;

namespace CrossCutting.EmailService.Factory
{
    public class GenericEmailFactory : IGenericEmailFactory
    {
        private readonly IEmailSendGridService _emailSendGridService;
        private readonly IEmailSmtpService _emailSmtpService;
        private readonly EmailConfiguration _emailConfiguration;

        private readonly Dictionary<string, GenericEmailTypeEnum> _types;

        public GenericEmailFactory(
            IEmailSendGridService EmailSendGridService,
            IEmailSmtpService EmailSmtpService,
            EmailConfiguration EmailConfiguration)
        {
            _emailSendGridService = EmailSendGridService;
            _emailSmtpService = EmailSmtpService;
            _emailConfiguration = EmailConfiguration;

            _types = new Dictionary<string, GenericEmailTypeEnum>
            {
                { GenericEmailTypeEnum.SG.ToString()   , GenericEmailTypeEnum.SG  },
                { GenericEmailTypeEnum.SMTP.ToString() , GenericEmailTypeEnum.SMTP }
            };
        }

        public IGenericEmailService GetDefault() => Get(_types[_emailConfiguration.Type]);

        public IGenericEmailService Get(GenericEmailTypeEnum storageType) => storageType switch
        {
            GenericEmailTypeEnum.SG => _emailSendGridService,
            GenericEmailTypeEnum.SMTP => _emailSmtpService,
            _ => throw new Exception("Por favor indique que tipo de Email desea usar"),
        };

        public IGenericEmailService Get(string storageType) => Get(_types[storageType]);
    }
}
