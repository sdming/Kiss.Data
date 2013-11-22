using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;
using Kiss.Data.Expression;
using Kiss.Core;
using Kiss.Data.Schema;
using System.Text.RegularExpressions;

namespace Kiss.Data
{
    /// <summary>
    /// DbContent
    /// </summary>
    public partial class DbContent
    {
        #region table

        /// <summary>
        /// get schema of table
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        protected SqlTable GetTable(string tableName)
        {
            IDbSchemaProvider schema = GetSchemaProvider();
            var t = schema.GetTable(tableName);
            if (t == null)
            {
                throw new Exception("can not get schema of:" + tableName);
            }
            return t;
        }

        /// <summary>
        /// Table
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public TableGate Table(string tableName)
        {
            return Table(tableName, false);
        }

        /// <summary>
        /// Table
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="ignoreSchema"></param>
        /// <returns></returns>
        public TableGate Table(string tableName, bool ignoreSchema)
        {
            if(string.IsNullOrWhiteSpace(tableName))
            {
                throw new ArgumentException("table name can not be null or empty", "tableName");
            }
            if (ignoreSchema)
            {
                return new TableGate(this, tableName, null);
            }
            var schema = GetTable(tableName);
            return new TableGate(this, tableName, schema);
        }

        #endregion

        #region procedure
        protected SqlProcedure GetProcedure(string procedureName)
        {
            IDbSchemaProvider schema = GetSchemaProvider();
            var p = schema.GetProcedure(procedureName);
            if (p == null)
            {
                throw new Exception("can not get schema of:" + procedureName);
            }
            return p;
        }

        /// <summary>
        /// BuildProcedure
        /// </summary>
        /// <param name="procedureName"></param>
        /// <param name="parameters"></param>
        /// <param name="ignoreSchema"></param>
        /// <returns></returns>
        public Procedure BuildProcedure(string procedureName, IDataObjectAdapter parameters, bool ignoreSchema)
        {
            if (string.IsNullOrEmpty(procedureName))
            {
                throw new Exception("procedureName can not be null or empty");
            }

            Procedure exp = null;
            if (ignoreSchema)
            {
                exp = new Procedure(procedureName);
                if (parameters != null)
                {
                    foreach (var f in parameters.Fields())
                    {
                        exp.Set(f, parameters.Get(f));
                    }
                }
                return exp;
            }

            var schema = GetProcedure(procedureName);
            exp = new Procedure(schema.Name);

            if (schema.Parameters != null)
            {
                for (var i = 0; i < schema.Parameters.Count; i++)
                {
                    var p = schema.Parameters[i];
                    Parameter pv = new Parameter();
                    p.Copy(pv);

                    if (p.Direction == ParameterDirection.ReturnValue)
                    {
                        pv.Value = DBNull.Value;
                        continue;
                    }

                    if (parameters != null && parameters.Contains(p.Name))
                    {
                        pv.Value = parameters.Get(p.Name);
                        exp.Set(pv);
                    }
                    else if (p.Direction == ParameterDirection.InputOutput ||  p.Direction == ParameterDirection.Output || p.Direction == ParameterDirection.ReturnValue)
                    {
                        pv.Value = DBNull.Value;
                        exp.Set(pv);
                    }
                }
            }

            return exp;
        }

        /// <summary>
        /// BuildProcedure
        /// </summary>
        /// <param name="procedureName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public Procedure BuildProcedure(string procedureName, params object[] parameters)
        {
            if (string.IsNullOrEmpty(procedureName))
            {
                throw new Exception("procedureName can not be null or empty");
            }

            var schema = GetProcedure(procedureName);
            Procedure exp = new Procedure(schema.Name);

            int j = -1;
            if (schema.Parameters != null)
            {
                for (var i = 0; i < schema.Parameters.Count; i++)
                {
                    var p = schema.Parameters[i];
                    Parameter pv = new Parameter();
                    p.Copy(pv);
                    
                    if (p.Direction == ParameterDirection.ReturnValue || p.Direction == ParameterDirection.Output)
                    {
                        pv.Value = DBNull.Value;
                        continue;
                    }

                    j++;
                    if (parameters != null && parameters.Length > j)
                    {
                        pv.Value = parameters[i];
                        exp.Set(pv);
                    }
                    else if (p.Direction == ParameterDirection.Output || p.Direction == ParameterDirection.ReturnValue)
                    {
                        pv.Value = DBNull.Value;
                        exp.Set(pv);
                    }
                    else
                    {
                        //set?
                    }
                }
            }
            return exp;
        }

        /// <summary>
        /// ProcedureNonQuery
        /// </summary>
        /// <param name="procedureName"></param>
        /// <param name="parameters"></param>
        /// <param name="ignoreSchema"></param>
        /// <returns></returns>
        public int ProcedureNonQuery(string procedureName, IDataObjectAdapter parameters, bool ignoreSchema)
        {
            var exp = BuildProcedure(procedureName, parameters, ignoreSchema);
            return ExecuteNonQuery(exp);
        }

        /// <summary>
        /// ProcedureNonQuery
        /// </summary>
        /// <param name="procedureName"></param>
        /// <param name="parameters"></param>
        /// <param name="ignoreSchema"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public int ProcedureNonQuery(string procedureName, IDataObjectAdapter parameters, bool ignoreSchema, out IExecuteResult result)
        {
            var exp = BuildProcedure(procedureName, parameters, ignoreSchema);
            var i = ExecuteNonQuery(exp, out result);
            return i;
        }

        /// <summary>
        /// ProcedureNonQuery
        /// </summary>
        /// <param name="procedureName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public int ProcedureNonQuery(string procedureName, IDataObjectAdapter parameters)
        {
            var exp = BuildProcedure(procedureName, parameters, false);
            return ExecuteNonQuery(exp);
        }

        /// <summary>
        /// ProcedureNonQuery
        /// </summary>
        /// <param name="procedureName"></param>
        /// <param name="parameters"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public int ProcedureNonQuery(string procedureName, IDataObjectAdapter parameters, out IExecuteResult result)
        {
            var exp = BuildProcedure(procedureName, parameters, false);
            var i = ExecuteNonQuery(exp, out result);
            return i;
        }

        /// <summary>
        /// ProcedureReader
        /// </summary>
        /// <param name="procedureName"></param>
        /// <param name="parameters"></param>
        /// <param name="ignoreSchema"></param>
        /// <returns></returns>
        public IDataReader ProcedureReader(string procedureName, IDataObjectAdapter parameters, bool ignoreSchema)
        {
            var exp = BuildProcedure(procedureName, parameters, ignoreSchema);
            return ExecuteReader(exp);
        }

        /// <summary>
        /// ProcedureReader
        /// </summary>
        /// <param name="procedureName"></param>
        /// <param name="parameters"></param>
        /// <param name="ignoreSchema"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public IDataReader ProcedureReader(string procedureName, IDataObjectAdapter parameters, bool ignoreSchema, out IExecuteResult result)
        {
            var exp = BuildProcedure(procedureName, parameters, ignoreSchema);
            return ExecuteReader(exp, out result);
        }

        /// <summary>
        /// ProcedureReader
        /// </summary>
        /// <param name="procedureName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public IDataReader ProcedureReader(string procedureName, IDataObjectAdapter parameters)
        {
            var exp = BuildProcedure(procedureName, parameters, false);
            return ExecuteReader(exp);
        }

        /// <summary>
        /// ProcedureReader
        /// </summary>
        /// <param name="procedureName"></param>
        /// <param name="parameters"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public IDataReader ProcedureReader(string procedureName, IDataObjectAdapter parameters, out IExecuteResult result)
        {
            var exp = BuildProcedure(procedureName, parameters, false);
            return ExecuteReader(exp, out result);
        }

        /// <summary>
        /// ProcedureScalar
        /// </summary>
        /// <param name="procedureName"></param>
        /// <param name="parameters"></param>
        /// <param name="ignoreSchema"></param>
        /// <returns></returns>
        public object ProcedureScalar(string procedureName, IDataObjectAdapter parameters, bool ignoreSchema)
        {
            var exp = BuildProcedure(procedureName, parameters, ignoreSchema);
            return ExecuteScalar(exp);
        }

        /// <summary>
        /// ProcedureScalar
        /// </summary>
        /// <param name="procedureName"></param>
        /// <param name="parameters"></param>
        /// <param name="ignoreSchema"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public object ProcedureScalar(string procedureName, IDataObjectAdapter parameters, bool ignoreSchema, out IExecuteResult result)
        {
            var exp = BuildProcedure(procedureName, parameters, ignoreSchema);
            return ExecuteScalar(exp, out result);
        }

        /// <summary>
        /// ProcedureScalar
        /// </summary>
        /// <param name="procedureName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public object ProcedureScalar(string procedureName, IDataObjectAdapter parameters)
        {
            var exp = BuildProcedure(procedureName, parameters, false);
            return ExecuteScalar(exp);
        }

        /// <summary>
        /// ProcedureScalar
        /// </summary>
        /// <param name="procedureName"></param>
        /// <param name="parameters"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public object ProcedureScalar(string procedureName, IDataObjectAdapter parameters, out IExecuteResult result)
        {
            var exp = BuildProcedure(procedureName, parameters, false);
            return ExecuteScalar(exp, out result);
        }

        /// <summary>
        /// ProcedureNonQuery
        /// </summary>
        /// <param name="procedureName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public int ProcedureNonQuery(string procedureName, params object[] parameters)
        {
            var exp = BuildProcedure(procedureName, parameters);
            return ExecuteNonQuery(exp);
        }

        /// <summary>
        /// ProcedureReader
        /// </summary>
        /// <param name="procedureName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public IDataReader ProcedureReader(string procedureName, params object[] parameters)
        {
            var exp = BuildProcedure(procedureName, parameters);
            return ExecuteReader(exp);
        }

        /// <summary>
        /// ProcedureScalar
        /// </summary>
        /// <param name="procedureName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public object ProcedureScalar(string procedureName, params object[] parameters)
        {
            var exp = BuildProcedure(procedureName, parameters);
            return ExecuteScalar(exp);
        }

        #endregion

        #region text

        /// <summary>
        /// BuildText
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public Text BuildText(string commandText, IDataObjectAdapter parameters)
        {
            if (string.IsNullOrEmpty(commandText))
            {
                throw new Exception("commandText can not be null or empty");
            }

            Text exp = new Text(commandText);
            char prefix = Driver().Dialecter.ParameterPrefix();

            MatchCollection matches = Driver().ParameterRegex().Matches(commandText);
            for (int i = 0; i < matches.Count; i++)
            {
                Parameter pv = new Parameter();
                pv.Name = StringUtils.RemovePrefix(matches[i].Value, prefix);
                if (parameters != null && parameters.Contains(pv.Name))
                {
                    pv.Value = parameters.Get(pv.Name);
                }
                else
                {
                    pv.Value = DBNull.Value;
                }
                exp.Set(pv);
            }

            return exp;
        }

        /// <summary>
        /// BuildText
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public Text BuildText(string commandText, params object[] parameters)
        {
            if (string.IsNullOrEmpty(commandText))
            {
                throw new Exception("commandText can not be null or empty");
            }

            Text exp = new Text(commandText);
            char prefix = Driver().Dialecter.ParameterPrefix();

            MatchCollection matches = Driver().ParameterRegex().Matches(commandText);
            for (int i = 0; i < matches.Count; i++)
            {
                Parameter pv = new Parameter();
                pv.Name = StringUtils.RemovePrefix(matches[i].Value, prefix);
                if (parameters != null && parameters.Length > i)
                {
                    pv.Value = parameters[i];
                }
                else
                {
                    pv.Value = DBNull.Value;
                }
                exp.Set(pv);
            }
            return exp;
        }

        /// <summary>
        /// TextNonQuery
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public int TextNonQuery(string commandText, IDataObjectAdapter parameters)
        {
            var exp = BuildText(commandText, parameters);
            return ExecuteNonQuery(exp);
        }

        /// <summary>
        /// TextReader
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public IDataReader TextReader(string commandText, IDataObjectAdapter parameters)
        {
            var exp = BuildText(commandText, parameters);
            return ExecuteReader(exp);
        }

        /// <summary>
        /// TextScalar
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public object TextScalar(string commandText, IDataObjectAdapter parameters)
        {
            var exp = BuildText(commandText, parameters);
            return ExecuteScalar(exp);
        }

        /// <summary>
        /// TextNonQuery
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public int TextNonQuery(string commandText, params object[] parameters)
        {
            var exp = BuildText(commandText, parameters);
            return ExecuteNonQuery(exp);
        }

        /// <summary>
        /// TextReader
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public IDataReader TextReader(string commandText, params object[] parameters)
        {
            var exp = BuildText(commandText, parameters);
            return ExecuteReader(exp);
        }

        /// <summary>
        /// TextScalar
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public object TextScalar(string commandText, params object[] parameters)
        {
            var exp = BuildText(commandText, parameters);
            return ExecuteScalar(exp);
        }

        #endregion

        #region command

        /// <summary>
        /// ExecuteReader
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public IDataReader ExecuteReader(DbCommand command)
        {
            IDataReader result = null;
            Execute(command, (x) => result = x.ExecuteReader(CommandBehavior.CloseConnection), false);
            return result;
        }

        /// <summary>
        /// ExecuteReader
        /// </summary>
        /// <param name="command"></param>
        /// <param name="behavior"></param>
        /// <param name="cached"></param>
        /// <returns></returns>
        public IDataReader ExecuteReader(DbCommand command, CommandBehavior behavior, bool cached)
        {
            IDataReader result = null;
            //bool release = (behavior & CommandBehavior.CloseConnection) == CommandBehavior.CloseConnection;

            Execute(command, (x) => result = x.ExecuteReader(behavior), false);
            if (cached)
            {
                DataTable dt = Utils.Read2Table(result);
                return dt.CreateDataReader();
            }
            return result;
        }

        /// <summary>
        /// ExecuteScalar
        /// </summary>
        /// <param name="command"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public object ExecuteScalar(DbCommand command)
        {
            object result = null;
            Execute(command, (x) => result = x.ExecuteScalar(), true);
            return result;
        }

        /// <summary>
        /// ExecuteNonQuery
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(DbCommand command)
        {
            int rowsAffected = -1;
            Execute(command, (x) => rowsAffected = x.ExecuteNonQuery(), true);
            return rowsAffected;
        }

        #endregion

        #region expression

        /// <summary>
        /// InternalExecuteNonQuery
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected int InternalExecuteNonQuery(ISqlExpression expression)
        {
            var command = Compile(expression);
            return ExecuteNonQuery(command);
        }

        /// <summary>
        /// InternalExecuteNonQuery
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        protected int InternalExecuteNonQuery(ISqlExpression expression, out IExecuteResult result)
        {
            var command = Compile(expression);
            var i = ExecuteNonQuery(command);
            result = new ExecuteResult(command.Parameters, Driver().Dialecter.ParameterPrefix(), i);
            return i;
        }

        /// <summary>
        /// execute update 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(Update expression)
        {
            return InternalExecuteNonQuery(expression);
        }

        /// <summary>
        /// execute insert 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(Insert expression)
        {
            return InternalExecuteNonQuery(expression);
        }

        /// <summary>
        /// execute delete 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(Delete expression)
        {
            return InternalExecuteNonQuery(expression);
        }

        /// <summary>
        /// execute sql 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(Text expression)
        {
            return InternalExecuteNonQuery(expression);
        }

        /// <summary>
        /// execute text with result
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(Text expression, out IExecuteResult result)
        {
            return InternalExecuteNonQuery(expression, out result);
        }

        /// <summary>
        /// execute procedure 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(Procedure expression)
        {
            return InternalExecuteNonQuery(expression);
        }

        /// <summary>
        /// execute procedure with result
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(Procedure expression, out IExecuteResult result)
        {
            return InternalExecuteNonQuery(expression, out result);
        }

        /// <summary>
        /// InternalExecuteScalar
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected object InternalExecuteScalar(ISqlExpression expression)
        {
            var command = Compile(expression);
            return ExecuteScalar(command);
        }

        /// <summary>
        /// InternalExecuteScalar
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        protected object InternalExecuteScalar(ISqlExpression expression, out IExecuteResult result)
        {
            var command = Compile(expression);
            var i = ExecuteScalar(command);
            result = new ExecuteResult(command.Parameters, Driver().Dialecter.ParameterPrefix(), -1);
            return i;
        }

        /// <summary>
        /// query text 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public object ExecuteScalar(Text expression)
        {
            return InternalExecuteScalar(expression);
        }

        /// <summary>
        /// query text with result
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public object ExecuteScalar(Text expression, out IExecuteResult result)
        {
            return InternalExecuteScalar(expression, out result);
        }

        /// <summary>
        /// query procedure 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public object ExecuteScalar(Procedure expression)
        {
            return InternalExecuteScalar(expression);
        }

        /// <summary>
        /// query procedure with result
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public object ExecuteScalar(Procedure expression, out IExecuteResult result)
        {
            return InternalExecuteScalar(expression, out result);
        }

        /// <summary>
        /// query expression 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public object ExecuteScalar(Query expression)
        {
            return InternalExecuteScalar(expression);
        }

        /// <summary>
        /// query expression 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public object ExecuteScalar(Insert expression)
        {
            return InternalExecuteScalar(expression);
        }

        /// <summary>
        /// InternalExecuteReader
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected IDataReader InternalExecuteReader(ISqlExpression expression)
        {
            var command = Compile(expression);
            return ExecuteReader(command);
        }

        /// <summary>
        /// InternalExecuteReader with result
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        protected IDataReader InternalExecuteReader(ISqlExpression expression, out IExecuteResult result)
        {
            var command = Compile(expression);
            var i = ExecuteReader(command);
            result = new ExecuteResult(command.Parameters, Driver().Dialecter.ParameterPrefix(), i.RecordsAffected);
            return i;
        }

        /// <summary>
        /// ExecuteReader
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public IDataReader ExecuteReader(Text expression)
        {
            return InternalExecuteReader(expression);
        }

        /// <summary>
        /// ExecuteReader with result
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public IDataReader ExecuteReader(Text expression, out IExecuteResult result)
        {
            return InternalExecuteReader(expression, out result); 
            
        }

        /// <summary>
        /// query procedure 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public IDataReader ExecuteReader(Procedure expression)
        {
            return InternalExecuteReader(expression);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public IDataReader ExecuteReader(Procedure expression, out IExecuteResult result)
        {
            return InternalExecuteReader(expression, out result); 
        }

        /// <summary>
        /// query expression 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public IDataReader ExecuteReader(Query expression)
        {
            return InternalExecuteReader(expression);
        }

        /// <summary>
        /// query expression 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public IDataReader ExecuteReader(Insert expression)
        {
            return InternalExecuteReader(expression);
        }

        #endregion

        #region internal execute

        /// <summary>
        /// Prepare
        /// </summary>
        /// <param name="command"></param>
        protected void Prepare(DbCommand command)
        {
            command.Connection = UseConnection();
            command.Transaction = this.Transaction;            
            if (this.CommandTimeout.HasValue)
            {
                command.CommandTimeout = this.CommandTimeout.Value;
            }
        }
        
        /// <summary>
        /// Terminate
        /// </summary>
        /// <param name="closeConnection"></param>
        protected void Terminate(bool closeConnection)
        {
            if (closeConnection)
            {
                ReleaseConnection();
            }
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="command"></param>
        /// <param name="action"></param>
        /// <param name="releaseConnection"></param>
        protected void Execute(DbCommand command, Action<DbCommand> action, bool releaseConnection)
        {
            long start = KTimer.QueryTick();
            try
            {
                Prepare(command);

                if (Listener != null)
                {
                    Listener.Executing(this, command);
                }
   
                action(command);
                
                if (Trace.Level >= Tracelevel.Debug)
                {
                    var d = KTimer.Elapsed(start);
                    if (Trace.Tracer != null)
                    {
                        string dump = Utils.Dump(command);
                        Trace.Tracer(new TraceData() { Duration = d, Level = Tracelevel.Debug, Source = "DbContent.Execute", Message = dump });
                    }
                }
            }
            catch (Exception ex)
            {
                if (Trace.Level >= Tracelevel.Error)
                {
                    var d = KTimer.Elapsed(start);
                    if (Trace.Tracer != null)
                    {
                        string dump = Utils.Dump(command);
                        Trace.Tracer(new TraceData() { Duration = d, Level = Tracelevel.Error, Source = "DbContent.Execute", Message = dump, StackTrace= ex.ToString() });
                    }
                }
                throw;
            }
            finally
            {
                Terminate(releaseConnection);
            }
        }

        #endregion
    }
}
