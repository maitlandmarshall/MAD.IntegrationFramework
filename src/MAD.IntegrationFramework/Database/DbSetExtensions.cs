using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MAD.IntegrationFramework.Database
{
    public static class DbSetExtensions
    {
        public async static Task<T> FirstOrDefaultAsyncLocalThenRemote<T>(this DbSet<T> dbSet, Expression<Func<T, bool>> predicate) where T : class
        {
            return dbSet.Local.FirstOrDefault(predicate.Compile()) ?? await dbSet.FirstOrDefaultAsync(predicate);
        }
    }
}
