using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Collections.Concurrent;
using Kiss.Data.Schema;
using Kiss.Data.Driver;

namespace Kiss.Data
{
    /// <summary>
    /// read schema from database
    /// </summary>
    public class NativeSchemaProvider : IDbSchemaProvider
    {
        public string Name { get; private set; }
        protected ConnectionStringSettings setting;
        protected ConcurrentDictionary<string, SqlProcedure> procedures = new ConcurrentDictionary<string, SqlProcedure>(StringComparer.OrdinalIgnoreCase);
        protected ConcurrentDictionary<string, SqlTable> tables = new ConcurrentDictionary<string, SqlTable>(StringComparer.OrdinalIgnoreCase);
        protected readonly object syncLock = new object();

        public NativeSchemaProvider(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("name can not be null or empty.", "name");
            }
            this.Name = name;
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
            return ConfigurationManager.ConnectionStrings[Name];
        }

        public void RefreshTable(string name)
        {
            SqlTable t = GetTableFromDb(name);
            if (t != null)
            {
                tables.AddOrUpdate(name, t, (k, v) => v);
            }
        }

        public SqlTable GetTable(string name)
        {
            SqlTable t;
            if (tables.TryGetValue(name, out t))
            {
                return t;
            }

            lock (syncLock)
            {
                if (tables.TryGetValue(name, out t))
                {
                    return t;
                }

                t = GetTableFromDb(name);
                if (t != null)
                {
                    tables.TryAdd(name, t);
                }
            }
            return t;
        }

        protected SqlTable GetTableFromDb(string name)
        {
            EnsureSetting();
            var driver = SqlDriverFactory.Get(setting.ProviderName);
            return driver.GetTable(name, setting.ConnectionString);
        }

        public void RefreshProcedure(string name)
        {
            SqlProcedure p = GetProcedureFromDb(name);
            if (p != null)
            {
                procedures.AddOrUpdate(name, p, (k, v) => v);
            }
        }

        public SqlProcedure GetProcedure(string name)
        {
            SqlProcedure p;
            if (procedures.TryGetValue(name, out p))
            {
                return p;
            }
            lock (syncLock)
            {
                if (procedures.TryGetValue(name, out p))
                {
                    return p;
                }

                p = GetProcedureFromDb(name);
                if (p != null)
                {
                    procedures.TryAdd(name, p);
                }
            }
            return p;
        }

        protected SqlProcedure GetProcedureFromDb(string name)
        {
            EnsureSetting();
            var driver = SqlDriverFactory.Get(setting.ProviderName);
            return driver.GetProcedure(name, setting.ConnectionString);
        }
    }

}
