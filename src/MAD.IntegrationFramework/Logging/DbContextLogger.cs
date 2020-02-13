using MAD.IntegrationFramework.Database;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MAD.IntegrationFramework.Logging
{
    internal class DbContextLogger<TDbContext, TLog> where TDbContext : MIFDbContext where TLog : class
    {
        internal class DbContextLoggerSaveDisposable : IDisposable
        {
            private readonly Task saveTask;

            public DbContextLoggerSaveDisposable(Task saveTask)
            {
                this.saveTask = saveTask;
            }

            public void Dispose()
            {
                this.saveTask.Wait();
            }

            public ValueTask DisposeAsync()
            {
                return new ValueTask(this.saveTask);
            }
        }

        private readonly IMIFDbContextFactory<TDbContext> dbContextFactory;

        public DbContextLogger(IMIFDbContextFactory<TDbContext> dbContextFactory)
        {
            this.dbContextFactory = dbContextFactory;
        }

        public async Task Log(TLog log)
        {
            using (TDbContext dbContext = this.dbContextFactory.Create())
            {
                await this.AddToDbSetAndSave(log, dbContext);
            }
        }

        public async Task<IDisposable> Step(TLog log, Action<TLog> finish)
        {
            TDbContext dbContext = this.dbContextFactory.Create();
            await this.AddToDbSetAndSave(log, dbContext);

            return new DbContextLoggerSaveDisposable(Task.Run(async () =>
            {
                finish();

                await dbContext.SaveChangesAsync();
                await dbContext.DisposeAsync();
            }));
        }

        private async Task AddToDbSetAndSave(TLog log, TDbContext dbContext)
        {
            dbContext.Set<TLog>().Add(log);
            await dbContext.SaveChangesAsync();
        }
    }
}
