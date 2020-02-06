using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Linq;
using System.Text;
using static MaitlandsInterfaceFramework.Services.Internals.LogService;

namespace MaitlandsInterfaceFramework.Database.Internals
{
    internal static class AutomaticMigration
    {
        /// <summary>
        /// Creates any tables which are missing in the target SQL database
        /// </summary>
        public static void EnsureDatabaseUpToDate(MIFDbContext dbContext)
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

                WriteToLog($"Creating {tableName} table");

                // Begin building the CREATE TABLE statement
                StringBuilder queryBuilder = new StringBuilder();
                queryBuilder.AppendLine($"CREATE TABLE {tableName}");
                queryBuilder.AppendLine("(");

                queryBuilder.AppendLine(
                    String.Join("," + Environment.NewLine,
                        entityType
                        .GetProperties()
                        .OrderByDescending(y => y.Name.ToLower().EndsWith("id"))
                        .Select(y => IPropertyToSqlColumnString(y))
                    )
                );

                queryBuilder.AppendLine(")");

                string createTableQuery = queryBuilder.ToString();

                dbContext.Connection.Execute(createTableQuery);
            }
        }

        private static string IPropertyToSqlColumnString(IProperty property)
        {
            StringBuilder columnBuilder = new StringBuilder();
            columnBuilder.Append($"[{property.Name}]");
            columnBuilder.Append(" ");

            Type propertyType = property.PropertyInfo.PropertyType;

            bool IsNullable(Type type) => Nullable.GetUnderlyingType(type) != null || type == typeof(string);

            if (IsNullable(propertyType) && typeof(string) != propertyType)
            {
                propertyType = Nullable.GetUnderlyingType(propertyType);
            }

            TypeCode propertyTypeCode = Type.GetTypeCode(propertyType);

            switch (propertyTypeCode)
            {
                case TypeCode.Boolean:
                    columnBuilder.Append("bit");
                    break;
                case TypeCode.Char:
                    columnBuilder.Append("nchar");
                    break;
                case TypeCode.SByte:
                case TypeCode.Int16:
                    columnBuilder.Append("smallint");
                    break;
                case TypeCode.Byte:
                    columnBuilder.Append("tinyint");
                    break;
                case TypeCode.UInt16:
                case TypeCode.Int32:
                    columnBuilder.Append("int");
                    break;

                case TypeCode.Int64:
                case TypeCode.UInt32:
                    columnBuilder.Append("bigint");
                    break;

                case TypeCode.UInt64:
                    columnBuilder.Append("decimal(20,0)");
                    break;
                case TypeCode.Single:
                    columnBuilder.Append("real");
                    break;
                case TypeCode.Double:
                    columnBuilder.Append("float");
                    break;
                case TypeCode.Decimal:
                    columnBuilder.Append("decimal");
                    break;
                case TypeCode.DateTime:
                    columnBuilder.Append("datetime");
                    break;
                case TypeCode.String:
                    columnBuilder.Append("nvarchar");

                    int? maxLength = property.GetMaxLength();

                    if (maxLength.HasValue)
                    {
                        columnBuilder.Append($"({maxLength.Value})");
                    }
                    else
                    {
                        columnBuilder.Append("(max)");
                    }

                    break;
                case TypeCode.DBNull:
                case TypeCode.Object:
                case TypeCode.Empty:
                default:
                    throw new NotImplementedException();
            }

            if (property.IsNullable)
            {
                columnBuilder.Append(" NULL");
            }
            else
            {
                columnBuilder.Append(" NOT NULL");

                if (propertyTypeCode == TypeCode.Boolean)
                    columnBuilder.Append(" DEFAULT 0");
            }

            if (property.IsPrimaryKey())
            {
                if (propertyTypeCode == TypeCode.Int32 || propertyTypeCode == TypeCode.UInt16)
                {
                    columnBuilder.Append(" IDENTITY(1,1)");
                }

                columnBuilder.Append(" PRIMARY KEY");
            }

            return columnBuilder.ToString();
        }
    }
}
