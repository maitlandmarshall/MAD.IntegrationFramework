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
        internal event EventHandler<DbContextOptionsBuilder> Configuring;

        public DbConnection Connection => this.Database.GetDbConnection();

        public MIFDbContext() : base() { }
        public MIFDbContext(DbContextOptions options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            this.Configuring?.Invoke(this, optionsBuilder);
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
