using Dapper;
using MaitlandsInterfaceFramework.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MaitlandsInterfaceFramework.Services.Internals.Database
{
    internal class AutomaticMigrationService
    {
        private readonly ILogger Logger;
        private readonly SqlBuilderService SqlBuilder;

        private readonly List<Type> MigratedContexts;
        private readonly object SyncToken;

        public AutomaticMigrationService(ILogger<AutomaticMigrationService> logger, SqlBuilderService sqlBuilder)
        {
            this.Logger = logger;
            this.SqlBuilder = sqlBuilder;

            this.MigratedContexts = new List<Type>();
            this.SyncToken = new object();
        }
        
        /// <summary>
        /// Creates any tables which are missing in the target SQL database
        /// </summary>
        public void EnsureDatabaseUpToDate(MIFDbContext dbContext)
        {
            lock (this.SyncToken)
            {
                if (this.MigratedContexts.Contains(dbContext.GetType()))
                    return;

                try
                {
                    // Loop through each entity defined as a DbSet in the DbContext's Model
                    foreach (IEntityType entityType in dbContext.Model.GetEntityTypes())
                    {
                        string tableName = entityType.GetTableName();

                        if (String.IsNullOrEmpty(tableName))
                            continue;

                        // Query the tableType from the database. It could potentially be a view.
                        string tableType = dbContext.Connection
                                .Query<string>(
                                    sql: "SELECT TABLE_TYPE FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @TableName",
                                    param: new
                                    {
                                        TableName = tableName
                                    }
                                ).FirstOrDefault();

                        // If tableType comes back as null, the table doesn't exist and must be created.
                        bool tableExists = !String.IsNullOrEmpty(tableType);

                        if (tableExists)
                            continue;

                        this.Logger.LogInformation($"Creating {tableName} table");

                        string createTableQuery = this.SqlBuilder.BuildCreateTableSqlStatementForIEntityType(entityType);

                        // Execute the CREATE Table statement
                        dbContext.Connection.Execute(createTableQuery);
                    }
                }
                finally
                {
                    this.MigratedContexts.Add(dbContext.GetType());
                }
            }
        }
    }
}
