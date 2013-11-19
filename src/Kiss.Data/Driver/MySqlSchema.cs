using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MySql.Data.MySqlClient;
using System.Data.Common;

namespace Kiss.Data.Driver
{
    /// <summary>
    /// MySqlSchema
    /// </summary>
    public class MySqlSchema : SqlSchema
    {
        /// <summary>
        /// ProviderTypeToDbType
        /// </summary>
        /// <param name="providerType"></param>
        /// <returns></returns>
        public override DbType ProviderTypeToDbType(int providerType)
        {
            var t = (MySqlDbType)providerType;
            if(t == MySqlDbType.Binary || t == MySqlDbType.VarBinary)
            {
                return DbType.Binary;
            }
            
            MySqlParameter p = new MySqlParameter();
            p.MySqlDbType = (MySqlDbType)providerType;
            return p.DbType;
        }

        /// <summary>
        /// DeriveParameters
        /// </summary>
        /// <param name="command"></param>
        protected override void DeriveParameters(DbCommand command)
        {
            MySqlCommandBuilder.DeriveParameters(command as MySqlCommand);
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
            MySqlParameter p = source as MySqlParameter;
            parameter.AllowDBNull = p.IsNullable;
            parameter.DbType = p.DbType;
            parameter.Direction = p.Direction;
            parameter.Name = UnQuoteParameterName(p.ParameterName, '@');
            parameter.Precision = p.Precision;
            parameter.Scale = p.Scale;
            parameter.Size = p.Size;
            parameter.ProviderDbType = (int)p.MySqlDbType;
        }
    }
}
