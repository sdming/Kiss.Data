//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Data;

//namespace Kiss.Data
//{
//    /// <summary>
//    /// SqlType
//    /// </summary>
//    [Serializable]
//    public struct SqlType
//    {
//        /// <summary>
//        /// DbType
//        /// </summary>
//        public DbType? DbType; 

//        /// <summary>
//        /// Precision
//        /// </summary>
//        public byte? Precision;

//        /// <summary>
//        /// Scale
//        /// </summary>
//        public byte? Scale;

//        /// <summary>
//        /// Size
//        /// </summary>
//        public int? Size;

//        /// <summary>
//        /// ProviderDbType
//        /// </summary>
//        public int? ProviderDbType;

//        /// <summary>
//        /// ToString
//        /// </summary>
//        /// <returns></returns>
//        public override string ToString()
//        {
//            return string.Format("DbType:{0},Precision:{1},Scale:{2},Size:{3}", DbType, Precision, Scale, Size);
//        }
//    }
//}
