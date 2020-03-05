using System;
using System.Collections.Generic;
using System.Text;

namespace MAD.IntegrationFramework.Database
{
    public interface IAutomaticMigrationService
    {
        void EnsureDatabaseUpToDate(MIFDbContext dbContext);
    }
}
