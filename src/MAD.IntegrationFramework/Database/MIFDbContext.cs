using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;

namespace MAD.IntegrationFramework.Database
{
    public abstract class MIFDbContext : DbContext
    {
        public DbConnection Connection => this.Database.GetDbConnection();

        public MIFDbContext() : this(MIF.Config.SqlConnectionString) { }

        public MIFDbContext(string connectionString) : base(new DbContextOptionsBuilder().UseSqlServer(connectionString).Options)
        {
            this.Database.SetCommandTimeout(TimeSpan.FromMinutes(10));
            this.Database.EnsureCreated();
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
