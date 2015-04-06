namespace Erbsenzaehler.SummaryMail
{
    public interface IMailer
    {
        void SendHtmlMail(string sender, string recipient, string subject, string htmlBody);
    }
}
