using MAD.IntegrationFramework.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace MAD.IntegrationFramework.Logging
{
    [MIFTable]
    internal class ExceptionLog
    {
        [Key]
        public int Id { get; set; }

        public string Message { get; set; }
        public string Detail { get; set; }

        public string Interface { get; set; }

        public DateTime DateTime { get; set; }
    }

    internal class ExceptionDbContext : MIFDbContext
    {
        public DbSet<ExceptionLog> ExceptionLogs { get; set; }
    }

    internal class ExceptionDbLogger : IExceptionLogger
    {
        private readonly IMIFDbContextFactory<ExceptionDbContext> exceptionDbContextFactory;

        public ExceptionDbLogger(IMIFDbContextFactory<ExceptionDbContext> exceptionDbContextFactory)
        {
            this.exceptionDbContextFactory = exceptionDbContextFactory;
        }

        public async Task LogException(Exception exception, string interfaceName = null)
        {
            if (String.IsNullOrEmpty(MIF.Config.SqlConnectionString))
                return;

            using (ExceptionDbContext dbContext = this.exceptionDbContextFactory.Create())
            {
                ExceptionLog log = new ExceptionLog
                {
                    Message = exception.Message,
                    Detail = exception.ToString(),
                    DateTime = DateTime.Now,
                    Interface = interfaceName
                };

                dbContext.ExceptionLogs.Add(log);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
