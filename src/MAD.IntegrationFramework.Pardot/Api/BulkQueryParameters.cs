using System;
using System.Collections.Generic;
using System.Text;

namespace MAD.IntegrationFramework.Pardot.Api
{
    public enum SortOrder
    {
        Ascending,
        Descending
    }

    public class BulkQueryParameters
    {
        public DateTime? CreatedBefore { get; set; }
        public DateTime? CreatedAfter { get; set; }
        public DateTime? UpdatedBefore { get; set; }
        public DateTime? UpdatedAfter { get; set; }
        public int? IdGreaterThan { get; set; }
        public int? IdLessThan { get; set; }

        public string SortBy { get; set; }
        public SortOrder? SortOrder { get; set; }

        public int? Take { get; set; }
    }
}
