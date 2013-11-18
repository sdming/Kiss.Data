using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kiss.Data.Expression
{
    /// <summary>
    /// native sql operator
    /// </summary>
    [Serializable]
    public sealed class SqlOperator : SqlFragment, ISqlExpression
    {
        /// <summary>
        /// ISqlExpression.NodeType
        /// </summary>
        /// <returns></returns>
        NodeType ISqlExpression.NodeType()
        {
            return NodeType.Operator;
        }

        public static readonly SqlOperator IsNull = new SqlOperator() { Name = Ansi.IsNull, Sql = Ansi.IsNull };
        public static readonly SqlOperator IsNotNull = new SqlOperator() { Name = Ansi.IsNotNull, Sql = Ansi.IsNotNull };
        public static readonly SqlOperator LessThan = new SqlOperator() { Name = Ansi.LessThan, Sql = Ansi.LessThan };
        public static readonly SqlOperator LessOrEquals = new SqlOperator() { Name = Ansi.LessOrEquals, Sql = Ansi.LessOrEquals };
        public static readonly SqlOperator GreaterThan = new SqlOperator() { Name = Ansi.GreaterThan, Sql = Ansi.GreaterThan };
        public static readonly SqlOperator GreaterOrEquals = new SqlOperator() { Name = Ansi.GreaterOrEquals, Sql = Ansi.GreaterOrEquals };
        public static readonly SqlOperator EqualsTo = new SqlOperator() { Name = Ansi.EqualsTo, Sql = Ansi.EqualsTo };
        public static readonly SqlOperator NotEquals = new SqlOperator() { Name = Ansi.NotEquals, Sql = Ansi.NotEquals };
        public static readonly SqlOperator Like = new SqlOperator() { Name = Ansi.Like, Sql = Ansi.Like };
        public static readonly SqlOperator NotLike = new SqlOperator() { Name = Ansi.NotLike, Sql = Ansi.NotLike };
        public static readonly SqlOperator In = new SqlOperator() { Name = Ansi.In, Sql = Ansi.In };
        public static readonly SqlOperator NotIn = new SqlOperator() { Name = Ansi.NotIn, Sql = Ansi.NotIn };
        public static readonly SqlOperator Exists = new SqlOperator() { Name = Ansi.Exists, Sql = Ansi.Exists };
        public static readonly SqlOperator NotExists = new SqlOperator() { Name = Ansi.NotExists, Sql = Ansi.NotExists };
        public static readonly SqlOperator All = new SqlOperator() { Name = Ansi.All, Sql = Ansi.All };
        public static readonly SqlOperator Some = new SqlOperator() { Name = Ansi.Some, Sql = Ansi.Some };
        public static readonly SqlOperator Any = new SqlOperator() { Name = Ansi.Any, Sql = Ansi.Any };
        public static readonly SqlOperator And = new SqlOperator() { Name = Ansi.And, Sql = Ansi.And };
        public static readonly SqlOperator Or = new SqlOperator() { Name = Ansi.Or, Sql = Ansi.Or };
        public static readonly SqlOperator OpenParentheses = new SqlOperator() { Name = Ansi.OpenParentheses, Sql = Ansi.OpenParentheses };
        public static readonly SqlOperator CloseParentheses = new SqlOperator() { Name = Ansi.CloseParentheses, Sql = Ansi.CloseParentheses };
    }

}
