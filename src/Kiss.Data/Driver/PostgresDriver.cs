using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql;
using System.Data.Common;
using System.Data;

namespace Kiss.Data.Driver
{
    internal class PostgresDriver : SqlDriver
    {
        /// <summary>
        /// ProviderName
        /// </summary>
        public override string ProviderName
        {
            get { return "Npgsql"; }
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
        private readonly static IDialecter dialecter = new PostgresDialecter();

        /// <summary>
        /// Dialecter
        /// </summary>
        public override IDialecter Dialecter
        {
            get { return dialecter; }
        }

        /// <summary>
        /// CreateCompiler
        /// </summary>
        /// <returns></returns>
        public override SqlCompiler CreateCompiler()
        {
            return new PostgresCompiler();
        }

        /// <summary>
        /// GetSchemaProvider
        /// </summary>
        /// <returns></returns>
        protected override SqlSchema CreateSchemaProvider()
        {
            return new PostgresSchema();
        }

        /// <summary>
        /// CreateConnection
        /// </summary>
        /// <returns></returns>
        public override DbConnection CreateConnection()
        {
            return new NpgsqlConnection();
        }

        /// <summary>
        /// CreateCommand
        /// </summary>
        /// <returns></returns>
        public override DbCommand CreateCommand()
        {
            return new NpgsqlCommand();
        }

        /// <summary>
        /// CreateParameter
        /// </summary>
        /// <returns></returns>
        public override DbParameter CreateParameter()
        {
            return new NpgsqlParameter();
        }

        /// <summary>
        /// CreateCommandBuilder
        /// </summary>
        /// <returns></returns>
        public override DbCommandBuilder CreateCommandBuilder()
        {
            return new NpgsqlCommandBuilder();
        }

        /// <summary>
        /// CreateDataAdapter
        /// </summary>
        /// <returns></returns>
        public override DbDataAdapter CreateDataAdapter()
        {
            return new NpgsqlDataAdapter();
        }

        /// <summary>
        /// SetParameterPrecision
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="sqlType"></param>
        protected override void SetParameterPrecision(DbParameter parameter, byte? precision, byte? scale)
        {
            NpgsqlParameter p = (NpgsqlParameter)parameter;
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

        /// <summary>
        /// SetParameterProviderDbType
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="providerDbType"></param>
        protected override void SetParameterProviderDbType(DbParameter parameter, int providerDbType)
        {
            NpgsqlParameter p = (NpgsqlParameter)parameter;
            p.NpgsqlDbType = (NpgsqlTypes.NpgsqlDbType)providerDbType;
        }

        public override System.Data.DbType NativeTypeToDbType(string dataType)
        {
            switch (dataType.ToLowerInvariant())
            {
                case "timestamp":
                    return DbType.Binary;
                default:
                    return base.NativeTypeToDbType(dataType);
            }
            
        }
    }
}
