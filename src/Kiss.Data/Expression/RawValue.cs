using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kiss.Data.Expression
{
    /// <summary>
    /// value
    /// </summary>
    [Serializable]
    public struct RawValue : ISqlExpression
    {
        /// <summary>
        /// Value
        /// </summary>
        public object Value;

        /// <summary>
        /// Value
        /// </summary>
        /// <param name="value"></param>
        public RawValue(object value)
        {
            this.Value = value;
        }

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            //return string.Format("{{value:{0}}}", Value);
            return string.Format("{0}", Utils.Print(Value));
        }

        /// <summary>
        /// ISqlExpression.NodeType
        /// </summary>
        /// <returns></returns>
        NodeType ISqlExpression.NodeType()
        {
            return NodeType.Value;
        }
    }
}
