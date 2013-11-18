using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kiss.Data.Expression
{
    /// <summary>
    /// MarkText
    /// </summary>
    [Serializable]
    public sealed class MarkText : ISqlExpression
    {
        /// <summary>
        /// command text
        /// </summary>
        public string CommandText { get; internal set; }

        /// <summary>
        /// command parameters
        /// </summary>
        public List<Parameter> Parameters { get; internal set; }

        /// <summary>
        /// TextCommand
        /// </summary>
        /// <param name="commandText"></param>
        public MarkText(string commandText)
        {
            this.CommandText = commandText;
            //this.Parameters = new List<Parameter>();
        }

        /// <summary>
        /// set a parameter
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        /// <returns></returns>
        public MarkText Set(string parameterName, object parameterValue)
        {
            return Set(new Parameter() { Name = parameterName, Value = parameterValue });
        }

        /// <summary>
        /// append a paramter
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public MarkText Set(Parameter parameter)
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
            return string.Format("{{text:{0}, parameters:{1} }}", CommandText, Utils.PrintList(Parameters));
        }

        /// <summary>
        /// ISqlExpression.NodeType
        /// </summary>
        /// <returns></returns>
        NodeType ISqlExpression.NodeType()
        {
            return NodeType.MarkText;
        }
    }
}
