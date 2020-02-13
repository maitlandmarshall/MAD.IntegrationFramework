using MAD.IntegrationFramework.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MAD.IntegrationFramework.Integrations
{
    internal class TimedIntegrationLogDbContext : MIFDbContext
    {
        public DbSet<TimedIntegrationLog> TimedIntegrationLogs { get; set; }
    }
}
