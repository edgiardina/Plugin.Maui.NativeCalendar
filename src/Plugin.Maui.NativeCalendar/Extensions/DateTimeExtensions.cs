using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.Maui.NativeCalendar.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Returns `long` representation of the DateTime in milliseconds since January 1, 1970 00:00:00
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long ToLongInteger(this DateTime dateTime)
        {
            return new DateTimeOffset(dateTime.Date).ToUnixTimeMilliseconds();
        }

    }
}
