using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Server.Application.Contracts;
using Server.Infrastructure.Helper;
using Server.Infrastructure.Options;

namespace Server.Infrastructure.Mail
{

    public enum EmailTemplatesEnum
    {
        EmailVerificationTemplate,
        ResetPasswordTemplate
    }
    public class EmailSender : IEmailSender
    {
        private string baseUrl;
        private MailOptions _mailOptions;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EmailSender(IOptionsSnapshot<MailOptions> mailOptions, IHttpContextAccessor httpContextAccessor)
        {
            _mailOptions = mailOptions.Value;
            _httpContextAccessor = httpContextAccessor;
            baseUrl = _httpContextAccessor.HttpContext.Request.Scheme + "://" + _httpContextAccessor.HttpContext.Request.Host.Value + "/";
        }



        public void SendEmailAsync(string to, string subject, string content)
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
            var message = new MailMessage(fromAddress, toAddress) { Subject = subject, Body = content };
            using (message)
            {
                smtp.Send(message);
            };
        }

        public void SendEmailVerificationLink(string to, string subject, string link)
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
                Body = CreateHTMLMessage("EmailVerificationTemplate", link),
                IsBodyHtml = true
            })
            {
                smtp.Send(message);
            }
        }

        public void SendEmailFromAdminVerificationLink(string to, string subject, string link, string password)
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
            //var templateLink = CreateHTMLMessage("EmailVerificationTemplate", link);
            var templatePassword = DefaultPasswordSetUp("EmailVerificationTemplateDefaultPassword", link, password);
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = templatePassword,
                IsBodyHtml = true
            })
            {
                smtp.Send(message);
            }
        }

        public void SendEmaiForgotPassowrdLink(string to, string subject, string link)
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
                Body = CreateHTMLMessage("ResetPasswordTemplate", link),
                IsBodyHtml = true
            })
            {
                smtp.Send(message);
            }
        }

        public void SendEmailFromWebSite(string topic, string message, string email)
        {
            var fromAddress = new MailAddress(_mailOptions.FromEmailAddress, _mailOptions.DisplayName);
            var toAddress = new MailAddress(_mailOptions.FromEmailAddress, "");
            var smtp = new SmtpClient
            {
                Host = _mailOptions.Server,
                Port = _mailOptions.Port,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential(fromAddress.Address, _mailOptions.FromPassword),
                Timeout = 20000
            };
            var htmlMmessage = new MailMessage(fromAddress, toAddress) { Subject = "From Web App", Body = CreateHTMLMessageFromWebiste(topic, message, email), IsBodyHtml = true };
            using (htmlMmessage)
            {
                smtp.Send(htmlMmessage);
            };
        }

        private string CreateHTMLMessage(string template, string link)
        {
            var file = $"./Utilities/EmailTemplates/{template}.html";
            var fileContent = File.ReadAllText(file);
            var placeHolder = (template.Equals("EmailVerificationTemplate"))
                ? "verifyemaillink"
                : "resetpasswordlink";
            fileContent = fileContent.Replace(placeHolder, link);
            return fileContent;
        }

        private string CreateHTMLMessageFromWebiste(string subject, string message, string email)
        {
            var file = $"./Utilities/EmailTemplates/EmailFromWebsiteTemplate.html";
            var fileContent = File.ReadAllText(file);
            fileContent = fileContent.Replace("websiteSubject", subject);
            fileContent = fileContent.Replace("websiteEmail", email);
            fileContent = fileContent.Replace("websiteMessage", message);
            return fileContent;
        }
        private string DefaultPasswordSetUp(string template, string link, string password)
        {
            var file = $"./Utilities/EmailTemplates/{template}.html";
            var fileContent = File.ReadAllText(file);
            var placeHolder = "verifyemaillink";
            fileContent = fileContent.Replace(placeHolder, link);
            string placeHolderPassword = "defaultPassword";
            fileContent = fileContent.Replace(placeHolderPassword, password);
            return fileContent;
        }


    }
}