using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Erbsenzaehler.ViewModels.Reports
{
    public class Date : IComparable<Date>
    {
        public Date(int year, int month)
        {
            Year = year;
            Month = month;
        }

        public int Month { get; }
        public int Year { get; }

        public override bool Equals(object obj)
        {
            var secondDate = obj as Date;
            if (secondDate == null)
            {
                return false;
            }

            return Month == secondDate.Month && Year == secondDate.Year;
        }


        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }


        public override string ToString()
        {
            return new DateTime(Year, Month, 1).ToString("MMM yyyy");
        }

        public int CompareTo(Date other)
        {
            if (other == null)
                return 1;

            if (Year == other.Year)
            {
                if (Month == other.Month)
                    return 0;
                if (Month < other.Month)
                    return -1;
                return 1;
            }
            if (Year < other.Year)
                return -1;
            return 1;
        }

        public static bool operator <(Date d1, Date d2)
        {
            return d1.CompareTo(d2) < 0;
        }

        public static bool operator >(Date d1, Date d2)
        {
            return d1.CompareTo(d2) > 0;
        }

        public static bool operator <=(Date d1, Date d2)
        {
            return d1.CompareTo(d2) <= 0;
        }

        public static bool operator >=(Date d1, Date d2)
        {
            return d1.CompareTo(d2) >= 0;
        }

        public static Date operator +(Date date, int months)
        {
            var d = new DateTime(date.Year, date.Month, 1).AddMonths(months);
            return new Date(d.Year, d.Month);
        }
    }
}