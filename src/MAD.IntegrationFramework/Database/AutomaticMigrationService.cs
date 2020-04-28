using Dapper;
using MAD.IntegrationFramework.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MAD.IntegrationFramework.Database
{
    internal class AutomaticMigrationService : IAutomaticMigrationService
    {
        private readonly ILogger logger;
        private readonly SqlStatementBuilder sqlBuilder;

        private readonly List<Type> migratedContexts;
        private readonly object syncToken;

        public AutomaticMigrationService(ILogger logger, SqlStatementBuilder sqlBuilder)
        {
            this.logger = logger;
            this.sqlBuilder = sqlBuilder;

            this.migratedContexts = new List<Type>();
            this.syncToken = new object();
        }
        
        /// <summary>
        /// Creates any tables which are missing in the target SQL database
        /// </summary>
        public void EnsureDatabaseUpToDate(MIFDbContext dbContext)
        {
            lock (this.syncToken)
            {
                if (this.migratedContexts.Contains(dbContext.GetType()))
                    return;

                try
                {
                    dbContext.Database.EnsureCreated();

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

                        this.logger.Information("Creating {tableName} table", tableName);

                        string createTableQuery = this.sqlBuilder.BuildCreateTableSqlStatementForIEntityType(entityType);

                        // Execute the CREATE Table statement
                        dbContext.Connection.Execute(createTableQuery);
                    }
                }
                finally
                {
                    this.migratedContexts.Add(dbContext.GetType());
                }
            }
        }
    }
}
