using System.IO;
using System.Net;
// using System.Net.Mail;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Server.Application.Contracts;
using Server.Infrastructure.Options;
using MailKit;
using MailKit.Net.Smtp;
using MimeKit;
using AutoMapper.Configuration.Annotations;
using Org.BouncyCastle.X509;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

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
            SendEmailDefault(subject, content, to);
        }

        public void SendEmailVerificationLink(string to, string subject, string link)
        {
            var bodyTemp = CreateHTMLMessage("EmailVerificationTemplate", link);
            SendEmailDefault(subject, bodyTemp, to);
        }

        public void SendEmailFromAdminVerificationLink(string to, string subject, string link, string password)
        {
            var templatePassword = DefaultPasswordSetUp("EmailVerificationTemplateDefaultPassword", link, password,to);
            SendEmailDefault(subject, templatePassword, to);
        }

        public void SendEmaiForgotPassowrdLink(string to, string subject, string link)
        {
            var bodyTemp = CreateHTMLMessage("ResetPasswordTemplate", link);
            SendEmailDefault(subject, bodyTemp, to);
        }

        public void SendEmailFromWebSite(string topic, string message, string email)
        {
            var bodyTemp = CreateHTMLMessageFromWebiste(topic, message, email);
            SendEmailDefault("Email from the application user form", bodyTemp, "");
        }

        private void SendEmailDefault(string subject, string body, string toEmailAddress)
        {
            var toEmailAddressTemp = (toEmailAddress == "") ? _mailOptions.FromEmailAddress : toEmailAddress;
            var mailMessage = new MimeMessage();
            mailMessage.From.Add(new MailboxAddress(_mailOptions.DisplayName, _mailOptions.FromEmailAddress));
            mailMessage.To.Add(MailboxAddress.Parse(toEmailAddressTemp));
            mailMessage.Subject = subject;
            var bodyTemp = new BodyBuilder()
            {
                HtmlBody = body
            };
            mailMessage.Body = bodyTemp.ToMessageBody();
            using (var smtpClient = new SmtpClient())
            {
                smtpClient.ServerCertificateValidationCallback = (object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) => { return true; };
                smtpClient.Connect(_mailOptions.Server, _mailOptions.Port, true);
                smtpClient.Authenticate(_mailOptions.FromEmailAddress, _mailOptions.FromPassword);
                smtpClient.Send(mailMessage);
                smtpClient.Disconnect(true);
            }
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
        private string DefaultPasswordSetUp(string template, string link, string password,string email)
        {
            var file = $"./Utilities/EmailTemplates/{template}.html";
            var fileContent = File.ReadAllText(file);
            var placeHolder = "verifyemaillink";
            fileContent = fileContent.Replace(placeHolder, link);
            string placeHolderPassword = "defaultPassword";
            fileContent = fileContent.Replace(placeHolderPassword, password);
            string placeHolderEmail = "useremail";
            fileContent = fileContent.Replace(placeHolderEmail, email);
            return fileContent;
        }


    }
}