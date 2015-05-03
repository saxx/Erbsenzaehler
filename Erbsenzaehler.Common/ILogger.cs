
namespace Erbsenzaehler
{
    public interface ILogger
    {
        void Info(string message, params object[] placeholders);
        void Trace(string message, params object[] placeholders);
        void Error(string message, params object[] placeholders);
        void Fatal(string message, params object[] placeholders);
    }
}
