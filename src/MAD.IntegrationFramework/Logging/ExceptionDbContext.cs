using MAD.IntegrationFramework.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MAD.IntegrationFramework.Logging
{
    internal class ExceptionDbContext : MIFDbContext
    {
        public ExceptionDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<ExceptionLog> ExceptionLogs { get; set; }
    }
}
