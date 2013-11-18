using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Kiss.Data
{
    public static class ConfigurationSettingProvider
    {
        public static ConnectionStringSettings GetConnectionStringSettings(string name)
        {            
            return ConfigurationManager.ConnectionStrings[name];
        }
    }
}
