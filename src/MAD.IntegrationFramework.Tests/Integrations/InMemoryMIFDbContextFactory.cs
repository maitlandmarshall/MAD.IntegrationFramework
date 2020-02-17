using MAD.IntegrationFramework.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MAD.IntegrationFramework.UnitTests.Integrations
{
    internal class InMemoryMIFDbContextFactory : IMIFDbContextFactory
    {
        public MIFDbContext Create(Type dbContextType)
        {
            if (!typeof(MIFDbContext).IsAssignableFrom(dbContextType))
                throw new ArgumentException($"Parameter {nameof(dbContextType)} must be a type derived from {nameof(MIFDbContext)}.");

            MIFDbContext dbContext = Activator.CreateInstance(dbContextType) as MIFDbContext;
            dbContext.Configuring += this.DbContext_Configuring;

            try
            {
                dbContext.Database.EnsureCreated();
            }
            finally
            {
                dbContext.Configuring -= this.DbContext_Configuring;
            }

            return dbContext;
        }

        private void DbContext_Configuring(object sender, DbContextOptionsBuilder e)
        {
            e.UseInMemoryDatabase(databaseName: "test");
        }
    }
}
