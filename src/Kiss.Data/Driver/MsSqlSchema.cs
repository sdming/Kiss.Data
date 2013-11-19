using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;


namespace Kiss.Data.Driver
{
    /// <summary>
    /// MsSqlSchema
    /// </summary>
    public class MsSqlSchema : SqlSchema
    {
        /// <summary>
        /// ProviderTypeToDbType
        /// </summary>
        /// <param name="providerType"></param>
        /// <returns></returns>
        public override DbType ProviderTypeToDbType(int providerType)
        {
            System.Data.SqlClient.SqlParameter p = new System.Data.SqlClient.SqlParameter();
            p.SqlDbType = (SqlDbType)providerType;
            return p.DbType;
        }

        /// <summary>
        /// DeriveParameters
        /// </summary>
        /// <param name="command"></param>
        protected override void DeriveParameters(DbCommand command)
        {
            SqlCommandBuilder.DeriveParameters(command as SqlCommand);
        }

        protected override Kiss.Data.Schema.SqlProcedure BuildProcedure(DbCommand command)
        {
            Kiss.Data.Schema.SqlProcedure procedure = new Kiss.Data.Schema.SqlProcedure();
            procedure.Name = command.CommandText;

            foreach (DbParameter p in command.Parameters)
            {
                Kiss.Data.Schema.SqlParameter parameter = new Kiss.Data.Schema.SqlParameter();
                if (p.Direction == ParameterDirection.ReturnValue)
                {
                    continue;
                }
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
            SqlParameter p = source as SqlParameter;
            parameter.AllowDBNull = p.IsNullable;
            parameter.DataTypeName = p.TypeName;
            parameter.DbType = p.DbType;
            parameter.Direction = p.Direction;
            parameter.Name = UnQuoteParameterName(p.ParameterName, '@');
            parameter.Precision = p.Precision;
            parameter.Scale = p.Scale;
            parameter.Size = p.Size;
            parameter.ProviderDbType = (int)p.SqlDbType;
        }
    }
}
