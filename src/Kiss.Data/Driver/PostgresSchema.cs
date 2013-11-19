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

        /// <summary>
        /// DeriveParameters
        /// </summary>
        /// <param name="command"></param>
        protected override void DeriveParameters(DbCommand command)
        {
            //don't support out parameter
            NpgsqlCommandBuilder.DeriveParameters(command as NpgsqlCommand);
        }

        protected override Kiss.Data.Schema.SqlProcedure BuildProcedure(DbCommand command)
        {
            Kiss.Data.Schema.SqlProcedure procedure = new Kiss.Data.Schema.SqlProcedure();
            procedure.Name = command.CommandText;

            foreach (DbParameter p in command.Parameters)
            {
                Kiss.Data.Schema.SqlParameter parameter = new Kiss.Data.Schema.SqlParameter();
                BuildParameter(parameter, p);
                procedure.Parameters.Add(parameter);
            }
            return procedure;
        }

        /// <summary>
        /// build sqlparameter form DbParameter
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="source"></param>
        protected override void BuildParameter(Kiss.Data.Schema.SqlParameter parameter, DbParameter source)
        {
            NpgsqlParameter p = source as NpgsqlParameter;
            parameter.AllowDBNull = p.IsNullable;
            parameter.DbType = p.DbType;
            parameter.Direction = p.Direction;
            parameter.Name = UnQuoteParameterName(p.ParameterName, ':');
            parameter.Precision = p.Precision;
            parameter.Scale = p.Scale;
            parameter.Size = p.Size;
            parameter.ProviderDbType = (int)p.NpgsqlDbType;
        }

        public override SqlProcedure GetProcedure(SqlDriver driver, string procedureName, string connectionString)
        {
            string fnSql = @"
select routine_catalog as catalog, routine_schema as schema, routine_name as name 
from information_schema.routines 
where routine_name = @name and routine_schema = current_schema() ;
";
            string parameterSql = @"
select 
        p.parameter_name, p.ordinal_position, p.parameter_mode, p.udt_name as data_type, p.character_maximum_length, p.numeric_precision, p.numeric_scale 
from 
        information_schema.parameters p,
        information_schema.routines r
where
        p.specific_catalog = r.specific_catalog and p.specific_schema = r.specific_schema and p.specific_name = r.specific_name
        and r.routine_name = @name and r.routine_schema = current_schema()
order by 
        ordinal_position ;
";

            using(var connecton = driver.CreateConnection())
            {
                connecton.ConnectionString = connectionString;
                connecton.Open();
                
                var command = connecton.CreateCommand();
                command.Connection = connecton;
                command.CommandText = fnSql;
                command.CommandType = CommandType.Text;
                var p = command.CreateParameter();
                p.ParameterName = "@name";
                p.Value = procedureName;
                command.Parameters.Add(p);
                var reader = command.ExecuteReader();
                if (!reader.HasRows)
                {
                    throw new Exception("can not find function:" + procedureName);
                }

                SqlProcedure procedure = new SqlProcedure();
                while (reader.Read())
                {
                    procedure.Name = reader["name"].ToString();
                }

                command = connecton.CreateCommand();
                command.Connection = connecton;
                command.CommandText = parameterSql;
                p = command.CreateParameter();
                p.ParameterName = "@name";
                p.Value = procedure.Name;
                command.Parameters.Add(p);
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    SqlParameter parameter = new SqlParameter();
                    parameter.Name = reader["PARAMETER_NAME"].ToString();
                    parameter.Ordinal = Convert.ToInt32(reader["ordinal_position"]);
                    parameter.DataTypeName = reader["data_type"].ToString();
                    parameter.DbType = driver.NativeTypeToDbType(parameter.DataTypeName);
                    switch (reader["PARAMETER_MODE"].ToString().ToUpperInvariant())
                    {
                        case "IN":
                            parameter.Direction = ParameterDirection.Input;
                            break;
                        case "INOUT":
                            parameter.Direction = ParameterDirection.InputOutput;
                            break;
                        case "OUT":
                            parameter.Direction = ParameterDirection.Output;
                            break;
                        default:
                            parameter.Direction = ParameterDirection.ReturnValue;
                            break;
                    }

                    //type.DbType = Provider.GetDbType(p.NativeType);
                    if (reader["CHARACTER_MAXIMUM_LENGTH"] != DBNull.Value && !string.IsNullOrEmpty(reader["CHARACTER_MAXIMUM_LENGTH"].ToString()))
                    {
                        parameter.Size = Convert.ToInt32(reader["CHARACTER_MAXIMUM_LENGTH"]);
                    }
                    if (reader["NUMERIC_PRECISION"] != DBNull.Value && !string.IsNullOrEmpty(reader["NUMERIC_PRECISION"].ToString()))
                    {
                        parameter.Precision = Convert.ToByte(reader["NUMERIC_PRECISION"]);
                    }
                    if (reader["NUMERIC_SCALE"] != DBNull.Value && !string.IsNullOrEmpty(reader["NUMERIC_SCALE"].ToString()))
                    {
                        parameter.Scale = Convert.ToByte(reader["NUMERIC_SCALE"]);
                    }
                    procedure.Parameters.Add(parameter);
                }

                return procedure;
            }

        }

    }
}
