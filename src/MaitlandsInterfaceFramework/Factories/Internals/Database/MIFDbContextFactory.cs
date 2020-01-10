using MaitlandsInterfaceFramework.Database;
using MaitlandsInterfaceFramework.Services.Internals.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaitlandsInterfaceFramework.Factories.Internals.Database
{
    internal class MIFDbContextFactory<DbContext> : IMIFDbContextFactory<DbContext> where DbContext : MIFDbContext, new()
    {
        private AutomaticMigrationService AutomaticMigrationService;

        public MIFDbContextFactory(AutomaticMigrationService automaticMigrationService)
        {
            this.AutomaticMigrationService = automaticMigrationService;
        }

        public DbContext Create()
        {
            DbContext dbContext = new DbContext();

            this.AutomaticMigrationService.EnsureDatabaseUpToDate(dbContext);

            return dbContext;
        }
    }
}
