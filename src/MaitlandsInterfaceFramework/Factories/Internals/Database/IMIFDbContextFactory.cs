using MaitlandsInterfaceFramework.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaitlandsInterfaceFramework.Factories.Internals.Database
{
    internal interface IMIFDbContextFactory <DbContext> where DbContext : MIFDbContext
    {
        DbContext Create();
    }
}
