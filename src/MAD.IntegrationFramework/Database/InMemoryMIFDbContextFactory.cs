using MAD.IntegrationFramework.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace MAD.IntegrationFramework.Database
{
    internal class InMemoryMIFDbContextFactory : IMIFDbContextFactory
    {
        public MIFDbContext Create(Type dbContextType, DbConnection dbConnection = null)
        {
            if (!typeof(MIFDbContext).IsAssignableFrom(dbContextType))
                throw new ArgumentException($"Parameter {nameof(dbContextType)} must be a type derived from {nameof(MIFDbContext)}.");

            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseInMemoryDatabase(dbContextType.Name);

            MIFDbContext dbContext = Activator.CreateInstance(
                dbContextType,
                new object[] { optionsBuilder.Options }
            ) as MIFDbContext;

            dbContext.Database.EnsureCreated();

            return dbContext;
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
