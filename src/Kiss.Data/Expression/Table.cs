using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kiss.Data.Expression
{
    /// <summary>
    /// sql table
    /// </summary>
    [Serializable]
    public struct Table : ISqlExpression
    {
        /// <summary>
        /// Name
        /// </summary>
        public string Name;

        /// <summary>
        /// Alias
        /// </summary>
        public string Alias;

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (string.IsNullOrEmpty(Alias))
            {
                return string.Format("{0}", Name);
            }
            return string.Format("{0} AS {1}", Name, Alias);
        }

        /// <summary>
        /// ISqlExpression.NodeType
        /// </summary>
        /// <returns></returns>
        NodeType ISqlExpression.NodeType()
        {
            return NodeType.Table;
        }

        /// <summary>
        /// Table
        /// </summary>
        /// <param name="name"></param>
        public Table(string name)
            : this(name, null)
        {
        }

        /// <summary>
        /// Table
        /// </summary>
        /// <param name="name"></param>
        /// <param name="alias"></param>
        public Table(string name, string alias)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("table name can not be empty", "name");
            }
            Name = name;
            Alias = alias;
        }

        /// <summary>
        /// whether named with name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal bool Is(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            if (string.Equals(this.Alias, name, StringComparison.OrdinalIgnoreCase)
                ||
                string.Equals(this.Name, name, StringComparison.OrdinalIgnoreCase)
                )
            {
                return true;
            }

            return false;
        }

    }
}
