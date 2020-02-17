using MAD.IntegrationFramework.Database;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MAD.IntegrationFramework.Logging
{
    internal interface IDbContextLogger<TDbContext, TLog> 
        where TDbContext : MIFDbContext where TLog : class
    {
        Task Log(TLog log);
        Task<IDisposable> Step(TLog log, Action<TLog> finish);
    }
}
