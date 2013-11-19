using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;
using Kiss.Data.Schema;

namespace Kiss.Data.Driver
{
    /// <summary>
    /// SqlSchema
    /// </summary>
    public class SqlSchema
    {
        public const string ColumnNameDbType = "KissDbType";
        public const string ColumnNameProviderType = "KissProviderDbType";

        /// <summary>
        /// ProviderTypeToDbType
        /// </summary>
        /// <param name="providerType"></param>
        /// <returns></returns>
        public virtual DbType ProviderTypeToDbType(int providerType)
        {
            return (DbType)providerType;
        }

        ///// <summary>
        ///// SetDbType
        ///// </summary>
        ///// <param name="col"></param>
        //public virtual void SetDbType(SqlColumn col, DataRow row)
        //{
        //    if (row.Table.Columns.Contains(ColumnNameDbType))
        //    {
        //        col.DbType = (DbType)Convert.ToInt32(row[ColumnNameDbType]);
        //        return;
        //    }

        //    try
        //    {
        //        col.DbType = ProviderTypeToDbType(col.ProviderDbType);
        //    }
        //    catch
        //    {
        //        col.DbType = RuntimeTypeToDbType(col.RuntimeType);
        //    }
        //}

        /// <summary>
        /// SetProviderDbType
        /// </summary>
        /// <param name="col"></param>
        public virtual void SetProviderDbType(SqlColumn col, DataRow row)
        {
            if (row.Table.Columns.Contains(ColumnNameProviderType))
            {
                col.ProviderDbType = Convert.ToInt32(row[ColumnNameProviderType]);
                return;
            }
            col.ProviderDbType = Convert.ToInt32(row["ProviderType"]);    
        }

        /// <summary>
        /// RuntimeTypeToDbType
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual DbType RuntimeTypeToDbType(Type type)
        {
            return DbTypeEntension.GetDbType(type);
        }

        /// <summary>
        /// SetColumn
        /// </summary>
        /// <param name="col"></param>
        protected virtual void SetColumn(SqlColumn col)
        {
            
        }

        /// <summary>
        /// GetSchemaTable
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        protected virtual DataTable GetSchemaTable(DbDataReader reader)
        {
            return reader.GetSchemaTable();
        }

        /// <summary>
        /// unquote parameter name, remove prefix @, :, ?
        /// </summary>
        /// <param name="s"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        protected virtual string UnQuoteParameterName(string s, char prefix)
        {
            if (string.IsNullOrEmpty(s))
            {
                return s;
            }
            if (s[0] == prefix)
            {
                return s.Substring(1);
            }
            return s;
        }

        /// <summary>
        /// DeriveParameters
        /// </summary>
        /// <param name="command"></param>
        protected virtual void DeriveParameters(DbCommand command)
        {
            throw new Exception("does not support derive parameters");
        }

        /// <summary>
        /// EnableDeriveParameters
        /// </summary>
        protected virtual bool EnableDeriveParameters
        {
            get { return true; }
        }

        /// <summary>
        /// build sqlparameter form DbParameter
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="source"></param>
        protected virtual void BuildParameter(SqlParameter parameter, DbParameter source)
        {
            parameter.Name = source.ParameterName;
            parameter.DbType = source.DbType;
            parameter.Direction = source.Direction;
        }

        protected virtual SqlProcedure BuildProcedure(DbCommand command)
        {
            SqlProcedure procedure = new SqlProcedure();
            procedure.Name = command.CommandText;

            foreach (DbParameter p in command.Parameters)
            {
                SqlParameter parameter = new SqlParameter();
                BuildParameter(parameter, p);
                procedure.Parameters.Add(parameter);
            }
            return procedure;
        }

        /// <summary>
        /// get parameter schema by DeriveParameters
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="procedureName"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:检查 SQL 查询是否存在安全漏洞")]
        protected virtual SqlProcedure GetProcedureByDerive(SqlDriver driver, string procedureName, string connectionString)
        {
            DbCommand command = null;

            using (var connection = driver.CreateConnection())
            {
                connection.ConnectionString = connectionString;
                connection.Open();

                command = connection.CreateCommand();
                command.CommandText = procedureName;
                command.CommandType = CommandType.StoredProcedure;
                command.Connection = connection;
                DeriveParameters(command);
            }

            SqlProcedure procedure = BuildProcedure(command);            
            return procedure;
        }

        /// <summary>
        /// get parameter shcema from GetSchema
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="procedureName"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        protected virtual SqlProcedure GetProcedureBySchema(SqlDriver driver, string procedureName, string connectionString)
        {
            DataTable parameters = null;
            using (DbConnection connection = driver.CreateConnection())
            {
                connection.ConnectionString = connectionString;
                connection.Open();

                string[] restriction = new string[] { null, procedureName};
                parameters = connection.GetSchema("ProcedureParameters", restriction);
            }

            SqlProcedure procedure = new SqlProcedure();
            procedure.Name = procedureName;
            var builder = driver.CreateCommandBuilder();
           
            foreach (DataRow row in parameters.Rows)
            {
                SqlParameter parameter =  new SqlParameter();
                //parameter.Name = UnQuote(row["PARAMETER_NAME"].ToString());
                parameter.Name = row["PARAMETER_NAME"].ToString();
                parameter.DataTypeName = row["DATA_TYPE"].ToString();
                switch (row["PARAMETER_MODE"].ToString().ToUpperInvariant())
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
                if (row["CHARACTER_MAXIMUM_LENGTH"] != DBNull.Value && !string.IsNullOrEmpty(row["CHARACTER_MAXIMUM_LENGTH"].ToString()))
                {
                    parameter.Size = Convert.ToInt32(row["CHARACTER_MAXIMUM_LENGTH"]);
                }
                if (row["NUMERIC_PRECISION"] != DBNull.Value && !string.IsNullOrEmpty(row["NUMERIC_PRECISION"].ToString()))
                {
                    parameter.Precision = Convert.ToByte(row["NUMERIC_PRECISION"]);
                }
                if (row["NUMERIC_SCALE"] != DBNull.Value && !string.IsNullOrEmpty(row["NUMERIC_SCALE"].ToString()))
                {
                    parameter.Scale = Convert.ToByte(row["NUMERIC_SCALE"]);
                }
                
                procedure.Parameters.Add(parameter);
            }

            return procedure;

        }

        public virtual SqlProcedure GetProcedure(SqlDriver driver, string procedureName, string connectionString)
        {                        
            if (EnableDeriveParameters)
            {
                return GetProcedureByDerive(driver, procedureName, connectionString);
            }
            return GetProcedureBySchema(driver, procedureName, connectionString);

        }


        /// <summary>
        /// get table schame
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="tableName"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:检查 SQL 查询是否存在安全漏洞")]
        public virtual SqlTable GetTable(SqlDriver driver, string tableName, string connectionString)
        {
            using (var cn = driver.CreateConnection())
            {
                cn.ConnectionString = connectionString;
                cn.Open();
                var cmd = driver.CreateCommand();
                cmd.Connection = cn;
                cmd.CommandText = string.Format("select * from {0} where 1 = 0", driver.Dialecter.QuoteIdentifer(tableName));
                cmd.CommandType = CommandType.Text;
                //cmd.CommandText = tableName;
                //cmd.CommandType = CommandType.TableDirect;
                var reader = cmd.ExecuteReader(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo);
                var table = GetSchemaTable(reader);
                
                //reader.Close();
                //var adapter = driver.CreateDataAdapter();
                //adapter.SelectCommand = cmd;
                //adapter.SelectCommand.Connection = cn;
                //adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                //DataTable adapterTable = new DataTable();
                //adapter.FillSchema(adapterTable, SchemaType.Mapped);
                //DbCommandBuilder builder = driver.CreateCommandBuilder();
                //builder.DataAdapter = adapter;
                //var insertcmd = builder.GetInsertCommand();
                //var updatecmd = builder.GetUpdateCommand();

                SqlTable t = new SqlTable();
                t.Name = tableName;
               
                for (var i = 0; i < table.Rows.Count; i++)
                {
                    DataRow row = table.Rows[i];
                    SqlColumn col = new SqlColumn();
                    string name = Convert.ToString(row["ColumnName"]);
                    col.Name = name;
                    col.Ordinal = Convert.ToInt32(row["ColumnOrdinal"]);
                    if (table.Columns.Contains("IsAutoIncrement"))
                    {
                        col.IsAutoIncrement = Convert.ToBoolean(row["IsAutoIncrement"]); 
                    }                    
                    col.AllowDBNull = Convert.ToBoolean(row["AllowDBNull"]);
                    col.IsKey = Convert.ToBoolean(row["IsKey"]);
                    col.IsReadOnly = Convert.ToBoolean(row["IsReadOnly"]);
                    //col.RuntimeType = reader.GetFieldType(reader.GetOrdinal(name));
                    col.DataTypeName = reader.GetDataTypeName(reader.GetOrdinal(name));
                    //col.ProviderType = Convert.ToInt32(row["ProviderType"]);                     
                    SetProviderDbType(col, row);
                    //SetDbType(col, row);
                    if (row.Table.Columns.Contains(ColumnNameDbType))
                    {
                        col.DbType = (DbType)Convert.ToInt32(row[ColumnNameDbType]);
                    }
                    else
                    {
                        try
                        {
                            if (col.ProviderDbType.HasValue)
                            {
                                col.DbType = ProviderTypeToDbType(col.ProviderDbType.Value);
                            }
                            else
                            {
                                col.DbType = RuntimeTypeToDbType(reader.GetFieldType(reader.GetOrdinal(name)));
                            }
                        }
                        catch
                        {
                            col.DbType = RuntimeTypeToDbType(reader.GetFieldType(reader.GetOrdinal(name)));
                        }
                    }

                    if (col.DbType.HasPrecisionAndScale())
                    {
                        if (!row.IsNull("NumericPrecision"))
                        {
                            col.Precision = Convert.ToByte(row["NumericPrecision"]);
                        }
                        if (!row.IsNull("NumericScale"))
                        {
                            col.Scale = Convert.ToByte(row["NumericScale"]);
                        }
                    }
                    if (col.DbType.HasSize())
                    {
                        col.Size = Convert.ToInt32(row["ColumnSize"]);
                    }
                    //SetColumn(col);
                    t.Columns.Add(col);
                }

                return t;

             }


        }
    }
}
