using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kiss.Data.Expression
{
    /// <summary>
    /// orderby item
    /// </summary>
    [Serializable]
    internal class OrderByField
    {
        /// <summary>
        /// Expression
        /// </summary>
        internal ISqlExpression Expression { get; set; }

        /// <summary>
        /// Direction
        /// </summary>
        internal SortDirection Direction { get; set; }

        /// <summary>
        /// OrderByField
        /// </summary>
        public OrderByField()
        {
        }

        /// <summary>
        /// OrderByField
        /// </summary>
        public OrderByField(ISqlExpression expression, SortDirection direction)
        {
            this.Expression = expression;
            this.Direction = direction;
        }


        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} {1}", Expression, Direction);
        }

    }
}
