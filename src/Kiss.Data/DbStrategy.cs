using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Kiss.Data
{
    public sealed class DbStrategy
    {
        /// <summary>
        /// return IDbSchemaProvider get database name
        /// </summary>
        public static Func<string, IDbSchemaProvider> SchemaProvider = SchemaProviderRepository.GetSchemaProvider;

        /// <summary>
        /// return ConnectionStringSettings according database logic name
        /// </summary>
        public static Func<string, ConnectionStringSettings> SettingsProvider = ConfigurationSettingProvider.GetConnectionStringSettings;


        //public static bool EnableRoute { get; set; }

        //public static Func<string, ConnectionStringSettings> DbFunc { get; set; }
        //public static Func<string, string> TableName { get; set; }
        //public static Func<Type, string> TypeName { get; set; }
    }
}
