using MAD.IntegrationFramework.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace MAD.IntegrationFramework.Database
{
    internal interface IMIFDbContextFactory <DbContext> where DbContext : MIFDbContext
    {
        DbContext Create();
    }
}
