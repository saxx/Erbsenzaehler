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
            if (s == null)
            {
                return null;
            }

            while (s.Contains("   ") || s.Contains("  "))
            {
                s = s.Replace("   ", " ");
                s = s.Replace("  ", " ");
            }
            return s;
        }


        public static string Truncate(this string s, int maxLen, string addAfterTruncate = null)
        {
            if (string.IsNullOrEmpty(s))
            {
                return "";
            }

            if (s.Length > maxLen)
            {
                return s.Substring(0, maxLen) + addAfterTruncate;
            }

            return s;
        }


        public static string NullIfEmpty(this string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return null;
            }

            return s;
        }
    }
}