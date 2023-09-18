using Core.Contracts.Services;
using Core.Domain.IdentityModels;
using CrossCutting.EmailService;
using CrossCutting.EmailService.Configurations;
using CrossCutting.EmailService.Factory;
using CrossCutting.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Core.Business.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly EmailSendGridConfiguration _emailSendGridConfiguration;
        private readonly IGenericEmailFactory _genericEmailFactory;

        public EmailService(
            ILogger<EmailService> logger,
            EmailSendGridConfiguration emailSendGridConfiguration,
            IGenericEmailFactory GenericEmailFactory
            )
        {
            _logger = logger;
            _emailSendGridConfiguration = emailSendGridConfiguration;
            _genericEmailFactory = GenericEmailFactory;
        }

        
    }
}
