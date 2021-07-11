using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MAD.IntegrationFramework.Database
{
    internal class SqlStatementBuilder
    {
        public string BuildCreateTableSqlStatementForIEntityType(IEntityType entityType)
        {
            // Get all the properties from the entity that need to be converted into table columns
            IEnumerable<IProperty> tableProperties = entityType
                .GetProperties()
                // This will ensure the id columns appear closer to the start when doing a select * from [tableName]
                .OrderByDescending(y => y.Name.ToLower().EndsWith("id"));

            // Begin building the CREATE TABLE statement
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.AppendLine($"CREATE TABLE [{entityType.GetTableName()}]");
            queryBuilder.AppendLine("(");

            queryBuilder.AppendLine(
                // Converts a list of IProperty into a comma separated string in a SQL create table format ie:
                // "Id int not null identity (1,1) primary key, ExampleCol nvarchar(max) null"

                String.Join("," + Environment.NewLine, tableProperties.Select(y => IPropertyToSqlColumnString(y)))
            );

            // Finish the CREATE TABLE statement
            queryBuilder.AppendLine(")");

            return queryBuilder.ToString();
        }

        private static string IPropertyToSqlColumnString(IProperty property)
        {
            StringBuilder columnBuilder = new StringBuilder();
            columnBuilder.Append("[");
            columnBuilder.Append(property.Name);
            columnBuilder.Append("] ");

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
                default:

                    if (typeof(byte[]).IsAssignableFrom(propertyType))
                    {
                        columnBuilder.Append("varbinary");

                        int? binaryMaxLength = property.GetMaxLength();

                        if (binaryMaxLength.HasValue)
                        {
                            columnBuilder.Append($"({binaryMaxLength.Value})");
                        }
                        else
                        {
                            columnBuilder.Append("(max)");
                        }

                        break;
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
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
