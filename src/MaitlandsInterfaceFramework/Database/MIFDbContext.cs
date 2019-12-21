using MaitlandsInterfaceFramework.Database.Internals;
using MaitlandsInterfaceFramework.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace MaitlandsInterfaceFramework.Database
{
  

    public abstract class MIFDbContext : DbContext
    {
        private static List<Type> MigratedContexts = new List<Type>();
        private static volatile object SyncToken = new object();

        public DbConnection Connection
        {
            get => this.Database.GetDbConnection();
        }

        public MIFDbContext() : this(MIF.Config.SqlConnectionString) { }

        public MIFDbContext(string connectionString) : base(new DbContextOptionsBuilder().UseSqlServer(connectionString).Options)
        {
            this.Database.SetCommandTimeout(TimeSpan.FromMinutes(10));
            this.Database.EnsureCreated();

            lock (SyncToken)
            {
                if (!MigratedContexts.Contains(this.GetType()))
                {
                    try
                    {
                        AutomaticMigration.EnsureDatabaseUpToDate(this);
                    }
                    catch (Exception ex)
                    {
                        _ = ex.LogException();
                    }
                    finally
                    {
                        MigratedContexts.Add(this.GetType());
                    }
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            IEnumerable<PropertyInfo> dbSets = this.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic)
                .Where(y => y.PropertyType.Name.StartsWith("DbSet"));

            foreach (PropertyInfo dbSetPropInfo in dbSets)
            {
                Type dbSetEntityType = dbSetPropInfo.PropertyType.GetGenericArguments().FirstOrDefault();

                if (dbSetEntityType.GetCustomAttribute<MIFTableAttribute>() != null)
                {
                    modelBuilder
                        .Entity(dbSetEntityType)
                        .ToTable($"MIF_{dbSetEntityType.Name}");
                }
                else
                {
                    modelBuilder
                        .Entity(dbSetEntityType)
                        .ToTable(dbSetEntityType.Name);
                }
            }
        }
    }
}
