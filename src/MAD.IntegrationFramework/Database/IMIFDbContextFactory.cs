using MAD.IntegrationFramework.Database;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace MAD.IntegrationFramework.Database
{
    public interface IMIFDbContextFactory
    {
        MIFDbContext Create(Type dbContextType, DbConnection dbConnection = null);
    }

    public interface IMIFDbContextFactory <TDbContext> 
        where TDbContext : MIFDbContext
    {
        TDbContext Create(DbConnection dbConnection = null);
    }
}
