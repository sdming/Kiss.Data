using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Oracle.DataAccess;
using Oracle.DataAccess.Client;

namespace Kiss.Data.Driver
{
    /// <summary>
    /// OracleSchema
    /// </summary>
    public class OracleSchema : SqlSchema
    {
        /// <summary>
        /// ProviderTypeToDbType
        /// </summary>
        /// <param name="providerType"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:丢失范围之前释放对象")]
        public override DbType ProviderTypeToDbType(int providerType)
        {
            var t = (OracleDbType)providerType;
            if (t == OracleDbType.Blob)
            {
                return DbType.Binary;
            }
            OracleParameter p = new OracleParameter();
            p.OracleDbType = t;
            return p.DbType;
        }
    }
}
