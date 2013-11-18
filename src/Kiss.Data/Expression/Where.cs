using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kiss.Data.Expression
{
    /// <summary>
    /// sql where clause
    /// </summary>
    [Serializable]
    public sealed class Where : ISqlExpression
    {
        /// <summary>
        /// Conditions
        /// </summary>
        public ConditionList Conditions { get; set; }

        public Where()
        {
            this.Conditions = new ConditionList();
        }

        /// <summary>
        /// IsEmpty
        /// </summary>
        /// <returns></returns>
        internal bool IsEmpty()
        {
            return Conditions == null || Conditions.IsEmpty();
        }

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("WHERE \r\n{0}", Conditions);
        }

        /// <summary>
        /// ISqlExpression.NodeType
        /// </summary>
        /// <returns></returns>
        NodeType ISqlExpression.NodeType()
        {
            return NodeType.Where;
        }

        /// <summary>
        /// Append
        /// </summary>
        /// <param name="expression"></param>
        public Where Append(ISqlExpression expression)
        {
            Conditions.Append(expression);
            return this;
        }

        /// <summary>
        /// append (left op right)
        /// </summary>
        /// <param name="op"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public Where Condition(SqlOperator op, ISqlExpression left, ISqlExpression right)
        {
            Conditions.Condition(op, left, right);
            return this;
        }

        /// <summary>
        /// append 'and'
        /// </summary>
        public Where And()
        {
            Conditions.And();
            return this;
        }

        /// <summary>
        /// append 'or'
        /// </summary>
        public Where Or()
        {
            Conditions.Or();
            return this;
        }

        /// <summary>
        /// append '('
        /// </summary>
        public Where OpenParentheses()
        {
            Conditions.OpenParentheses();
            return this;
        }

        /// <summary>
        /// append ')'
        /// </summary>
        public Where CloseParentheses()
        {
            Conditions.CloseParentheses();
            return this;
        }

        /// <summary>
        /// append raw sql
        /// </summary>
        /// <param name="sql"></param>
        public Where Sql(string sql)
        {
            Conditions.Sql(sql);
            return this;
        }

        /// <summary>
        /// Exists
        /// </summary>
        /// <param name="expression"></param>
        public Where Exists(ISqlExpression expression)
        {
            Conditions.Exists(expression);
            return this;
        }

        /// <summary>
        /// NotExists
        /// </summary>
        /// <param name="expression"></param>
        public Where NotExists(ISqlExpression expression)
        {
            Conditions.NotExists(expression);
            return this;
        }

        /// <summary>
        /// Compare
        /// </summary>
        /// <param name="op"></param>
        /// <param name="column"></param>
        /// <param name="value"></param>
        public Where Compare(SqlOperator op, string column, object value)
        {
            Conditions.Compare(op, column, value);
            return this;
        }

        /// <summary>
        /// Like
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        public Where Like(string column, object value)
        {
            Conditions.Like(column, value);
            return this;
        }

        /// <summary>
        /// NotLike
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        public Where NotLike(string column, object value)
        {
            Conditions.NotLike(column, value);
            return this;
        }

        /// <summary>
        /// LessOrEquals
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        public Where LessOrEquals(string column, object value)
        {
            Conditions.LessOrEquals(column, value);
            return this;
        }

        /// <summary>
        /// LessThan
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        public Where LessThan(string column, object value)
        {
            Conditions.LessThan(column, value);
            return this;
        }

        /// <summary>
        /// GreaterOrEquals
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        public Where GreaterOrEquals(string column, object value)
        {
            Conditions.GreaterOrEquals(column, value);
            return this;
        }

        /// <summary>
        /// GreaterThan
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        public Where GreaterThan(string column, object value)
        {
            Conditions.GreaterThan(column, value);
            return this;
        }

        /// <summary>
        /// EqualsTo
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        public Where EqualsTo(string column, object value)
        {
            Conditions.EqualsTo(column, value);
            return this;
        }

        /// <summary>
        /// NotEquals
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        public Where NotEquals(string column, object value)
        {
            Conditions.NotEquals(column, value);
            return this;
        }

        /// <summary>
        /// IsNull
        /// </summary>
        /// <param name="column"></param>
        public Where IsNull(string column)
        {
            Conditions.IsNull(column);
            return this;
        }

        /// <summary>
        /// IsNotNull
        /// </summary>
        /// <param name="column"></param>
        public Where IsNotNull(string column)
        {
            Conditions.IsNotNull(column);
            return this;
        }

        /// <summary>
        /// In
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        public Where In(string column, object value)
        {
            Conditions.In(column, value);
            return this;
        }

        /// <summary>
        /// NotIn
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        public Where NotIn(string column, object value)
        {
            Conditions.NotIn(column, value);
            return this;
        }

    }
}
