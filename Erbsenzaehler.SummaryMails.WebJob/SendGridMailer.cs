using System.Net;
using System.Net.Mail;
using Erbsenzaehler.SummaryMail;
using SendGrid;

namespace Erbsenzaehler.SummaryMails.WebJob
{
    public class SendGridMailer : IMailer
    {
        public void SendHtmlMail(string recipient, string subject, string htmlBody)
        {
            // build message
            var message = new SendGridMessage
            {
                From = new MailAddress(Config.SendGridSender),
                Subject = subject,
                Html = htmlBody
            };
            message.AddTo(recipient);

            message.EnableClickTracking();

            if (string.IsNullOrEmpty(Config.SendGridUserName) || string.IsNullOrEmpty(Config.SendGridPassword))
            {
                return;
            }

            var credentials = new NetworkCredential(Config.SendGridUserName, Config.SendGridPassword);
            var transportWeb = new Web(credentials);
            transportWeb.DeliverAsync(message).Wait();
        }
    }
}