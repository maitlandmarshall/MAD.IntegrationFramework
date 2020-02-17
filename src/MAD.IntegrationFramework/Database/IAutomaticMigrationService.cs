using System;
using System.Collections.Generic;
using System.Text;

namespace MAD.IntegrationFramework.Database
{
    internal interface IAutomaticMigrationService
    {
        void EnsureDatabaseUpToDate(MIFDbContext dbContext);
    }
}
