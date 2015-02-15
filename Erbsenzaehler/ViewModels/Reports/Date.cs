using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Erbsenzaehler.ViewModels.Reports
{
    public class Date
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
    }
}