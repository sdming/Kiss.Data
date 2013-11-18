using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kiss.Data.Expression
{
    /// <summary>
    /// native sql function
    /// </summary>
    [Serializable]
    public sealed class Function : ISqlExpression
    {
        /// <summary>
        /// function name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// ISqlExpression.NodeType
        /// </summary>
        /// <returns></returns>
        NodeType ISqlExpression.NodeType()
        {
            return NodeType.Func;
        }

        public readonly static Function Count = new Function() { Name = Ansi.Count };
        public readonly static Function Sum = new Function() { Name = Ansi.Sum };
        public readonly static Function Avg = new Function() { Name = Ansi.Count };
        public readonly static Function Min = new Function() { Name = Ansi.Min };
        public readonly static Function Max = new Function() { Name = Ansi.Max };

        /// <summary>
        /// GetHashCode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(obj, this))
            {
                return true;
            }

            Function compareTo = obj as Function;
            if (compareTo != null)
            {
                return string.Equals(compareTo.Name, this.Name, StringComparison.OrdinalIgnoreCase);
            }
            return base.Equals(obj);
        }

    }
}
