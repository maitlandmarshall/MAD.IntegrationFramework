using System;
using System.Collections.Generic;
using System.Text;

namespace MAD.IntegrationFramework.LinkedIn
{
    public class LinkedInApiPaging
    {
        public int Count { get; set; }
        public int Start { get; set; }
        public int Total { get; set; }
    }

    public class LinkedInApiPaginatedResult <ElementType>
    {
        public List<ElementType> Elements { get; set; }
        public LinkedInApiPaging Paging { get; set; }
    }

    public struct LinkedInApiDateRange
    {
        public struct DayMonthYear
        {
            public int Day { get; set; }
            public int Month { get; set; }
            public int Year { get; set; }

            public override string ToString()
            {
                return $"{this.Day}/{this.Month}/{this.Year}";
            }
        }

        public DayMonthYear Start { get; set; }
        public DayMonthYear End { get; set; }
    }
}
