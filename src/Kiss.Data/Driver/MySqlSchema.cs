using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MySql.Data.MySqlClient;

namespace Kiss.Data.Driver
{
    /// <summary>
    /// MySqlSchema
    /// </summary>
    public class MySqlSchema : SqlSchema
    {
        /// <summary>
        /// ProviderTypeToDbType
        /// </summary>
        /// <param name="providerType"></param>
        /// <returns></returns>
        public override DbType ProviderTypeToDbType(int providerType)
        {
            var t = (MySqlDbType)providerType;
            if(t == MySqlDbType.Binary || t == MySqlDbType.VarBinary)
            {
                return DbType.Binary;
            }
            
            MySqlParameter p = new MySqlParameter();
            p.MySqlDbType = (MySqlDbType)providerType;
            return p.DbType;
        }
    }
}
