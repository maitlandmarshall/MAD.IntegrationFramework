using MAD.IntegrationFramework.Configuration;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data.Common;

namespace MAD.IntegrationFramework.Database
{
    internal class SqlServerMIFDbContextFactory : IMIFDbContextFactory
    {
        private readonly MIFConfig config;
        private readonly Func<MIFDbContextBuilder> dbContextBuilderFactory;

        public SqlServerMIFDbContextFactory(MIFConfig config, Func<MIFDbContextBuilder> dbContextBuilderFactory)
        {
            this.config = config;
            this.dbContextBuilderFactory = dbContextBuilderFactory;
        }

        public MIFDbContext Create(Type dbContextType, DbConnection connection = null)
        {
            if (!typeof(MIFDbContext).IsAssignableFrom(dbContextType))
                throw new ArgumentException($"{nameof(dbContextType)} must be Type {nameof(MIFDbContext)}.");

            if (string.IsNullOrEmpty(this.config.SqlConnectionString))
                throw new ConnectionStringIsNullOrEmptyException();

            MIFDbContextBuilder dbContextBuilder = this.dbContextBuilderFactory().UseDbContext(dbContextType);

            if (connection != null)
            {
                dbContextBuilder.UseConnection(connection);
            }
            else
            {
                dbContextBuilder.UseConnectionString(this.config.SqlConnectionString);
                dbContextBuilder.UseAutomaticMigrations();
            }

            MIFDbContext dbContext = dbContextBuilder.Build();

            dbContext.Database.SetCommandTimeout(TimeSpan.FromMinutes(10));

            return dbContext;
        }
    }

    internal class SqlServerMIFDbContextFactory<TDbContext> : SqlServerMIFDbContextFactory, IMIFDbContextFactory<TDbContext>
        where TDbContext : MIFDbContext
    {
        public SqlServerMIFDbContextFactory(MIFConfig config, Func<MIFDbContextBuilder> dbContextBuilderFactory) : base(config, dbContextBuilderFactory)
        {
        }

        public TDbContext Create(DbConnection dbConnection = null)
        {
            return this.Create(typeof(TDbContext), dbConnection) as TDbContext;
        }
    }
}
