using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kiss.Data.Expression
{
    /// <summary>
    /// sql update expression
    /// </summary>
    [Serializable]
    public sealed class Update : ISqlExpression
    {
        /// <summary>
        /// table
        /// </summary>
        public Table Table { get; set; }

        /// <summary>
        /// assignments
        /// </summary>
        public List<Set> Sets { get; set; }

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
        /// Update
        /// </summary>
        /// <param name="tableName"></param>
        public Update(string tableName) :
            this(new Table(tableName))
        {
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="table"></param>
        public Update(Table table)
        {
            this.Table = table;
            this.Sets = new List<Set>();
            this.Where = new Where();
            this.OrderBy = new OrderBy();
        }

        /// <summary>
        /// append assignment
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Update Set(string columnName, object value)
        {
            return Set(new Set(columnName, value.AsExpression()));
        }

        /// <summary>
        /// append assignment
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        public Update Set(Set set)
        {
            Sets.Add(set);
            return this;
        }

        /// <summary>
        ///  rows count to update
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public Update Limit(int count)
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
            return string.Format("UPDATE {0} SET {1} \r\n{2} \r\n{3} \r\nLIMIT {4}", Table, Utils.PrintList(Sets), Where, OrderBy, Count);
        }

        /// <summary>
        /// ISqlExpression.NodeType
        /// </summary>
        /// <returns></returns>
        NodeType ISqlExpression.NodeType()
        {
            return NodeType.Update;
        }
    }
}
