using System;
using System.Collections.Generic;
using System.Text;

namespace MaitlandsInterfaceFramework.LinkedIn
{
    public class ApiAction
    {
        public string Actor { get; set; }
        public long Time { get; set; }

        public static DateTime AsDateTime(long time)
        {
            return DateTimeOffset.FromUnixTimeMilliseconds((long)time).UtcDateTime;
        }
    }
}
