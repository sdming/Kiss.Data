using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kiss.Data.Expression
{
    /// <summary>
    /// sql group by clause
    /// </summary>
    [Serializable]
    public sealed class GroupBy : ISqlExpression
    {
        public List<ISqlExpression> Fields { get; set; }

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("GROUP BY ");
            if (Fields != null)
            {
                for (var i = 0; i < Fields.Count; i++)
                {
                    if (i > 0)
                    {
                        sb.Append(", ");
                    }
                    sb.Append(Fields[i]);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// ISqlExpression.NodeType
        /// </summary>
        /// <returns></returns>
        NodeType ISqlExpression.NodeType()
        {
            return NodeType.GroupBy;
        }

        /// <summary>
        /// append a expression
        /// </summary>
        /// <param name="expression"></param>
        internal void Append(ISqlExpression expression)
        {
            if (Fields == null)
            {
                Fields = new List<ISqlExpression>();
            }
            Fields.Add(expression);
        }

        /// <summary>
        /// append a expression
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public GroupBy By(ISqlExpression expression)
        {
            Append(expression);
            return this;
        }

        /// <summary>
        /// append columns
        /// </summary>
        /// <param name="column"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public GroupBy Columns(string column, params string[] columns)
        {
            Append(new Column(column));
            if (columns != null)
            {
                for (var i = 0; i < columns.Length; i++)
                {
                    Append(new Column(columns[i]));
                }
            }
            return this;
        }

        /// <summary>
        /// whether empty
        /// </summary>
        /// <returns></returns>
        internal bool IsEmpty()
        {
            return Fields == null || Fields.Count == 0;
        }
    }
}
