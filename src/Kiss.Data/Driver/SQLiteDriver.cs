using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data.SQLite;
//using System.Data.SQLite;

namespace Kiss.Data.Driver
{
    /// <summary>
    /// SQLiteDriver
    /// </summary>
    public class SQLiteDriver : SqlDriver
    {
        /// <summary>
        /// sqlite IDialecter
        /// </summary>
        private readonly static IDialecter dialecter = new SQLiteDialecter();

        /// <summary>
        /// Dialecter
        /// </summary>
        public override IDialecter Dialecter
        {
            get { return dialecter; }
        }

        /// <summary>
        /// ProviderName
        /// </summary>
        public override string ProviderName
        {
            get { return "System.Data.SQLite"; }
        }

        /// <summary>
        /// SupportNamedParameter
        /// </summary>
        public override bool SupportNamedParameter
        {
            get { return true; }
        }

        /// <summary>
        /// CreateConnection
        /// </summary>
        /// <returns></returns>
        public override DbConnection CreateConnection()
        {
            return new SQLiteConnection();
        }

        /// <summary>
        /// CreateCommand
        /// </summary>
        /// <returns></returns>
        public override DbCommand CreateCommand()
        {
            return new SQLiteCommand();
        }

        /// <summary>
        /// CreateParameter
        /// </summary>
        /// <returns></returns>
        public override DbParameter CreateParameter()
        {
            return new SQLiteParameter();
        }

        /// <summary>
        /// CreateCommandBuilder
        /// </summary>
        /// <returns></returns>
        public override DbCommandBuilder CreateCommandBuilder()
        {
            return new SQLiteCommandBuilder();
        }

        /// <summary>
        /// CreateDataAdapter
        /// </summary>
        /// <returns></returns>
        public override DbDataAdapter CreateDataAdapter()
        {
            return new SQLiteDataAdapter();
        }

        /// <summary>
        /// CreateCompiler
        /// </summary>
        /// <returns></returns>
        public override SqlCompiler CreateCompiler()
        {
            return new SQLiteCompiler();
        }

        /// <summary>
        /// GetSchemaProvider
        /// </summary>
        /// <returns></returns>
        protected override SqlSchema CreateSchemaProvider()
        {
            return new SQLiteSchema();
        }
    }
}
