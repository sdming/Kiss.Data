using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Kiss.Data.Driver
{
    /// <summary>
    /// DbDriverFactory
    /// </summary>
    public static class SqlDriverFactory
    {
        class DbDriverInvariantName
        {
            public const string SqlClient = "System.Data.SqlClient";
            public const string MySql = "MySql.Data.MySqlClient";
            public const string OleDb = "System.Data.OleDb";
            public const string Odbc = "System.Data.Odbc";
            public const string Oracle = "Oracle.DataAccess.Client";
            public const string OracleManaged = "Oracle.ManagedDataAccess.Client";
            public const string SQLite = "System.Data.SQLite";
            public const string Npgsql = "Npgsql";
            public const string Sql = "SQL";
        }

        #region drivers
        private static Lazy<MsSqlDriver> sqlClient = new Lazy<MsSqlDriver>(LazyThreadSafetyMode.PublicationOnly);
        private static Lazy<MySqlDriver> mySql = new Lazy<MySqlDriver>(LazyThreadSafetyMode.PublicationOnly);
        //private static Lazy<OleDbDriver> oleDb = new Lazy<OleDbDriver>(LazyThreadSafetyMode.PublicationOnly);
        //private static Lazy<OdbcDriver> odbc = new Lazy<OdbcDriver>(LazyThreadSafetyMode.PublicationOnly);
        private static Lazy<OracleDriver> oracle = new Lazy<OracleDriver>(LazyThreadSafetyMode.PublicationOnly);
        private static Lazy<OracleManagedDriver> oracleManaged = new Lazy<OracleManagedDriver>(LazyThreadSafetyMode.PublicationOnly);
        private static Lazy<SQLiteDriver> sqlite = new Lazy<SQLiteDriver>(LazyThreadSafetyMode.PublicationOnly);
        private static Lazy<PostgresDriver> npgsql = new Lazy<PostgresDriver>(LazyThreadSafetyMode.PublicationOnly);
        private static Lazy<SqlDriver> sql = new Lazy<SqlDriver>(LazyThreadSafetyMode.PublicationOnly);

        private readonly static object lockSync = new object();

        /// <summary>
        /// SqlClient
        /// </summary>
        /// <returns></returns>
        public static SqlDriver SqlClient()
        {            
            return sqlClient.Value;
        }

        ///// <summary>
        ///// OleDb
        ///// </summary>
        ///// <returns></returns>
        //public static SqlDriver OleDb()
        //{
        //    return oleDb.Value;
        //}

        ///// <summary>
        ///// Odbc
        ///// </summary>
        ///// <returns></returns>
        //public static SqlDriver Odbc()
        //{
        //    return odbc.Value;
        //}

        /// <summary>
        /// OracleManaged
        /// </summary>
        /// <returns></returns>
        public static SqlDriver OracleManaged()
        {
            return oracleManaged.Value;
        }

        /// <summary>
        /// Sqlite
        /// </summary>
        /// <returns></returns>
        public static SqlDriver Sqlite()
        {
            return sqlite.Value;
        }

        /// <summary>
        /// npgsql
        /// </summary>
        /// <returns></returns>
        public static SqlDriver Npgsql()
        {
            return npgsql.Value;
        }

        /// <summary>
        /// oracle
        /// </summary>
        /// <returns></returns>
        public static SqlDriver Oracle()
        {
            return oracle.Value;
        }

        /// <summary>
        /// MySql
        /// </summary>
        /// <returns></returns>
        public static SqlDriver MySql()
        {
            return mySql.Value;
        }

        /// <summary>
        /// Sql
        /// </summary>
        /// <returns></returns>
        public static SqlDriver Sql()
        {
            return sql.Value;
        }


        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="providerName"></param>
        /// <returns></returns>
        public static SqlDriver Get(string providerName)
        {
            if (string.Equals(providerName, DbDriverInvariantName.SqlClient, StringComparison.InvariantCultureIgnoreCase))
            {
                return SqlClient();
            }
            else if (string.Equals(providerName, DbDriverInvariantName.MySql, StringComparison.InvariantCultureIgnoreCase))
            {
                return MySql();
            }
            else if (string.Equals(providerName, DbDriverInvariantName.Npgsql, StringComparison.InvariantCultureIgnoreCase))
            {
                return Npgsql();
            }  
            else if (string.Equals(providerName, DbDriverInvariantName.Oracle, StringComparison.InvariantCultureIgnoreCase))
            {
                return Oracle();
            }
            else if (string.Equals(providerName, DbDriverInvariantName.OracleManaged, StringComparison.InvariantCultureIgnoreCase))
            {
                return OracleManaged();
            }
            else if (string.Equals(providerName, DbDriverInvariantName.SQLite, StringComparison.InvariantCultureIgnoreCase))
            {
                return Sqlite();
            }            
            //else if (string.Equals(providerName, DbDriverInvariantName.OleDb, StringComparison.InvariantCultureIgnoreCase))
            //{
            //    return OleDb();
            //}
            //else if (string.Equals(providerName, DbDriverInvariantName.Odbc, StringComparison.InvariantCultureIgnoreCase))
            //{
            //    return Odbc();
            //}
            else if (string.Equals(providerName, DbDriverInvariantName.Sql, StringComparison.InvariantCultureIgnoreCase))
            {
                return Sql();
            }
            else
            {
                throw new NotImplementedException("do not support driver:" + providerName);
            }
        }
    }
}
