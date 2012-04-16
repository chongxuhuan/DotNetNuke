using System;

using DotNetNuke.Services.Localization;

namespace DotNetNuke.Common.Utilities
{
    /// <summary>
    /// Provides utility methods to work with Dates
    /// </summary>
    public class DateUtils
    {
        /// <summary>
        /// Returns a string with the pretty printed amount of time since the specified date.
        /// </summary>
        public static string CalculateDateForDisplay(DateTime date)
        {
            var utcTimeDifference = Services.SystemDateTime.SystemDateTime.GetCurrentTimeUtc() - date;

            if (utcTimeDifference.TotalSeconds < 60)
            {
                return (int)utcTimeDifference.TotalSeconds + Localization.GetString("SecondsAgo");
            }

            if (utcTimeDifference.TotalMinutes < 60)
            {
                if (utcTimeDifference.TotalMinutes < 2)
                {
                    return (int)utcTimeDifference.TotalMinutes + Localization.GetString("MinuteAgo");
                }

                return (int)utcTimeDifference.TotalMinutes + Localization.GetString("MinutesAgo");
            }

            if (utcTimeDifference.TotalHours < 24)
            {
                if (utcTimeDifference.TotalHours < 2)
                {
                    return (int)utcTimeDifference.TotalHours + Localization.GetString("HourAgo");
                }

                return (int)utcTimeDifference.TotalHours + Localization.GetString("HoursAgo");
            }

            if (utcTimeDifference.TotalDays < 7)
            {
                if (utcTimeDifference.TotalDays < 2)
                {
                    return (int)utcTimeDifference.TotalDays + Localization.GetString("DayAgo");
                }

                return (int)utcTimeDifference.TotalDays + Localization.GetString("DaysAgo");
            }

            if (utcTimeDifference.TotalDays < 30)
            {
                if (utcTimeDifference.TotalDays < 14)
                {
                    return (int)utcTimeDifference.TotalDays / 7 + Localization.GetString("WeekAgo");
                }

                return (int)utcTimeDifference.TotalDays / 7 + Localization.GetString("WeeksAgo");
            }

            if (utcTimeDifference.TotalDays < 180)
            {
                if (utcTimeDifference.TotalDays < 60)
                {
                    return (int)utcTimeDifference.TotalDays / 30 + Localization.GetString("MonthAgo");
                }

                return (int)utcTimeDifference.TotalDays / 30 + Localization.GetString("MonthsAgo");
            }

            // anything else (this is the only time we have to personalize it to the user)
            return date.ToShortDateString();
        }
    }
}
