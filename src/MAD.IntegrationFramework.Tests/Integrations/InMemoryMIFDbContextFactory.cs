using MAD.IntegrationFramework.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace MAD.IntegrationFramework.Tests.Integrations
{
    internal class InMemoryMIFDbContextFactory : IMIFDbContextFactory
    {
        public MIFDbContext Create(Type dbContextType, DbConnection dbConnection = null)
        {
            if (!typeof(MIFDbContext).IsAssignableFrom(dbContextType))
                throw new ArgumentException($"Parameter {nameof(dbContextType)} must be a type derived from {nameof(MIFDbContext)}.");

            MIFDbContext dbContext = Activator.CreateInstance(dbContextType) as MIFDbContext;
            dbContext.Database.EnsureCreated();

            return dbContext;
        }

        private void DbContext_Configuring(object sender, DbContextOptionsBuilder e)
        {
            e.UseInMemoryDatabase(databaseName: "test");
        }
    }

    internal class InMemoryMIFDbContextFactory<TDbContext> : InMemoryMIFDbContextFactory, IMIFDbContextFactory<TDbContext>
        where TDbContext : MIFDbContext
    {
        public TDbContext Create(DbConnection dbConnection = null)
        {
            return this.Create(typeof(TDbContext), dbConnection) as TDbContext;
        }
    }
}
