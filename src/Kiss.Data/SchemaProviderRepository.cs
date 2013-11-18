using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Configuration;
using Kiss.Data.Schema;
using Kiss.Data.Driver;

namespace Kiss.Data
{

    /// <summary>
    /// SchemaProviderRepository
    /// </summary>
    public class SchemaProviderRepository
    {
        /// <summary>
        /// read schema from database
        /// </summary>
        public static bool AutoRefreshSchemaFromDb = true;

        private static ConcurrentDictionary<string, IDbSchemaProvider> providers = new ConcurrentDictionary<string, IDbSchemaProvider>(StringComparer.OrdinalIgnoreCase);

        private readonly static object syncLock = new object();

        /// <summary>
        /// register IDbSchemaProvider to logical name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="provider"></param>
        public static void Register(string name, IDbSchemaProvider provider)
        {
            
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("name");
            }
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            providers.AddOrUpdate(name, provider, (k, v) => v);
        }

        /// <summary>
        /// get IDbSchemaProvider according db logic name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IDbSchemaProvider GetSchemaProvider(string name)
        {
            IDbSchemaProvider p;
            if (providers.TryGetValue(name, out p))
            {
                return p;
            }
            if (!AutoRefreshSchemaFromDb)
            {
                return null;
            }

            lock (syncLock)
            {
                if (providers.TryGetValue(name, out p))
                {
                    return p;
                }
                p = new NativeSchemaProvider(name);
                Register(name, p);
                return p;
            }

        }

    }
}
