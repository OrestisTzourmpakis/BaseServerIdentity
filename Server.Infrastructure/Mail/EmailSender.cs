using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Server.Application.Contracts;
using Server.Infrastructure.Options;

namespace Server.Infrastructure.Mail
{

    public class EmailSender : IEmailSender
    {
        private MailOptions _mailOptions;

        public EmailSender(IOptionsSnapshot<MailOptions> mailOptions)
        {
            _mailOptions = mailOptions.Value;
        }

        public async Task SendEmailAsync(string to, string subject, string htmlMessage)
        {
            var fromAddress = new MailAddress(_mailOptions.FromEmailAddress, _mailOptions.DisplayName);
            var toAddress = new MailAddress(to, "");
            var smtp = new SmtpClient
            {
                Host = _mailOptions.Server,
                Port = _mailOptions.Port,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential(fromAddress.Address, _mailOptions.FromPassword),
                Timeout = 20000
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                //Body = content
            })
            {
                smtp.Send(message);
            };
        }
    }
}