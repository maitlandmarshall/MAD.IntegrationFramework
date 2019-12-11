using System;
using System.Collections.Generic;
using System.Text;

namespace MaitlandsInterfaceFramework.Pardot.Api
{
    public class BulkQueryParameters
    {
        public DateTime? CreatedBefore { get; set; }
        public DateTime? CreatedAfter { get; set; }
        public DateTime? UpdatedBefore { get; set; }
        public DateTime? UpdatedAfter { get; set; }
        public int? IdGreaterThan { get; set; }
        public int? IdLessThan { get; set; }
    }
}
