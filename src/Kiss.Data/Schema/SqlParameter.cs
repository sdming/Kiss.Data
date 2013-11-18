using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kiss.Data;
using System.Data;

namespace Kiss.Data.Schema
{
    /// <summary>
    /// schema of parameter
    /// </summary>
    [Serializable]
    public class SqlParameter : SqlField
    {
          /// <summary>
        /// Ordinal
        /// </summary>
        public int Ordinal { get; set; }
        
        /// <summary>
        /// AllowDBNull
        /// </summary>
        public bool AllowDBNull { get; set; }
        
        /// <summary>
        /// direction
        /// </summary>
        public ParameterDirection Direction { get; set; }

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format(@"{{""Name"":""{0}"",""Ordinal"":{1},""Direction"":""{2}"",""AllowDBNull"":""{3}"",""DataTypeName"":""{4}"",""ProviderDbType"":""{5}"",""DbType"":""{6}"",""Precision"":""{7}"",""Scale"":""{8}"",""Size"":""{9}"" }}",
                Name, Ordinal, Direction, AllowDBNull, DataTypeName, ProviderDbType, DbType, Precision, Scale, Size);
        }
    }
}
