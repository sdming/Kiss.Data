using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading;
using System.Transactions;
using System.Data.Common;
using System.Configuration;
using Kiss.Data.Driver;
using Kiss.Data.Expression;

namespace Kiss.Data
{
    /// <summary>
    /// DbContent
    /// </summary>
    public partial class DbContent : IDisposable
    {
        #region properties
        protected string connectionString;
        protected string providerName;
        public string Name { get; set; }
        public int? CommandTimeout { get; set; }
        public ConnectionStringSettings setting { get; private set; }
        public IContentListener Listener  { get; set; }
        #endregion

        #region constructors
        public DbContent(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("name can not be null or empty.", "name");
            }
            this.Name = name;
        }

        public DbContent(ConnectionStringSettings setting)
        {
            if (setting == null)
            {
                throw new ArgumentNullException("setting");
            }
            this.setting = setting;
            this.Name = setting.Name;
        }
        #endregion

        #region connection
        protected SqlDriver driver;
        protected DbConnection dbConnection;
        protected int connectCount;
        //protected ConnectionState state = ConnectionState.Closed;

        protected DbConnection UseConnection()
        {
            OpenConnection();
            connectCount++;
            return this.dbConnection;
        }

        protected void ReleaseConnection()
        {
            connectCount--;
        }

        protected bool IsUsingConnection
        {
            get { return connectCount > 0; }
        }

        public void CloseConnection()
        {
            if (dbConnection == null)
            {
                connectCount = 0;
                return;
            }

            if (dbConnection.State != ConnectionState.Closed)
            {
                dbConnection.Close();
                connectCount = 0;
            }
        }

        public DbConnection OpenConnection()
        {
            if (dbConnection == null)
            {
                dbConnection = CreateConnection();
                dbConnection.ConnectionString = setting.ConnectionString;
            }
            if (dbConnection.State == ConnectionState.Closed)
            {
                dbConnection.Open();
            }
            return dbConnection;
        }

        public ConnectionState State()
        {
            if (dbConnection == null)
            {
                return ConnectionState.Closed;
            }
            return dbConnection.State;
        }

        protected DbConnection CreateConnection()
        {
            if (this.Listener != null)
            {
                this.Listener.Preparing(this);
            }

            var connection = Driver().CreateConnection();
            //connection.StateChange += delegate(Object sender, StateChangeEventArgs e)
            //{
            //    state = e.CurrentState;
            //};
            return connection;
        }

        public SqlDriver Driver()
        {
            if (this.driver == null)
            {
                EnsureSetting();
                this.driver = SqlDriverFactory.Get(setting.ProviderName);
            }
            
            return this.driver;
        }

        protected void EnsureSetting()
        {
            if (setting == null)
            {
                setting = GetConnectionStringSettings();
            }
            if (setting == null)
            {
                throw new Exception("can not find connection string settings:" + Name);
            }
        }

        protected ConnectionStringSettings GetConnectionStringSettings()
        {
            //return ConfigurationManager.ConnectionStrings[Name];
            return DbStrategy.SettingsProvider(Name);
        }

        protected void ClearConnection()
        {
            if (dbConnection != null && !IsUsingConnection)
            {
                CloseConnection();
                dbConnection.Dispose();
            }
        }

        public DbCommand Compile(ISqlExpression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            if (Listener != null)
            {
                Listener.Compiling(this, expression);
            }

            return Driver().Compile(expression, null);
        }
        #endregion

        #region Transaction

        protected DbTransaction dbTransaction;

        public DbTransaction Transaction
        {
            get { return dbTransaction; }
        }

        protected int transactionCount;

        public int TransactionCount
        {
            get { return transactionCount; }
        }

        public void ClearTransaction()
        {
            if (dbTransaction == null)
            {
                return;
            }
            RollbackTransaction();
            dbTransaction.Dispose();
            dbTransaction = null;
            transactionCount = 0;
        }

        public void BeginTransaction()
        {
            dbTransaction = OpenConnection().BeginTransaction();
            transactionCount++;
        }

        public void BeginTransaction(System.Data.IsolationLevel level)
        {
            OpenConnection();
            dbTransaction = OpenConnection().BeginTransaction(level);
            transactionCount++;
        }

        public void CommitTransaction()
        {
            if (ExistsTransition)
            {
                dbTransaction.Commit();
                transactionCount--;
            }
        }

        public void RollbackTransaction()
        {
            if (ExistsTransition)
            {
                dbTransaction.Rollback();
                transactionCount--;
            }
        }

        public bool ExistsTransition
        {
            get
            {
                if (transactionCount <= 0 || dbTransaction == null)
                {
                    return false;
                }
                return true;
            }
        }

        public void EnlistTransaction(Transaction tx)
        {
            OpenConnection().EnlistTransaction(tx);
        }

        #endregion //Transaction

        #region schema

        /// <summary>
        /// GetSchemaProvider
        /// </summary>
        /// <returns></returns>
        protected IDbSchemaProvider GetSchemaProvider()
        {
            if (DbStrategy.SchemaProvider == null)
            {
                throw new Exception("DbStrategy.SchemaProvider can not be null");
            }
            IDbSchemaProvider schema = DbStrategy.SchemaProvider(Name);
            if (schema == null)
            {
                throw new Exception("can not get IDbSchemaProvider:" + Name);
            }
            return schema;
        }

        #endregion

        #region IDisposable
        private bool disposed;

        ~DbContent()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                try
                {
                    ClearTransaction();
                }
                catch { }

                try
                {
                    ClearConnection();
                }
                catch { }

                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
