using MAD.IntegrationFramework.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace MAD.IntegrationFramework.Logging
{
    internal class ExceptionDbLogger : IExceptionLogger
    {
        private readonly IMIFDbContextFactory<ExceptionDbContext> exceptionDbLogger;

        public ExceptionDbLogger(IMIFDbContextFactory<ExceptionDbContext> exceptionDbLogger)
        {
            this.exceptionDbLogger = exceptionDbLogger;
        }

        public async Task LogException(Exception exception, string interfaceName = null)
        {
            using (ExceptionDbContext dbContext = this.exceptionDbLogger.Create())
            {
                dbContext.Add(new ExceptionLog
                {
                    Message = exception.Message,
                    Detail = exception.ToString(),
                    DateTime = DateTime.Now,
                    Interface = interfaceName
                });

                await dbContext.SaveChangesAsync();
            }
        }
    }
}
