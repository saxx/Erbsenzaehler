using System;
using Erbsenzaehler.Models;

namespace Erbsenzaehler.SummaryMail
{
    public static class SummaryMailIntervalService
    {
        public static bool ShouldReceiveSummaryMail(SummaryMailIntervalOptions setting, DateTime? lastMailDate)
        {
            if (setting == SummaryMailIntervalOptions.Disable)
            {
                return false;
            }

            if (!lastMailDate.HasValue || lastMailDate == default(DateTime))
            {
                return true;
            }

            if (setting == SummaryMailIntervalOptions.Daily)
            {
                return lastMailDate.Value.Date != DateTime.UtcNow.Date;
            }

            if (setting == SummaryMailIntervalOptions.Weekly)
            {
                return lastMailDate.Value.Date != DateTime.UtcNow.Date && DateTime.UtcNow.DayOfWeek == DayOfWeek.Sunday;
            }

            if (setting == SummaryMailIntervalOptions.Monthly)
            {
                return lastMailDate.Value.Month != DateTime.UtcNow.Month || lastMailDate.Value.Year != DateTime.UtcNow.Year;
            }

            return false;
        }
    }
}