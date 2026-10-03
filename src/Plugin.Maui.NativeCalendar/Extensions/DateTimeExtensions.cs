namespace Plugin.Maui.NativeCalendar.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Returns the UTC midnight of the date as milliseconds since January 1, 1970 00:00:00 UTC.
        /// MaterialDatePicker works in UTC, so a local offset here shifts the date by a day for
        /// time zones east of UTC.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long ToLongInteger(this DateTime dateTime)
        {
            var date = dateTime.Date;
            return new DateTimeOffset(date.Year, date.Month, date.Day, 0, 0, 0, TimeSpan.Zero).ToUnixTimeMilliseconds();
        }

        /// <summary>
        /// The inverse of <see cref="ToLongInteger"/>. Returns the calendar day of a UTC
        /// millisecond value, with no time of day and no time zone shift.
        /// </summary>
        internal static DateTime ToCalendarDate(this long utcMilliseconds)
        {
            var date = DateTimeOffset.FromUnixTimeMilliseconds(utcMilliseconds).UtcDateTime.Date;
            return DateTime.SpecifyKind(date, DateTimeKind.Unspecified);
        }
    }
}
