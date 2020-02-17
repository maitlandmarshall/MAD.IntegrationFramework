using MAD.IntegrationFramework.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace MAD.IntegrationFramework.Logging
{
    internal class ExceptionDbLogger : IExceptionLogger
    {
        private readonly IDbContextLogger<ExceptionDbContext, ExceptionLog> exceptionDbLogger;

        public ExceptionDbLogger(IDbContextLogger<ExceptionDbContext, ExceptionLog> exceptionDbLogger)
        {
            this.exceptionDbLogger = exceptionDbLogger;
        }

        public async Task LogException(Exception exception, string interfaceName = null)
        {
            await this.exceptionDbLogger.Log(new ExceptionLog
            {
                Message = exception.Message,
                Detail = exception.ToString(),
                DateTime = DateTime.Now,
                Interface = interfaceName
            });
        }
    }
}
