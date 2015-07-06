using System;

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


        public static int DamerauLevenshteinDistanceTo(this string s, string t)
        {
            if (string.IsNullOrEmpty(s))
            {
                return !string.IsNullOrEmpty(t) ? t.Length : 0;
            }

            if (string.IsNullOrEmpty(t))
            {
                return !string.IsNullOrEmpty(s) ? s.Length : 0;
            }

            var length1 = s.Length;
            var length2 = t.Length;

            var d = new int[length1 + 1, length2 + 1];

            for (var i = 0; i <= d.GetUpperBound(0); i++)
            {
                d[i, 0] = i;
            }

            for (var i = 0; i <= d.GetUpperBound(1); i++)
            {
                d[0, i] = i;
            }

            for (var i = 1; i <= d.GetUpperBound(0); i++)
            {
                for (var j = 1; j <= d.GetUpperBound(1); j++)
                {
                    var cost = s[i - 1] == t[j - 1] ? 0 : 1;
                    var del = d[i - 1, j] + 1;
                    var ins = d[i, j - 1] + 1;
                    var sub = d[i - 1, j - 1] + cost;

                    d[i, j] = Math.Min(del, Math.Min(ins, sub));
                    if (i > 1 && j > 1 && s[i - 1] == t[j - 2] && s[i - 2] == t[j - 1])
                    {
                        d[i, j] = Math.Min(d[i, j], d[i - 2, j - 2] + cost);
                    }
                }
            }

            return d[d.GetUpperBound(0), d.GetUpperBound(1)];
        }
    }
}