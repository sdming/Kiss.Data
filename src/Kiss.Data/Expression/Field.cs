using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kiss.Data.Expression
{
    /// <summary>
    /// field
    /// </summary>
    [Serializable]
    public class Field
    {
        /// <summary>
        /// Expression
        /// </summary>
        internal ISqlExpression Expression { get; set; }

        /// <summary>
        /// Alias
        /// </summary>
        internal string Alias { get; set; }

        /// <summary>
        /// Field
        /// </summary>
        public Field()
        {
        }

        /// <summary>
        /// Field
        /// </summary>
        public Field(ISqlExpression expression, string alias)
        {
            this.Expression = expression;
            this.Alias = alias;
        }

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (string.IsNullOrEmpty(Alias))
            {
                return string.Format("{0}", Expression);
            }
            return string.Format("{0} AS {1}", Expression, Alias);
        }

    }
}
