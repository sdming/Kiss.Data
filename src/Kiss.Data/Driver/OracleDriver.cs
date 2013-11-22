using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;
using Oracle.DataAccess.Client;
using Kiss.Data.Expression;

namespace Kiss.Data.Driver
{
    /// <summary>
    /// OracleDriver
    /// </summary>
    public class OracleDriver : SqlDriver
    {
        /// <summary>
        /// ProviderName
        /// </summary>
        public override string ProviderName 
        { 
            get 
            { 
                return "Oracle.DataAccess.Client"; 
            } 
        }

        /// <summary>
        /// SupportNamedParameter
        /// </summary>
        public override bool SupportNamedParameter
        {
            get { return true; }
        }

        /// <summary>
        /// oracle IDialecter
        /// </summary>
        private readonly static IDialecter dialecter = new OracleDialecter();

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
            return new OracleConnection();            
        }

        /// <summary>
        /// CreateCommand
        /// </summary>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:丢失范围之前释放对象")]
        public override DbCommand CreateCommand()
        {
            var command = new OracleCommand();
            command.BindByName = true;
            return command;
        }

        /// <summary>
        /// CreateParameter
        /// </summary>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:丢失范围之前释放对象")]
        public override DbParameter CreateParameter()
        {
            return new OracleParameter();
        }
        /// <summary>
        /// CreateCommandBuilder
        /// </summary>
        /// <returns></returns>
        public override DbCommandBuilder CreateCommandBuilder()
        {
            return new OracleCommandBuilder();
        }

        /// <summary>
        /// CreateDataAdapter
        /// </summary>
        /// <returns></returns>
        public override DbDataAdapter CreateDataAdapter()
        {
            return new OracleDataAdapter();
        }

        /// <summary>
        /// CreateCompiler
        /// </summary>
        /// <returns></returns>
        public override SqlCompiler CreateCompiler()
        {
            return new OracleCompiler();
        }

        /// <summary>
        /// GetSchemaProvider
        /// </summary>
        /// <returns></returns>
        protected override SqlSchema CreateSchemaProvider()
        {
            return new OracleSchema();
        }

        /// <summary>
        /// SetParameterBinary
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="value"></param>
        protected override void SetParameterBinary(DbParameter parameter, object value)
        {
            if (value == null)
            {
                parameter.Value = value;
                return;
            }

            if (value.GetType() == typeof(Guid))
            {
                parameter.Value = ((Guid)value).ToByteArray();
            }
            else
            {
                parameter.Value = value;
            }
        }

        /// <summary>
        /// SetParameterType
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="dbType"></param>
        public override void SetParameterType(DbParameter parameter, DbType dbType)
        {
            switch (dbType)
            {
                case DbType.Boolean:
                    parameter.DbType = DbType.Int16;
                    break;
                case DbType.Guid:
                    parameter.DbType = DbType.Binary;
                    break;
                default:
                    base.SetParameterType(parameter, dbType);
                    break;
            }
        }

        /// <summary>
        /// SetParameterNumeric
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="value"></param>
        protected override void SetParameterNumeric(DbParameter parameter, object value)
        {
            OracleParameter p = (OracleParameter)parameter;
            if (p.DbType == DbType.Decimal && p.Precision == 0 && p.Scale == 0)
            {
                p.Precision = 38;
                p.Scale = byte.MaxValue;
            }
            parameter.Value = value;
        }

        /// <summary>
        /// SetParameterProviderDbType
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="providerDbType"></param>
        public override void SetParameterProviderDbType(DbParameter parameter, int providerDbType)
        {
            OracleParameter p = (OracleParameter)parameter;
            p.OracleDbType = (OracleDbType)providerDbType;
        }

        /// <summary>
        /// SetParameterPrecision
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="sqlType"></param>
        protected override void SetParameterPrecision(DbParameter parameter, byte? precision, byte? scale)
        {
            OracleParameter p = (OracleParameter)parameter;
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
        /// compile procedure
        /// </summary>
        /// <param name="procedure"></param>
        /// <param name="schemaProvider"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:检查 SQL 查询是否存在安全漏洞")]
        protected override DbCommand  CompileProcedure(Procedure procedure)
        {
            if (string.IsNullOrEmpty(procedure.Name))
            {
                throw new ArgumentException("procedure name can not be empty");
            }

            var parameterPrefix = Dialecter.ParameterPrefix();
            DbCommand command = CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = procedure.Name;

            if (procedure.Parameters != null)
            {
                foreach (var p in procedure.Parameters)
                {
                    DbParameter parameter = CreateParameter(p);
                    if (parameter.ParameterName[0] == parameterPrefix)
                    {
                        parameter.ParameterName = parameter.ParameterName.Substring(1);
                    }
                    command.Parameters.Add(parameter);
                }
            }
            return command;
        }
    }
}
