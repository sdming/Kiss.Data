using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Oracle.ManagedDataAccess;
using Oracle.ManagedDataAccess.Client;
using Kiss.Data.Schema;

namespace Kiss.Data.Driver
{
    /// <summary>
    /// OracleSchema
    /// </summary>
    public class OracleManagedSchema : SqlSchema
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
            OracleParameter p = new OracleParameter("p", t);
            return p.DbType;
        }

        ///// <summary>
        ///// SetColumn
        ///// </summary>
        ///// <param name="col"></param>
        //protected override void SetColumn(SqlColumn col)
        //{
        //    if(col.ProviderType == (int)OracleDbType.Decimal)
        //    {
        //        OracleDbType type = ConvertNumberToOraDbType((byte)col.Precision, (byte)col.Scale);
        //        col.DbType = ProviderTypeToDbType((int)type);
        //    }
        //}

        ///// <summary>
        ///// ConvertNumberToOraDbType
        ///// </summary>
        ///// <param name="precision"></param>
        ///// <param name="scale"></param>
        ///// <returns></returns>
        //private static OracleDbType ConvertNumberToOraDbType(int precision, int scale)
        //{
        //    if ((scale <= 0) && ((precision - scale) < 5))
        //    {
        //        return OracleDbType.Int16;
        //    }
        //    if ((scale <= 0) && ((precision - scale) < 10))
        //    {
        //        return OracleDbType.Int32;
        //    }
        //    if ((scale <= 0) && ((precision - scale) < 0x13))
        //    {
        //        return OracleDbType.Int64;
        //    }
        //    if ((precision < 8) && (((scale <= 0) && ((precision - scale) <= 0x26)) || ((scale > 0) && (scale <= 0x2c))))
        //    {
        //        return OracleDbType.Single;
        //    }
        //    if (precision < 0x10)
        //    {
        //        return OracleDbType.Double;
        //    }
        //    return  OracleDbType.Decimal;
        //}

    }
}
