using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlTypes;

namespace Kiss.Data.Schema
{
    /// <summary>
    /// SqlColumn
    /// </summary>
    [Serializable]
    public class SqlColumn : SqlField
    {
        
        /// <summary>
        /// Ordinal
        /// </summary>
        public int Ordinal { get; set; }

        /// <summary>
        /// IsReadOnly
        /// </summary>
        public bool IsReadOnly { get; set; }

        /// <summary>
        /// IsAutoIncrement
        /// </summary>
        public bool IsAutoIncrement { get; set; }
        
        /// <summary>
        /// IsKey
        /// </summary>
        public bool IsKey { get; set; }

        /// <summary>
        /// AllowDBNull
        /// </summary>
        public bool AllowDBNull { get; set; }

        
        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format(@"{{""Name"":""{0}"",""Ordinal"":{1},""IsReadOnly"":""{2}"",""IsAutoIncrement"":""{3}"",""IsKey"":""{4}"",""AllowDBNull"":""{5}"",""DataTypeName"":""{6}"",""ProviderDbType"":""{7}"",""DbType"":""{8}"",""Precision"":""{9}"",""Scale"":""{10}"",""Size"":""{11}"" }}",
                Name, Ordinal, IsReadOnly, IsAutoIncrement, IsKey, AllowDBNull, DataTypeName, ProviderDbType, DbType, Precision, Scale, Size);            
        }
    }
}
