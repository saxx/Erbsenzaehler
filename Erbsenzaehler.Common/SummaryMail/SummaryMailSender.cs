using System;
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
            var subject = "Erbsenzähler: Zusammenfassung vom " + DateTime.UtcNow.ToShortDateString();
            var html = _renderer.Render(user).Result;

            _mailer.SendHtmlMail(user.Email, subject, html);
        }
    }
}