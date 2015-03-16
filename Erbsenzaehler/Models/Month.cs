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

        public bool IsCurrentMonth => Date.Year == DateTime.Now.Year && Date.Month == DateTime.Now.Month;

        public Month PreviousMonth => new Month(Date.AddMonths(-1).ToString("yyyy-MM"), false);

        public Month NextMonth => new Month(Date.AddMonths(1).ToString("yyyy-MM"), false);

        public int NumberOfDays => DateTime.DaysInMonth(Date.Year, Date.Month);

        public int NumberOfDaysLeft
        {
            get
            {
                if (IsCurrentMonth)
                {
                    return NumberOfDays - DateTime.Now.Day;
                }
                if (DateTime.Now < Date)
                {
                    return NumberOfDays;
                }
                return 0;
            }
        }
    }
}