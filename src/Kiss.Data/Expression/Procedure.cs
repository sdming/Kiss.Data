using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Kiss.Data.Expression
{
    /// <summary>
    /// store procedure command
    /// </summary>
    [Serializable]
    public sealed class Procedure : ISqlExpression
    {
        /// <summary>
        /// procedure name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// command parameters
        /// </summary>
        public List<Parameter> Parameters { get; set; }

        /// <summary>
        /// store procedure command
        /// </summary>
        /// <param name="commandText"></param>
        public Procedure(string procedureName)
        {
            this.Name = procedureName;
            this.Parameters = new List<Parameter>();
        }

        /// <summary>
        /// set a parameter
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        /// <returns></returns>
        public Procedure Set(string parameterName, object parameterValue)
        {
            return Set(new Parameter() { Name = parameterName, Value = parameterValue });
        }

        /// <summary>
        /// set a parameter
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        /// <returns></returns>
        public Procedure Set(string parameterName, object parameterValue, ParameterDirection direction)
        {
            return Set(new Parameter() { Name = parameterName, Value = parameterValue, Direction = direction });
        }

        /// <summary>
        /// append a paramter
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public Procedure Set(Parameter parameter)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException("parameter");
            }
            if (Parameters == null)
            {
                Parameters = new List<Parameter>();
            }
            Parameters.Add(parameter);
            return this;
        }

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{{name:{0}, parameters:{1} }}", Name, Utils.PrintList(Parameters));
        }

        /// <summary>
        /// ISqlExpression.NodeType
        /// </summary>
        /// <returns></returns>
        NodeType ISqlExpression.NodeType()
        {
            return NodeType.Procedure;
        }
    }
}
