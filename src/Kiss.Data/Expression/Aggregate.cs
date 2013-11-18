using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kiss.Data.Expression
{
    /// <summary>
    /// sql aggregate expression
    /// </summary>
    public sealed class Aggregate : ISqlExpression
    {
        /// <summary>
        /// aggregate function
        /// </summary>
        public Function Func { get; set; }

        /// <summary>
        /// exression
        /// </summary>
        public ISqlExpression Exression { get; set; }

        /// <summary>
        /// Aggregate
        /// </summary>
        public Aggregate(Function func, ISqlExpression exression)
        {
            this.Func = func;
            this.Exression = exression;
        }

        /// <summary>
        /// Aggregate
        /// </summary>
        public Aggregate()
        {
        }

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}({1}) ", Func, Exression);
        }

        /// <summary>
        /// ISqlExpression.NodeType
        /// </summary>
        /// <returns></returns>
        NodeType ISqlExpression.NodeType()
        {
            return NodeType.Aggregate;
        }

    }
}
