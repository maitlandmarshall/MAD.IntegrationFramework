using System;
using System.Collections.Generic;
using System.Text;

namespace MaitlandsInterfaceFramework.Pardot.Domain
{
    public class Campaign
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal? Cost { get; set; }
    }
}
