using MAD.IntegrationFramework.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MAD.IntegrationFramework.Integrations
{
    [MIFTable]
    internal class TimedIntegrationLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string InterfaceName { get; set; }

        [Required]
        public DateTime StartDateTime { get; set; }

        public DateTime? EndDateTime { get; set; }

        [Required]
        public string ExecutablePath { get; set; }

        [Required]
        public string MachineName { get; set; }
    }
}
