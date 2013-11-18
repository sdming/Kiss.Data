using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Data.Common;

namespace Kiss.Data.Driver
{
    /// <summary>
    /// SqlClientDriver
    /// </summary>
    internal class MsSqlDriver : SqlDriver
    {
        /// <summary>
        /// ProviderName
        /// </summary>
        public override string ProviderName
        {
            get { return "System.Data.SqlClient"; }
        }

        /// <summary>
        /// SupportNamedParameter
        /// </summary>
        public override bool SupportNamedParameter
        {
            get { return true; }
        }

        /// <summary>
        /// mssql IDialecter
        /// </summary>
        private readonly static IDialecter dialecter = new MsSqlDialecter();

        /// <summary>
        /// Dialecter
        /// </summary>
        public override IDialecter Dialecter
        {
            get { return dialecter; }
        }

        /// <summary>
        /// CreateConnection
        /// </summary>
        /// <returns></returns>
        public override DbConnection CreateConnection()
        {
            return new SqlConnection();
        }

        /// <summary>
        /// CreateParameter
        /// </summary>
        /// <returns></returns>
        public override DbParameter CreateParameter()
        {
            return new SqlParameter();
        }

        /// <summary>
        /// CreateCommand
        /// </summary>
        /// <returns></returns>
        public override DbCommand CreateCommand()
        {
            return new SqlCommand();
        }

        /// <summary>
        /// CreateCommandBuilder
        /// </summary>
        /// <returns></returns>
        public override DbCommandBuilder CreateCommandBuilder()
        {
            return new SqlCommandBuilder();
        }

        /// <summary>
        /// CreateDataAdapter
        /// </summary>
        /// <returns></returns>
        public override DbDataAdapter CreateDataAdapter()
        {
            return new SqlDataAdapter();
        }

        /// <summary>
        /// CreateCompiler
        /// </summary>
        /// <returns></returns>
        public override SqlCompiler CreateCompiler()
        {
            return new MsSqlCompiler();
        }

        /// <summary>
        /// GetSchemaProvider
        /// </summary>
        /// <returns></returns>
        protected override SqlSchema CreateSchemaProvider()
        {
            return new MsSqlSchema();
        }

        /// <summary>
        /// SetParameterPrecision
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="sqlType"></param>
        protected override void SetParameterPrecision(DbParameter parameter, byte? precision, byte? scale)
        {
            SqlParameter p = (SqlParameter)parameter;
            if (p.DbType.HasPrecisionAndScale())
            {
                if (scale.HasValue)
                {
                    p.Scale = scale.Value;
                }
                else
                {
                    p.Scale = byte.MaxValue;
                }
                if (precision.HasValue)
                {
                    p.Precision = precision.Value;
                }
                else
                {
                    p.Precision = byte.MaxValue;
                }
            }
        }
    }
}
