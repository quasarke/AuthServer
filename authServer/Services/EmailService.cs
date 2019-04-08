using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using AuthServer.Models;
using AuthServer.Settings;
using Microsoft.Extensions.Options;

namespace AuthServer.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailConfig _config;

        public EmailService(IOptions<EmailConfig> config)
        {
            _config = config.Value;
        }


        private async Task SendMailAsync(MailMessage message)
        {

            message.From = new MailAddress(_config.messageFrom, _config.messageFromDisplay);

            var smtpClient = new SmtpClient
            {
                Host = _config.smtpHost,
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_config.smtpUser, _config.smtpPassword)
            };

            try
            {
                await smtpClient.SendMailAsync(message);
            }
            catch (Exception)
            {
                await Task.FromResult(0);
            }
        }


        public async Task SendVerifyEmail(RegistrationViewModel model, string confirmcode)
        {
            var mailBody = "";
            await Task.Run(() => mailBody =
                File.ReadAllText(Startup.Environment.ContentRootPath + "\\EmailTemplates\\" +
                                 _config.VerifyEmailTemplate));

            mailBody = mailBody.Replace("{firstname}", model.FirstName);
            mailBody = mailBody.Replace("{lastname}", model.LastName);
            mailBody = mailBody.Replace("{why}", _config.VerifyEmailWhyUrl);
            mailBody = mailBody.Replace("{url}", _config.VerifyEmailUrl + "/Verify?confirmcode=" + UrlEncoder.Default.Encode(confirmcode));

            var mail = new MailMessage();
            mail.To.Add(new MailAddress(model.Email));
            mail.Subject = "Registration Confirmation";
            mail.IsBodyHtml = true;
            mail.Body = mailBody;
            await SendMailAsync(mail);
        }

        public async Task SendEmailWithUserName(string username, string email)
        {
            var mailBody = "";
            await Task.Run(() => mailBody =
                File.ReadAllText(Startup.Environment.ContentRootPath + "\\EmailTemplates\\" +
                                 _config.UserNameTemplate));

            mailBody = mailBody.Replace("{username}", username);

            var mail = new MailMessage();
            mail.To.Add(new MailAddress(email));
            mail.Subject = "Forgot Username";
            mail.IsBodyHtml = true;
            mail.Body = mailBody;
            await SendMailAsync(mail);
        }

        public async Task SendPasswordReset(string firstname, string email, string resetCode)
        {
            var mailBody = "";
            await Task.Run(() => mailBody =
                File.ReadAllText(Startup.Environment.ContentRootPath + "\\EmailTemplates\\" +
                                 _config.PasswordResetTemplate));

            mailBody = mailBody.Replace("{firstname}", firstname);
            mailBody = mailBody.Replace("{why}", _config.VerifyEmailWhyUrl);
            mailBody = mailBody.Replace("{url}", _config.PasswordResetUrl + "/ForgotFinish?resetCode=" + UrlEncoder.Default.Encode(resetCode));

            var mail = new MailMessage();
            mail.To.Add(new MailAddress(email));
            mail.Subject = "Password Reset";
            mail.IsBodyHtml = true;
            mail.Body = mailBody;
            await SendMailAsync(mail);
        }
    }

    public interface IEmailService
    {
        Task SendVerifyEmail(RegistrationViewModel model, string verifyUrl);

        Task SendEmailWithUserName(string username, string email);

        Task SendPasswordReset(string firstname, string email, string resetCode);
    }

}
