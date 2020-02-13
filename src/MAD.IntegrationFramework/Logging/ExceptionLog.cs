using MAD.IntegrationFramework.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MAD.IntegrationFramework.Logging
{
    [MIFTable]
    internal class ExceptionLog
    {
        [Key]
        public int Id { get; set; }

        public string Message { get; set; }
        public string Detail { get; set; }

        public string Interface { get; set; }

        public DateTime DateTime { get; set; }
    }

}
