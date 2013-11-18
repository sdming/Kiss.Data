using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kiss.Data.Expression
{
    /// <summary>
    /// sql query expression
    /// </summary>
    [Serializable]
    public sealed class Query : ISqlExpression
    {
        /// <summary>
        /// select
        /// </summary>
        public Select Select { get; set; }

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
        /// group by clause
        /// </summary>
        public GroupBy GroupBy { get; set; }

        /// <summary>
        /// having by clause
        /// </summary>
        public Having Having { get; set; }

        /// <summary>
        /// order by clause
        /// </summary>
        public OrderBy OrderBy { get; set; }

        /// <summary>
        /// offset
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        /// limit count
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        ///  is distinct?
        /// </summary>
        public bool IsDistinct { get; set; }

        /// <summary>
        /// Query
        /// </summary>
        /// <param name="tableName"></param>
        public Query(string tableName) :
            this(new Table(tableName))
        {
        }


        /// <summary>
        /// Query
        /// </summary>
        /// <param name="table"></param>
        public Query(Table table)
        {
            this.Table = table;
            this.Select = new Select();
            this.Where = new Where();
            this.GroupBy = new GroupBy();
            this.Having = new Having();
            this.OrderBy = new OrderBy();

            this.Offset = -1;
            this.Count = -1;
        }

        /// <summary>
        /// limit rows to return
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public Query Limit(int offset, int count)
        {
            this.Count = count;
            this.Offset = offset;
            return this;
        }

        /// <summary>
        /// distinct result
        /// </summary>
        /// <returns></returns>
        public Query Distinct()
        {
            this.IsDistinct = true;
            return this;
        }

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string distinct = IsDistinct ? "distinct" : string.Empty;
            return string.Format("SELECT {0} {1} \r\nFROM {2} \r\n{3} \r\n{4} \r\n{5} \r\n{6} \r\nLIMIT {7}, {8} ", 
                distinct, Select, Table, Where, GroupBy, Having, OrderBy, Offset, Count);
        }

        /// <summary>
        /// ISqlExpression.NodeType
        /// </summary>
        /// <returns></returns>
        NodeType ISqlExpression.NodeType()
        {
            return NodeType.Query;
        }
    }
}
