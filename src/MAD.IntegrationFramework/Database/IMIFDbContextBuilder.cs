using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace MAD.IntegrationFramework.Database
{
    public interface IMIFDbContextBuilder
    {
        IMIFDbContextBuilder UseConnectionString(string connectionString);
        IMIFDbContextBuilder UseConnection(DbConnection connection);
        IMIFDbContextBuilder UseDbContextType(Type dbContextType);

        MIFDbContext MIFDbContext { get; }
    }

    public interface IMIFDbContextBuilder <TDbContext> : IMIFDbContextBuilder where TDbContext : MIFDbContext
    {

    }

    
}
