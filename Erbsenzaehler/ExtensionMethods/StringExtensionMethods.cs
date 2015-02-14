namespace Erbsenzaehler.ExtensionMethods
{
    public static class StringExtensionMethods
    {
        public static string RemoveCrlf(this string s)
        {
            return s?.Replace("\n", " ").Replace("\r", "");
        }


        public static string RemoveMultipleBlanks(this string s)
        {
            return s?.Replace("   ", " ").Replace("  ", " ");
        }
    }
}