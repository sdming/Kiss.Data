using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using MySql.Data.MySqlClient;
using Kiss.Data.Schema;

namespace Kiss.Data.Driver
{
    /// <summary>
    /// MySqlDriver
    /// </summary>
    public class MySqlDriver : SqlDriver
    {
        /// <summary>
        /// ProviderName
        /// </summary>
        public override string ProviderName
        {
            get { return "MySql.Data.MySqlClient"; }
        }

        /// <summary>
        /// SupportNamedParameter
        /// </summary>
        public override bool SupportNamedParameter
        {
            get { return true; }
        }

        /// <summary>
        /// mysql IDialecter
        /// </summary>
        private readonly static IDialecter dialecter = new MySqlDialecter();

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
            return new MySqlConnection();
        }

        /// <summary>
        /// CreateCommand
        /// </summary>
        /// <returns></returns>
        public override DbCommand CreateCommand()
        {
            return new MySqlCommand();
        }

        /// <summary>
        /// CreateCommandBuilder
        /// </summary>
        /// <returns></returns>
        public override DbCommandBuilder CreateCommandBuilder()
        {
            return new MySqlCommandBuilder();
        }

        /// <summary>
        /// CreateDataAdapter
        /// </summary>
        /// <returns></returns>
        public override DbDataAdapter CreateDataAdapter()
        {
            return new MySqlDataAdapter();
        }

        /// <summary>
        /// CreateParameter
        /// </summary>
        /// <returns></returns>
        public override DbParameter CreateParameter()
        {
            return new MySqlParameter();
        }

        /// <summary>
        /// CreateCompiler
        /// </summary>
        /// <returns></returns>
        public override SqlCompiler CreateCompiler()
        {
            return new MySqlCompiler();
        }

        /// <summary>
        /// GetSchemaProvider
        /// </summary>
        /// <returns></returns>
        protected override SqlSchema CreateSchemaProvider()
        {
            return new MySqlSchema();
        }

        /// <summary>
        /// SetParameterProviderDbType
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="providerDbType"></param>
        public override void SetParameterProviderDbType(DbParameter parameter, int providerDbType)
        {
            MySqlParameter p = (MySqlParameter)parameter;
            p.MySqlDbType = (MySqlDbType)providerDbType;
        }

        /// <summary>
        /// SetParameterPrecision
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="sqlType"></param>
        protected override void SetParameterPrecision(DbParameter parameter, byte? precision, byte? scale)
        {
            MySqlParameter p = (MySqlParameter)parameter;

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
