using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kiss.Data.Expression
{
    /// <summary>
    /// sql delete expression
    /// </summary>
    [Serializable]
    public sealed class Delete : ISqlExpression
    {
        /// <summary>
        /// table
        /// </summary>
        public Table Table { get; set; }

        ///// <summary>
        ///// from
        ///// </summary>
        //public From From { get; set; }

        /// <summary>
        /// where clause
        /// </summary>
        public Where Where { get; set; }

        /// <summary>
        /// order by clause
        /// </summary>
        public OrderBy OrderBy { get; set; }

        /// <summary>
        /// limit count
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="tableName"></param>
        public Delete(string tableName) :
            this(new Table(tableName))
        {
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="table"></param>
        public Delete(Table table)
        {
            this.Table = table;
            this.Where = new Where();
            this.OrderBy = new OrderBy();

        }

        /// <summary>
        ///  rows count to update
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public Delete Limit(int count)
        {
            this.Count = count;
            return this;
        }

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("DELETE FROM {0} \r\n{1} \r\n{2} LIMIT {3} ", Table, Where, OrderBy, Count);
        }

        /// <summary>
        /// ISqlExpression.NodeType
        /// </summary>
        /// <returns></returns>
        NodeType ISqlExpression.NodeType()
        {
            return NodeType.Delete;
        }
    }
}
