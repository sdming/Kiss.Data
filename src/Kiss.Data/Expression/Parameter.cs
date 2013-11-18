using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Kiss.Data.Expression
{
    /// <summary>
    /// sql parameter
    /// </summary>
    [Serializable]
    public sealed class Parameter : ISqlExpression
    {
        /// <summary>
        /// parameter name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// parameter value
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// DbType
        /// </summary>
        public DbType? DbType;

        /// <summary>
        /// Precision
        /// </summary>
        public byte? Precision;

        /// <summary>
        /// Scale
        /// </summary>
        public byte? Scale;

        /// <summary>
        /// Size
        /// </summary>
        public int? Size;

        /// <summary>
        /// ProviderDbType
        /// </summary>
        public int? ProviderDbType;

        /// <summary>
        /// parameter direction
        /// </summary>
        public ParameterDirection Direction { get; set; }

        ///// <summary>
        ///// Source
        ///// </summary>
        //public String Source { get; set; } 

        /// <summary>
        /// Parameter
        /// </summary>
        public Parameter()
        {
        }

        /// <summary>
        /// Parameter
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public Parameter(string name, object value)
        {
            this.Name = name;
            this.Value = value;
        }

        /// <summary>
        /// Parameter
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        public Parameter(string name, object value, ParameterDirection direction)
        {
            this.Name = name;
            this.Value = value;
            this.Direction = direction;
        }

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{{name:{0}, value:{1}, direction:{2} }}", Name, Value, Direction);
        }

        /// <summary>
        /// whether input parameter
        /// </summary>
        /// <returns></returns>
        public bool IsIn()
        {
            return Direction == ParameterDirection.Input || Direction == ParameterDirection.InputOutput;
        }

        /// <summary>
        /// whether output parameter
        /// </summary>
        /// <returns></returns>
        public bool IsOut()
        {
            return Direction == ParameterDirection.Output || Direction == ParameterDirection.InputOutput;
        }

        /// <summary>
        /// ISqlExpression.NodeType
        /// </summary>
        /// <returns></returns>
        NodeType ISqlExpression.NodeType()
        {
            return NodeType.Parameter;
        }
    }

}
