using MAD.IntegrationFramework.Configuration;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data.Common;

namespace MAD.IntegrationFramework.Database
{
    internal class SqlServerMIFDbContextFactory : IMIFDbContextFactory
    {
        private readonly IAutomaticMigrationService automaticMigrationService;
        private readonly MIFConfig config;
        private readonly static object dbLock = new object();

        public SqlServerMIFDbContextFactory(IAutomaticMigrationService automaticMigrationService,
                                            MIFConfig config)
        {
            this.automaticMigrationService = automaticMigrationService;
            this.config = config;
        }

        public MIFDbContext Create(Type dbContextType, DbConnection dbConnection = null)
        {
            if (!typeof(MIFDbContext).IsAssignableFrom(dbContextType))
                throw new ArgumentException($"{nameof(dbContextType)} must be Type {nameof(MIFDbContext)}.");

            if (string.IsNullOrEmpty(this.config.SqlConnectionString))
                throw new ConnectionStringIsNullOrEmptyException();

            var optionsBuilder = new DbContextOptionsBuilder();

            if (dbConnection != null)
                optionsBuilder.UseSqlServer(dbConnection);
            else
                optionsBuilder.UseSqlServer(this.config.SqlConnectionString);

            MIFDbContext dbContext = Activator.CreateInstance(
                dbContextType,
                new object[] { optionsBuilder.Options }
            ) as MIFDbContext;

            dbContext.Database.SetCommandTimeout(TimeSpan.FromMinutes(10));
            
            if (dbConnection == null)
            {
                lock (dbLock)
                {
                    dbContext.Database.EnsureCreated();
                    this.automaticMigrationService.EnsureDatabaseUpToDate(dbContext);
                }
            }

            return dbContext;
        }
    }

    internal class SqlServerMIFDbContextFactory<TDbContext> : SqlServerMIFDbContextFactory, IMIFDbContextFactory<TDbContext>
        where TDbContext : MIFDbContext
    {
        public SqlServerMIFDbContextFactory(IAutomaticMigrationService automaticMigrationService,
                                            MIFConfig config) : base(automaticMigrationService, config) { }


        public TDbContext Create(DbConnection dbConnection = null)
        {
            return this.Create(typeof(TDbContext), dbConnection) as TDbContext;
        }
    }
}
