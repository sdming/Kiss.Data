using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kiss.Data.Expression;
using System.Data.Common;
using System.Data;
using Kiss.Data.Schema;
using System.Text.RegularExpressions;

namespace Kiss.Data.Driver
{
    /// <summary>
    /// DbDriver
    /// </summary>
    public class SqlDriver
    {
        #region provider

        /// <summary>
        /// SupportNamedParameter
        /// </summary>
        public virtual bool SupportNamedParameter
        {
            get { return false; }
        }

        /// <summary>
        /// ParameterPlaceHolder
        /// </summary>
        /// <returns></returns>
        public virtual string ParameterPlaceHolder(string name)
        {
            if (!SupportNamedParameter)
            {
                return Dialecter.QuoteParameter(name);
            }
            return " ? ";
        }

        /// <summary>
        /// DbProviderFactory
        /// </summary>
        protected DbProviderFactory factory = null;

        /// <summary>
        /// ProviderName
        /// </summary>
        public virtual string ProviderName { get; set; }

        /// <summary>
        /// return DbProviderFactory
        /// </summary>
        /// <returns></returns>
        public DbProviderFactory GetFactory() 
        {
            if(factory == null)
            {
                factory = DbProviderFactories.GetFactory(ProviderName);
            }

            if(factory == null)
            {
                throw new Exception("can not get provider factory:" + ProviderName);
            }
            return factory;
        }
        
        /// <summary>
        /// create DbConnection
        /// </summary>
        /// <returns></returns>
        public virtual DbConnection CreateConnection()
        {
            return GetFactory().CreateConnection();
        }

        /// <summary>
        /// create DbCommand
        /// </summary>
        /// <returns></returns>
        public virtual DbCommand CreateCommand()
        {
            return GetFactory().CreateCommand();
        }

        /// <summary>
        /// create DbParameter
        /// </summary>
        /// <returns></returns>
        public virtual DbParameter CreateParameter()
        {
            return GetFactory().CreateParameter();
        }

        /// <summary>
        /// CreateParameter
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual DbParameter CreateParameter(string name, object value)
        {
            DbParameter p = CreateParameter();
            p.ParameterName = name;
            if (value == null)
            {
                p.Value = DBNull.Value;
            }
            else
            {
                p.Value = value;
            }
            return p;
        }

        /// <summary>
        /// CreateCommandBuilder
        /// </summary>
        /// <returns></returns>
        public virtual DbCommandBuilder CreateCommandBuilder()
        {
            return GetFactory().CreateCommandBuilder();
        }

        /// <summary>
        /// CreateDataAdapter
        /// </summary>
        /// <returns></returns>
        public virtual DbDataAdapter CreateDataAdapter()
        {
            return GetFactory().CreateDataAdapter();
        }

        /// <summary>
        /// parameterRegex
        /// </summary>
        protected Regex parameterRegex;

        /// <summary>
        /// ParameterRegex
        /// </summary>
        /// <returns></returns>
        public virtual Regex ParameterRegex()
        {
            if(parameterRegex == null)
            {
                parameterRegex = new Regex(Dialecter.ParameterPrefix() + @"\w*");
            }
            return parameterRegex;
        }

        #endregion ProviderName

        #region compile

        /// <summary>
        /// SetParameterPrecision
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="sqlType"></param>
        protected virtual void SetParameterPrecision(DbParameter parameter, byte? precision, byte? scale)
        {

        }

        /// <summary>
        /// SetParameterType
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="sqlType"></param>
        public virtual void SetParameterType(DbParameter parameter, DbType dbType, int? size, byte? precision, byte? scale)
        {
            parameter.DbType = dbType;
            if (size.HasValue)
            {
                parameter.Size = size.Value;
            }
            else
            {
                if (parameter.DbType.IsString() || parameter.DbType == DbType.Binary)
                {
                    parameter.Size = Int32.MaxValue;
                }
            }
            SetParameterPrecision(parameter, precision, scale);
        }

        /// <summary>
        /// SetParameterType
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="sqlType"></param>
        public virtual void SetParameterType(DbParameter parameter, DbType dbType)
        {
            parameter.DbType = dbType;
            if (parameter.DbType.IsString() || parameter.DbType == DbType.Binary)
            {
                parameter.Size = Int32.MaxValue;
            }
        }

        /// <summary>
        /// SetParameterNullValue
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="value"></param>
        protected virtual void SetParameterNullValue(DbParameter parameter)
        {
            if (parameter.IsNullable)
            {
                parameter.Value = DBNull.Value;
            }
            else if (parameter.DbType.IsString())
            {
                parameter.Value = string.Empty;
            }
            else if (parameter.DbType.IsNumeric())
            {
                parameter.Value = 0;
            }
            else if (parameter.DbType.IsDateTime())
            {
                parameter.Value = Defaults.DateTime;
            }
            else if (parameter.DbType == DbType.Guid)
            {
                parameter.Value = Defaults.Guid;
            }
            else
            {
                parameter.Value = DBNull.Value;
            }
        }

        /// <summary>
        /// SetParameterDateTime
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="value"></param>
        protected virtual void SetParameterDateTime(DbParameter parameter, object value)
        {
            DateTime v;
            if (value is DateTime)
            {
                v = (DateTime)value;
            }
            else
            {
                v = Convert.ToDateTime(value);
            }

            if (v.Year < Defaults.DateTimeMinYear)
            {
                parameter.Value = Defaults.DateTime;
            }
            else
            {
                parameter.Value = v;
            }
        }

        /// <summary>
        /// SetParameterGuid
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="value"></param>
        protected virtual void SetParameterGuid(DbParameter parameter, object value)
        {
            if (value is Guid)
            {
                parameter.Value = value;
                return;
            }
            
            parameter.Value = Guid.Parse(value.ToString());
        }

        /// <summary>
        /// SetParameterString
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="value"></param>
        protected virtual void SetParameterString(DbParameter parameter, object value)
        {
            if (value == null)
            {
                parameter.Value = value;
            }
            else
            {
                parameter.Value = value.ToString();
            }            
        }

        /// <summary>
        /// SetParameterBinary
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="value"></param>
        protected virtual void SetParameterBinary(DbParameter parameter, object value)
        {
            parameter.Value = value;
        }


        /// <summary>
        /// SetParameterValue
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="value"></param>
        public virtual void SetParameterValue(DbParameter parameter, object value)
        {
            if (value == null)
            {
                SetParameterNullValue(parameter);
                return;
            }
            if (parameter.DbType.IsDateTime())
            {
                SetParameterDateTime(parameter, value);
                return;
            }

            //set sieze for string??

            switch (parameter.DbType)
            {
                case DbType.Guid:
                    SetParameterGuid(parameter, value);
                    return;
                case DbType.Binary:
                    SetParameterBinary(parameter, value);
                    return;
                case DbType.String:
                case DbType.StringFixedLength:
                case DbType.AnsiString:
                case DbType.AnsiStringFixedLength:
                    SetParameterString(parameter, value);
                    return;
                default:
                    break;
            }

            parameter.Value = value;
        }

        /// <summary>
        /// default ansi IDialecter
        /// </summary>
        private static IDialecter ansiDialecter = new SqlDialecter();

        /// <summary>
        /// Dialecter
        /// </summary>
        public virtual IDialecter Dialecter
        {
            get { return ansiDialecter; }
        }

        /// <summary>
        /// compile expression to DbCommand
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="schemaProvider"></param>
        /// <returns></returns>
        public virtual DbCommand Compile(ISqlExpression expression, IDbSchemaProvider schemaProvider)
        {
            if(expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            switch(expression.NodeType())
            {
                case NodeType.Text:
                    return CompileText(expression as Text);
                case NodeType.Procedure:
                    return CompileProcedure(expression as Procedure);
                case NodeType.Query:
                case NodeType.Delete:
                case NodeType.Insert:
                case NodeType.Update:
                    return CompileStatement(expression, schemaProvider);
            }

            throw new NotSupportedException("not support expression type:" + expression.NodeType().ToString());
        }

        /// <summary>
        /// CreateCompiler
        /// </summary>
        /// <returns></returns>
        public virtual SqlCompiler CreateCompiler()
        {
            return new SqlCompiler();
        }

        /// <summary>
        /// CompileStatement
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="schemaProvider"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:检查 SQL 查询是否存在安全漏洞")]
        protected virtual DbCommand CompileStatement(ISqlExpression expression, IDbSchemaProvider schemaProvider)
        {
            DbCommand command = CreateCommand();
            command.CommandType = CommandType.Text;
            SqlWriter writer = new SqlWriter();
            SqlCompiler c = CreateCompiler();
            c.Driver = this;
            c.Writer = writer;
            c.Command = command;
            //c.Schema = schemaProvider;
            c.Compile(expression);
            command.CommandText = writer.String();
            //writer.Reset();
            return command;
        }
        
        /// <summary>
        /// compile text command expression
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:检查 SQL 查询是否存在安全漏洞")]
        protected virtual DbCommand CompileText(Text text)
        {
            if (string.IsNullOrEmpty(text.CommandText))
            {
                throw new Exception("command text can not be empty.");
            }

            DbCommand command = CreateCommand();
            command.CommandText = text.CommandText;
            command.CommandType = CommandType.Text;
            var dialecter = Dialecter;

            if (text.Parameters != null)
            {
                foreach (var p in text.Parameters)
                {
                    DbParameter parameter = CreateParameter(p); 
                    command.Parameters.Add(parameter);
                }
            }

            return command;
        }

        /// <summary>
        /// CreateParameter
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        protected DbParameter CreateParameter(Parameter p)
        {
            var dialecter = Dialecter;
            DbParameter parameter = null;
            if (p.DbType.HasValue)
            {
                parameter = CreateParameter();
                parameter.ParameterName = dialecter.QuoteParameter(p.Name);
                SetParameterType(parameter, p.DbType.Value, p.Size, p.Precision, p.Scale);
                SetParameterValue(parameter, p.Value);
            }
            else
            {
                parameter = CreateParameter(dialecter.QuoteParameter(p.Name), p.Value);
            }
            parameter.Direction = p.Direction;
            return parameter;
        }

        /// <summary>
        /// compile procedure
        /// </summary>
        /// <param name="procedure"></param>
        /// <param name="schemaProvider"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:检查 SQL 查询是否存在安全漏洞")]
        protected DbCommand CompileProcedure(Procedure procedure)
        {
            if (string.IsNullOrEmpty(procedure.Name))
            {
                throw new ArgumentException("procedure name can not be empty");
            }

            DbCommand command = CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = procedure.Name;
            
            if (procedure.Parameters != null)
            {
                foreach (var p in procedure.Parameters)
                {
                    DbParameter parameter = CreateParameter(p); 
                    command.Parameters.Add(parameter);
                }
            }
            return command;
        }

        #endregion

        #region schema

        protected SqlSchema schemaProvider;

        public SqlSchema GetSchemaProvider()
        {
            if (schemaProvider == null)
            {
                schemaProvider = CreateSchemaProvider();
            }
            return schemaProvider;
        }

        protected virtual SqlSchema CreateSchemaProvider()
        {
            return new SqlSchema();
        }

        public virtual SqlTable GetTable(string tableName, string connectionString)
        {
            return GetSchemaProvider().GetTable(this, tableName, connectionString);
        }

        public virtual SqlProcedure GetProcedure(string procedureName, string connectionString)
        {
            throw new Exception("GetProcedure:" + procedureName);
        }

        #endregion

        public SqlDriver()
        {
            
        }

    }
}
