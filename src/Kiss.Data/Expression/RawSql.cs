using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kiss.Data.Expression
{
    /// <summary>
    /// RawSql
    /// </summary>
    [Serializable]
    public sealed class RawSql : ISqlExpression, IRawSql
    {
        /// <summary>
        /// sql string
        /// </summary>
        public string Sql { get; set; }

        /// <summary>
        /// RawSql
        /// </summary>
        /// <param name="sql"></param>
        public RawSql(string sql)
        {
            this.Sql = sql;
        }

        /// <summary>
        /// ISqlExpression.NodeType
        /// </summary>
        /// <returns></returns>
        NodeType ISqlExpression.NodeType()
        {
            return NodeType.Sql;
        }

        /// <summary>
        /// ToSql
        /// </summary>
        /// <returns></returns>
        public string ToSql()
        {
            return Sql;
        }

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            //return string.Format("{{sql:{0}}}", Sql);
            return "sql(" + Sql + ")";
        }

    }
}
