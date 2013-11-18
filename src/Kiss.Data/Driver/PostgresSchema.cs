using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Kiss.Data.Schema;
using Npgsql;
using NpgsqlTypes;
using System.Data.Common;

namespace Kiss.Data.Driver
{
    /// <summary>
    /// PostgresSchema
    /// </summary>
    public class PostgresSchema : SqlSchema
    {
        /// <summary>
        /// ProviderTypeToDbType
        /// </summary>
        /// <param name="providerType"></param>
        /// <returns></returns>
        public override DbType ProviderTypeToDbType(int providerType)
        {
            NpgsqlParameter p = new NpgsqlParameter("p", (NpgsqlDbType)providerType);
            return p.DbType;
        }

        /// <summary>
        /// GetSchemaTable
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        protected override DataTable GetSchemaTable(DbDataReader reader)
        {
            NpgsqlDataReader r = reader as NpgsqlDataReader;
            if (r == null)
            {
                throw new Exception("do not support type:" + reader.GetType().FullName);
            }

            DataTable table = r.GetSchemaTable();
            table.Columns["ProviderType"].ColumnName = "ProviderTypeName";
            table.Columns.Add(ColumnNameDbType, typeof(int));
            table.Columns.Add(ColumnNameProviderType, typeof(int));
            for (var i = 0; i < table.Rows.Count; i++)
            {
                DataRow row = table.Rows[i];
                string name = Convert.ToString(row["ColumnName"]);
                int ordinal = reader.GetOrdinal(name);
                row[ColumnNameDbType] = (int)r.GetFieldDbType(ordinal);
                row[ColumnNameProviderType] = (int)r.GetFieldNpgsqlDbType(ordinal);
            }            
            return table;

        }

    }
}
