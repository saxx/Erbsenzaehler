namespace Erbsenzaehler.SummaryMail
{
    public interface IMailer
    {
        void SendHtmlMail(string recipient, string subject, string htmlBody);
    }
}