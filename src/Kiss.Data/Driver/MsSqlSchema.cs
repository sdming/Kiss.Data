using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Kiss.Data.Driver
{
    /// <summary>
    /// MsSqlSchema
    /// </summary>
    public class MsSqlSchema : SqlSchema
    {
        /// <summary>
        /// ProviderTypeToDbType
        /// </summary>
        /// <param name="providerType"></param>
        /// <returns></returns>
        public override DbType ProviderTypeToDbType(int providerType)
        {
            System.Data.SqlClient.SqlParameter p = new System.Data.SqlClient.SqlParameter();
            p.SqlDbType = (SqlDbType)providerType;
            return p.DbType;
        }
    }
}
