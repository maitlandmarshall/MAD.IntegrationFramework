using MAD.IntegrationFramework.Configuration;
using Microsoft.EntityFrameworkCore;
using System;

namespace MAD.IntegrationFramework.Database
{
    internal class SqlServerMIFDbContextFactory<TDbContext> : IMIFDbContextFactory<TDbContext>
        where TDbContext : MIFDbContext
    {
        private readonly IAutomaticMigrationService automaticMigrationService;
        private readonly IMIFConfigFactory mifConfigFactory;

        public SqlServerMIFDbContextFactory(IAutomaticMigrationService automaticMigrationService,
                                            IMIFConfigFactory mifConfigFactory)
        {
            this.automaticMigrationService = automaticMigrationService;
            this.mifConfigFactory = mifConfigFactory;
        }

        public MIFDbContext Create(Type dbContextType)
        {
            if (typeof(MIFDbContext).IsAssignableFrom(dbContextType))
                throw new ArgumentException($"{nameof(dbContextType)} must be Type {nameof(MIFDbContext)}.");

            MIFDbContext dbContext = Activator.CreateInstance(dbContextType) as MIFDbContext;
            dbContext.Configuring += this.DbContext_Configuring;

            try
            {
                dbContext.Database.SetCommandTimeout(TimeSpan.FromMinutes(10));
                dbContext.Database.EnsureCreated();

                this.automaticMigrationService.EnsureDatabaseUpToDate(dbContext);
            }
            finally
            {
                dbContext.Configuring -= this.DbContext_Configuring;
            }

            return dbContext;
        }

        public TDbContext Create()
        {
            return this.Create(typeof(TDbContext)) as TDbContext;
        }

        private void DbContext_Configuring(object sender, DbContextOptionsBuilder e)
        {
            MIFConfig config = this.mifConfigFactory.Create();

            if (String.IsNullOrEmpty(config.SqlConnectionString))
                throw new ConnectionStringIsNullOrEmptyException();

            e.UseSqlServer(config.SqlConnectionString);
        }
    }
}
