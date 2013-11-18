using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kiss.Data.Expression
{
    /// <summary>
    /// assignment
    /// </summary>
    [Serializable]
    public struct Set : ISqlExpression
    {
        /// <summary>
        /// column
        /// </summary>
        public Column Column;

        /// <summary>
        /// value
        /// </summary>
        public ISqlExpression Value;

        /// <summary>
        /// Set
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        public Set(Column column, ISqlExpression value)
        {
            this.Column = column;
            this.Value = value;
        }

        /// <summary>
        /// Set
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="value"></param>
        public Set(string columnName, ISqlExpression value)
        {
            this.Column = new Column(columnName);
            this.Value = value;
        }

        /// <summary>
        /// ISqlExpression.NodeType
        /// </summary>
        /// <returns></returns>
        NodeType ISqlExpression.NodeType()
        {
            return NodeType.Set;
        }

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} = {1} ", Column, Value);
        }

    }
}
