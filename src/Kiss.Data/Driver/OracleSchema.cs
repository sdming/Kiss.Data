using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Oracle.DataAccess;
using Oracle.DataAccess.Client;
using System.Data.Common;
using Kiss.Data.Schema;

namespace Kiss.Data.Driver
{
    /// <summary>
    /// OracleSchema
    /// </summary>
    public class OracleSchema : SqlSchema
    {
        /// <summary>
        /// ProviderTypeToDbType
        /// </summary>
        /// <param name="providerType"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:丢失范围之前释放对象")]
        public override DbType ProviderTypeToDbType(int providerType)
        {
            var t = (OracleDbType)providerType;
            if (t == OracleDbType.Blob)
            {
                return DbType.Binary;
            }
            OracleParameter p = new OracleParameter();
            p.OracleDbType = t;
            return p.DbType;
        }

        /// <summary>
        /// DeriveParameters
        /// </summary>
        /// <param name="command"></param>
        protected override void DeriveParameters(DbCommand command)
        {
            //can not get scale/precision
            OracleCommandBuilder.DeriveParameters(command as OracleCommand);
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
            OracleParameter p = source as OracleParameter;
            parameter.AllowDBNull = p.IsNullable;
            parameter.DbType = p.DbType;
            parameter.Direction = p.Direction;
            parameter.Name = UnQuoteParameterName(p.ParameterName, '@');
            parameter.Precision = p.Precision;
            parameter.Scale = p.Scale;
            parameter.Size = p.Size;
            parameter.ProviderDbType = (int)p.OracleDbType;
        }

//        public override SqlProcedure GetProcedure(SqlDriver driver, string procedureName, string connectionString)
//        {
//            string fnSql = @"
//select distinct OWNER as catalog, OWNER as schema, OBJECT_NAME as name 
//from all_procedures 
//where OBJECT_NAME = :name and OWNER = (select sys_context('USERENV','SESSION_USER') from dual) ;
//";

//            string parameterSql = @"
//select distinct
//    ARGUMENT_NAME as PARAMETER_NAME, POSITION as ORDINAL_POSITION, 
//    IN_OUT as PARAMETER_MODE, 
//    DATA_TYPE as DATA_TYPE, 
//    CHAR_LENGTH as CHARACTER_MAXIMUM_LENGTH, 
//    DATA_PRECISION as NUMERIC_PRECISION, 
//    DATA_SCALE as NUMERIC_SCALE 
//from 
//    ALL_ARGUMENTS
//where
//    DATA_LEVEL = 0 AND OBJECT_NAME =:name AND OWNER = (select sys_context('USERENV','SESSION_USER') from dual)
//order by 
//    POSITION ;
//";

//            using (var connecton = driver.CreateConnection())
//            {
//                connecton.ConnectionString = connectionString;
//                connecton.Open();

//                var command = connecton.CreateCommand();
//                command.Connection = connecton;
//                command.CommandText = fnSql;
//                command.CommandType = CommandType.Text;
//                var p = command.CreateParameter();
//                p.ParameterName = ":name";
//                p.Value = procedureName;
//                command.Parameters.Add(p);
//                var reader = command.ExecuteReader();
//                if (!reader.HasRows)
//                {
//                    throw new Exception("can not find function:" + procedureName);
//                }

//                SqlProcedure procedure = new SqlProcedure();
//                while (reader.Read())
//                {
//                    procedure.Name = reader["name"].ToString();
//                }

//                command = connecton.CreateCommand();
//                command.Connection = connecton;
//                command.CommandText = parameterSql;
//                p = command.CreateParameter();
//                p.ParameterName = ":name";
//                p.Value = procedure.Name;
//                command.Parameters.Add(p);
//                reader = command.ExecuteReader();

//                while (reader.Read())
//                {
//                    SqlParameter parameter = new SqlParameter();
//                    parameter.Name = reader["PARAMETER_NAME"].ToString();
//                    parameter.Ordinal = Convert.ToInt32(reader["ORDINAL_POSITION"]);
//                    parameter.DataTypeName = reader["DATA_TYPE"].ToString();
//                    parameter.DbType = driver.NativeTypeToDbType(parameter.DataTypeName);
//                    switch (reader["PARAMETER_MODE"].ToString().ToUpperInvariant())
//                    {
//                        case "IN":
//                            parameter.Direction = ParameterDirection.Input;
//                            break;
//                        case "INOUT":
//                            parameter.Direction = ParameterDirection.InputOutput;
//                            break;
//                        case "OUT":
//                            parameter.Direction = ParameterDirection.Output;
//                            break;
//                        default:
//                            parameter.Direction = ParameterDirection.ReturnValue;
//                            break;
//                    }

//                    //type.DbType = Provider.GetDbType(p.NativeType);
//                    if (reader["CHARACTER_MAXIMUM_LENGTH"] != DBNull.Value && !string.IsNullOrEmpty(reader["CHARACTER_MAXIMUM_LENGTH"].ToString()))
//                    {
//                        parameter.Size = Convert.ToInt32(reader["CHARACTER_MAXIMUM_LENGTH"]);
//                    }
//                    if (reader["NUMERIC_PRECISION"] != DBNull.Value && !string.IsNullOrEmpty(reader["NUMERIC_PRECISION"].ToString()))
//                    {
//                        parameter.Precision = Convert.ToByte(reader["NUMERIC_PRECISION"]);
//                    }
//                    if (reader["NUMERIC_SCALE"] != DBNull.Value && !string.IsNullOrEmpty(reader["NUMERIC_SCALE"].ToString()))
//                    {
//                        parameter.Scale = Convert.ToByte(reader["NUMERIC_SCALE"]);
//                    }
//                    procedure.Parameters.Add(parameter);
//                }

//                return procedure;
//            }

//        }

    }
}
