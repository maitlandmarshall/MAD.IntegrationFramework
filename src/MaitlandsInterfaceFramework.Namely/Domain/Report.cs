using System;
using System.Collections.Generic;
using System.Text;

namespace MaitlandsInterfaceFramework.Namely.Domain
{
    public class Report
    {
        public List<ReportColumn> Columns { get; set; }
        public List<string[]> Content { get; set; }
    }
}
