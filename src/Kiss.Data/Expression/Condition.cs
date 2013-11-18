using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kiss.Data.Expression
{
    /// <summary>
    /// condition
    /// </summary>
    [Serializable]
    public sealed class Condition : ISqlExpression
    {
        /// <summary>
        /// right
        /// </summary>
        public ISqlExpression Right { get; set; }

        /// <summary>
        /// left
        /// </summary>
        public ISqlExpression Left { get; set; }

        /// <summary>
        /// operator
        /// </summary>
        public SqlOperator Op { get; set; }

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Right == null && Left == null)
            {
                return string.Format("{0}", Op);
            }
            else if (Left == null)
            {
                return string.Format("{0}({1})", Op, Right);
            }
            else if (Right == null)
            {
                return string.Format("{0} {1}", Left, Op);
            }
            return string.Format("{0} {1} {2}", Left, Op, Right);
        }

        /// <summary>
        /// ISqlExpression.NodeType
        /// </summary>
        /// <returns></returns>
        NodeType ISqlExpression.NodeType()
        {
            return NodeType.Condition;
        }
    }
}
