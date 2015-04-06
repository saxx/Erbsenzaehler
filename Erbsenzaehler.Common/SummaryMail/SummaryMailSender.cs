using Erbsenzaehler.Models;

namespace Erbsenzaehler.SummaryMail
{
    public class SummaryMailSender
    {
        #region Constructor

        private readonly SummaryMailRenderer _renderer;
        private readonly IMailer _mailer;


        public SummaryMailSender(SummaryMailRenderer renderer, IMailer mailer)
        {
            _mailer = mailer;
            _renderer = renderer;
        }

        #endregion

        public void RenderAndSend(User user)
        {
            const string subject = "Erbsenzähler Zusammenfassung";
            var html = _renderer.Render(user).Result;

            _mailer.SendHtmlMail("zusammenfassung@erbsenzaehler.azurewebsites.net", user.Email, subject, html);
        }
    }
}
