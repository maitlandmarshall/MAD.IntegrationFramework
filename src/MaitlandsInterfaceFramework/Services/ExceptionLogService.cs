using MaitlandsInterfaceFramework.Database;
using MaitlandsInterfaceFramework.Services.Internals;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace MaitlandsInterfaceFramework.Services
{
    [MIFTable]
    internal class ExceptionLog
    {
        [Key]
        public int Id { get; set; }

        public string Message { get; set; }
        public string Detail { get; set; }

        public DateTime DateTime { get; set; }
    }

    internal class ExceptionDbContext : MIFDbContext
    {
        public DbSet<ExceptionLog> ExceptionLogs { get; set; }
    }

    public static class ExceptionLogService
    {
        public static async Task LogException(this Exception exception)
        {
            LogService.WriteToLog(exception.ToString());

            if (String.IsNullOrEmpty(MIF.Config.SqlConnectionString))
                return;

            using (ExceptionDbContext db = new ExceptionDbContext())
            {
                ExceptionLog log = new ExceptionLog
                {
                    Message = exception.Message,
                    Detail = exception.ToString(),
                    DateTime = DateTime.Now
                };

                db.ExceptionLogs.Add(log);

                await db.SaveChangesAsync();
            }
        }
    }
}
