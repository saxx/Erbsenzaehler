using System;

namespace Erbsenzaehler.ExtensionMethods
{
    public static class DateTimeExtensionMethods
    {

        public static string ToRelativeDate(this DateTime date)
        {
            if (date == DateTime.MinValue)
            {
                return "Nie";
            }

            var differenceInHours = (int)Math.Ceiling((DateTime.UtcNow - date).TotalHours);
            if (differenceInHours == 1)
            {
                return "Vor einer Stunde";
            }
            if (differenceInHours <= 48)
            {
                return "Vor " + differenceInHours.ToString("N0") + " Stunden";
            }

            var differenceInDays = (int)Math.Ceiling(differenceInHours / 24.0);
            return "Vor " + differenceInDays + " Tagen";
        }
    }
}
