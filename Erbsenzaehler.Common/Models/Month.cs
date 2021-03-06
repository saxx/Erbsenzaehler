﻿using System;

namespace Erbsenzaehler.Models
{
    public class Month
    {
        public Month(DateTime date)
        {
            Date = new DateTime(date.Year, date.Month, 1);
        }


        public Month()
        {
            Date = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        }


        public Month(string yearAndMonth, bool useCurrentAsFallback = true)
        {
            try
            {
                if (string.IsNullOrEmpty(yearAndMonth))
                {
                    if (useCurrentAsFallback)
                    {
                        Date = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
                    }
                    else
                    {
                        throw new ArgumentException("No year and month specified.", nameof(yearAndMonth));
                    }
                }
                else if (yearAndMonth.IndexOf("-", StringComparison.InvariantCulture) != 4)
                {
                    throw new ArgumentException("'" + yearAndMonth + "' is not a valid year and month.", nameof(yearAndMonth));
                }
                else
                {
                    Date = new DateTime(int.Parse(yearAndMonth.Substring(0, 4)), int.Parse(yearAndMonth.Substring(5)), 1);
                }
            }
            catch
            {
                if (useCurrentAsFallback)
                {
                    Date = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
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


        public bool IsCurrentMonth => Date.Year == DateTime.UtcNow.Year && Date.Month == DateTime.UtcNow.Month;

        public Month PreviousMonth => new Month(Date.AddMonths(-1).ToString("yyyy-MM"), false);

        public Month NextMonth => new Month(Date.AddMonths(1).ToString("yyyy-MM"), false);

        /// <summary>
        ///     Returns the total number of days in this month.
        /// </summary>
        public int NumberOfDays => DateTime.DaysInMonth(Date.Year, Date.Month);

        /// <summary>
        ///     Returns the number of days left in this month, relative to the current date.
        ///     Returns 0 when this month is in the past, or the total number of days when this month is in the future.
        /// </summary>
        public int NumberOfDaysLeft
        {
            get
            {
                if (IsCurrentMonth)
                {
                    return NumberOfDays - DateTime.UtcNow.Day;
                }
                if (DateTime.UtcNow < Date)
                {
                    return NumberOfDays;
                }
                return 0;
            }
        }
    }
}