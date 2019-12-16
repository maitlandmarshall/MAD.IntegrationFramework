using MaitlandsInterfaceFramework.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MaitlandsInterfaceFramework.Services.Internals
{
    [MIFTable]
    internal class TimedInterfaceLog
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

    internal class TimedInterfaceDbContext : MIFDbContext
    {
        public DbSet<TimedInterfaceLog> TimedInterfaceLogs { get; set; }
    }
}
