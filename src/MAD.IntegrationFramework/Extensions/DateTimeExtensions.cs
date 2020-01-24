using System;
using System.Collections.Generic;
using System.Text;

namespace MAD.IntegrationFramework.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime SetTime (this DateTime dateTime, int hour, int minute, int second)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, hour, minute, second);
        }
    }
}
