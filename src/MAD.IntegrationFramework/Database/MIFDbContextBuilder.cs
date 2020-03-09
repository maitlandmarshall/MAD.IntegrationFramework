using MAD.IntegrationFramework.Configuration;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace MAD.IntegrationFramework.Database
{
    public class MIFDbContextBuilder
    {
        private readonly static object dbLock = new object();
        private readonly IAutomaticMigrationService automaticMigrationService;

        protected MIFConfig Config { get; }

        protected Type DbContextType { get; set; }
        protected string ConnectionString { get; set; }
        protected DbConnection Connection { get; set; }
        protected bool AutomaticMigrations { get; set; } = false;

        public MIFDbContextBuilder(MIFConfig config, IAutomaticMigrationService automaticMigrationService)
        {
            this.Config = config;
            this.automaticMigrationService = automaticMigrationService;
        }

        public MIFDbContextBuilder UseConnectionString(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException("Must have a value.", nameof(connectionString));

            this.ConnectionString = connectionString;
            this.Connection = null;

            return this;
        }

        public MIFDbContextBuilder UseConnection(DbConnection dbConnection)
        {
            this.Connection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
            this.ConnectionString = null;

            return this;
        }

        public MIFDbContextBuilder UseDbContext (Type dbContextType)
        {
            this.DbContextType = dbContextType;

            return this;
        }

        public MIFDbContextBuilder UseAutomaticMigrations(bool automaticMigrations = true)
        {
            this.AutomaticMigrations = automaticMigrations;

            return this;
        }

        public MIFDbContext Build()
        {
            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder();

            if (!String.IsNullOrEmpty(this.ConnectionString))
            {
                optionsBuilder.UseSqlServer(this.ConnectionString);
            }
            else if (this.Connection != null)
            {
                optionsBuilder.UseSqlServer(this.Connection);
            }
            else
            {
                throw new NotSupportedException($"A connection string or connection must be supplied.");
            }

            MIFDbContext dbContext = Activator.CreateInstance(
                this.DbContextType,
                new object[] { optionsBuilder.Options }
            ) as MIFDbContext;

            if (this.AutomaticMigrations)
            {
                this.automaticMigrationService.EnsureDatabaseUpToDate(dbContext);
            }

            return dbContext;
        }
    }

    public class MIFDbContextBuilder<TDbContext> : MIFDbContextBuilder
        where TDbContext : MIFDbContext
    {
        public MIFDbContextBuilder(MIFConfig config, IAutomaticMigrationService automaticMigrationService) : base(config, automaticMigrationService)
        {
            this.DbContextType = typeof(TDbContext);
        }

        public new TDbContext Build()
        {
            return base.Build() as TDbContext;
        }
    }
}
