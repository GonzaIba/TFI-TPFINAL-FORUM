using System;
using System.Collections.Generic;
using System.Text;

namespace CrossCutting.Helpers
{
    public class FrontConfiguration
    {
        private string _confirmAccountPage;
        private string _recoverPasswordPage;
        private string _linkEmailWeb;
        private string _linkChangeInformation;
        private string _returnUrl;

        public string BaseUrl { get; set; }

        public string ReturnUrl
        {
            get => BaseUrl + _returnUrl;
            set => _returnUrl = value;
        }

        public string ConfirmAccountPage
        {
            get => BaseUrl + _confirmAccountPage;
            set => _confirmAccountPage = value;
        }

        public string RecoverPasswordPage
        {
            get => BaseUrl + _recoverPasswordPage;
            set => _recoverPasswordPage = value;
        }

        public string LinkEmailWeb
        {
            get => BaseUrl + _linkEmailWeb;
            set => _linkEmailWeb = value;
        }


        public string LinkChangeInformation
        {
            get => BaseUrl + _linkChangeInformation;
            set => _linkChangeInformation = value;
        }
    }
}
