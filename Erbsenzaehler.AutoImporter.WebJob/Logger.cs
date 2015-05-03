using NLog;

namespace Erbsenzaehler.AutoImporter.WebJob
{
    public class Logger : ILogger
    {
        public void Info(string message, params object[] placeholders)
        {
            NLog.Info(message, placeholders);
        }


        public void Trace(string message, params object[] placeholders)
        {
            NLog.Trace(message, placeholders);
        }


        public void Error(string message, params object[] placeholders)
        {
            NLog.Error(message, placeholders);
        }


        public void Fatal(string message, params object[] placeholders)
        {
            NLog.Fatal(message, placeholders);
        }


        private NLog.Logger NLog => LogManager.GetLogger("Erbsenzaehler.SummaryMails.WebJob");
    }
}