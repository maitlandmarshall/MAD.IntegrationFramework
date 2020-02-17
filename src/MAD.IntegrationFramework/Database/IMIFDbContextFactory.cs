using MAD.IntegrationFramework.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace MAD.IntegrationFramework.Database
{
    internal interface IMIFDbContextFactory
    {
        MIFDbContext Create(Type dbContextType);
    }

    internal interface IMIFDbContextFactory <TDbContext> 
        where TDbContext : MIFDbContext
    {
        TDbContext Create();
    }
}
