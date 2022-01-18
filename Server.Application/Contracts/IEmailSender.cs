using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Application.Contracts
{
    public interface IEmailSender
    {
        void SendEmailAsync(string to, string subject, string content);
        void SendEmailVerificationLink(string to, string subject, string link);
        void SendEmaiForgotPassowrdLink(string to, string subject, string link);

    }
}