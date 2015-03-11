using System;

namespace Erbsenzaehler.Models
{
    public class Month
    {
        public Month(string yearAndMonth, bool useCurrentAsFallback = true)
        {
            try
            {
                if (string.IsNullOrEmpty(yearAndMonth))
                {
                    throw new Exception("No year and month specified.");
                }

                if (yearAndMonth.IndexOf("-", StringComparison.InvariantCulture) != 4)
                {
                    throw new Exception("'" + yearAndMonth + "' is not a valid year and month.");
                }

                Date = new DateTime(int.Parse(yearAndMonth.Substring(0, 4)), int.Parse(yearAndMonth.Substring(5)), 1);
            }
            catch
            {
                if (useCurrentAsFallback)
                {
                    Date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                }
                else
                {
                    throw;
                }
            }
        }

        public DateTime Date { get; set; }

        public override string ToString()
        {
            return Date.ToString("yyyy-MM");
        }
    }
}