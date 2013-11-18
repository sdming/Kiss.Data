using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Kiss.Data.Schema
{
    /// <summary>
    /// SqlField
    /// </summary>
    [Serializable]
    public abstract class SqlField
    {
        /// <summary>
        /// name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// DbType
        /// </summary>
        public DbType DbType;

        /// <summary>
        /// Precision
        /// </summary>
        public byte? Precision;

        /// <summary>
        /// Scale
        /// </summary>
        public byte? Scale;

        /// <summary>
        /// Size
        /// </summary>
        public int? Size;

        /// <summary>
        /// native data type name
        /// </summary>
        public string DataTypeName { get; set; }

        /// <summary>
        /// ProviderDbType
        /// </summary>
        public int? ProviderDbType { get; set; }

        ///// <summary>
        ///// RuntimeType
        ///// </summary>
        //public Type RuntimeType { get; set; }


    }
}
