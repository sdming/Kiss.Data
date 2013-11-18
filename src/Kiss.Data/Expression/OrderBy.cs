using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kiss.Data.Expression
{
    /// <summary>
    /// sql order by clause
    /// </summary>
    [Serializable]
    public sealed class OrderBy : ISqlExpression
    {
        /// <summary>
        /// items in orderby clause
        /// </summary>
        internal List<OrderByField> Fields { get; set; }

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Fields == null || Fields.Count == 0)
            {
                return string.Empty;
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("ORDER BY ");
            for (var i = 0; i < Fields.Count; i++)
            {
                if (i > 0)
                {
                    sb.Append(", ");
                }
                sb.Append(Fields[i]);
            }

            return sb.ToString();
        }

        /// <summary>
        /// ISqlExpression.NodeType
        /// </summary>
        /// <returns></returns>
        NodeType ISqlExpression.NodeType()
        {
            return NodeType.OrderBy;
        }

        /// <summary>
        /// whether orderby has items
        /// </summary>
        /// <returns></returns>
        internal bool IsEmpty()
        {
            return Fields == null || Fields.Count == 0;
        }

        /// <summary>
        /// append a item
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public OrderBy By(SortDirection direction, ISqlExpression expression)
        {
            if (Fields == null)
            {
                Fields = new List<OrderByField>();
            }
            Fields.Add(new OrderByField(expression, direction));
            return this;
        }

        /// <summary>
        /// append columns as asc
        /// </summary>
        /// <param name="column"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public OrderBy Asc(string column, params string[] columns)
        {
            By(SortDirection.Asc, new Column(column));
            if (columns != null && columns.Length > 0)
            {
                for (var i = 0; i < columns.Length; i++)
                {
                    By(SortDirection.Asc, new Column(columns[i]));
                }
            }
            return this;
        }

        /// <summary>
        /// append columns as desc
        /// </summary>
        /// <param name="column"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public OrderBy Desc(string column, params string[] columns)
        {
            By(SortDirection.Desc, new Column(column));
            if (columns != null && columns.Length > 0)
            {
                for (var i = 0; i < columns.Length; i++)
                {
                    By(SortDirection.Desc, new Column(columns[i]));
                }
            }
            return this;
        }
    }
}
