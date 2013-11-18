using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kiss.Data.Expression
{
    /// <summary>
    /// sql having clause
    /// </summary>
    [Serializable]
    public sealed class Having : ISqlExpression
    {
        /// <summary>
        /// Conditions
        /// </summary>
        public ConditionList Conditions { get; set; }

        public Having()
        {
            this.Conditions = new ConditionList();
        }

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("HAVING \r\n{0}", Conditions);
        }

        /// <summary>
        /// ISqlExpression.NodeType
        /// </summary>
        /// <returns></returns>
        NodeType ISqlExpression.NodeType()
        {
            return NodeType.Having;
        }

        /// <summary>
        /// Append
        /// </summary>
        /// <param name="expression"></param>
        public Having Append(ISqlExpression expression)
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
        public Having Condition(SqlOperator op, ISqlExpression left, ISqlExpression right)
        {
            Conditions.Condition(op, left, right);
            return this;
        }

        /// <summary>
        /// append 'and'
        /// </summary>
        public Having And()
        {
            Conditions.And();
            return this;
        }

        /// <summary>
        /// append 'or'
        /// </summary>
        public Having Or()
        {
            Conditions.Or();
            return this;
        }

        /// <summary>
        /// append '('
        /// </summary>
        public Having OpenParentheses()
        {
            Conditions.OpenParentheses();
            return this;
        }

        /// <summary>
        /// append ')'
        /// </summary>
        public Having CloseParentheses()
        {
            Conditions.CloseParentheses();
            return this;
        }

        /// <summary>
        /// append raw sql
        /// </summary>
        /// <param name="sql"></param>
        public Having Sql(string sql)
        {
            Conditions.Sql(sql);
            return this;
        }

        /// <summary>
        /// Exists
        /// </summary>
        /// <param name="expression"></param>
        public Having Exists(ISqlExpression expression)
        {
            Conditions.Exists(expression);
            return this;
        }

        /// <summary>
        /// NotExists
        /// </summary>
        /// <param name="expression"></param>
        public Having NotExists(ISqlExpression expression)
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
        public Having Compare(SqlOperator op, string column, object value)
        {
            Conditions.Compare(op, column, value);
            return this;
        }

        /// <summary>
        /// Like
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        public Having Like(string column, object value)
        {
            Conditions.Like(column, value);
            return this;
        }

        /// <summary>
        /// NotLike
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        public Having NotLike(string column, object value)
        {
            Conditions.NotLike(column, value);
            return this;
        }

        /// <summary>
        /// LessOrEquals
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        public Having LessOrEquals(string column, object value)
        {
            Conditions.LessOrEquals(column, value);
            return this;
        }

        /// <summary>
        /// LessThan
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        public Having LessThan(string column, object value)
        {
            Conditions.LessThan(column, value);
            return this;
        }

        /// <summary>
        /// GreaterOrEquals
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        public Having GreaterOrEquals(string column, object value)
        {
            Conditions.GreaterOrEquals(column, value);
            return this;
        }

        /// <summary>
        /// GreaterThan
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        public Having GreaterThan(string column, object value)
        {
            Conditions.GreaterThan(column, value);
            return this;
        }

        /// <summary>
        /// EqualsTo
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        public Having EqualsTo(string column, object value)
        {
            Conditions.EqualsTo(column, value);
            return this;
        }

        /// <summary>
        /// NotEquals
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        public Having NotEquals(string column, object value)
        {
            Conditions.NotEquals(column, value);
            return this;
        }

        /// <summary>
        /// IsNull
        /// </summary>
        /// <param name="column"></param>
        public Having IsNull(string column)
        {
            Conditions.IsNull(column);
            return this;
        }

        /// <summary>
        /// IsNotNull
        /// </summary>
        /// <param name="column"></param>
        public Having IsNotNull(string column)
        {
            Conditions.IsNotNull(column);
            return this;
        }

        /// <summary>
        /// In
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        public Having In(string column, object value)
        {
            Conditions.In(column, value);
            return this;
        }

        /// <summary>
        /// NotIn
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        public Having NotIn(string column, object value)
        {
            Conditions.NotIn(column, value);
            return this;
        }

        /// <summary>
        /// append a aggregate
        /// </summary>
        /// <param name="op"></param>
        /// <param name="func"></param>
        /// <param name="column"></param>
        /// <param name="value"></param>
        internal void AppendAggregate(SqlOperator op, Function func, string column, ISqlExpression value)
        {
            Condition(op, new Aggregate() { Func = func, Exression = new Column(column) }, value);
        }

        /// <summary>
        /// append avg(column) op value
        /// </summary>
        /// <param name="op"></param>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Having Avg(SqlOperator op, string column, ISqlExpression value)
        {
            AppendAggregate(op, Function.Avg, column, value);
            return this;
        }

        /// <summary>
        /// append avg(column) op value
        /// </summary>
        /// <param name="op"></param>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Having Avg(SqlOperator op, string column, object value)
        {
            AppendAggregate(op, Function.Avg, column, value.AsExpression());
            return this;
        }

        /// <summary>
        /// append count(column) op value
        /// </summary>
        /// <param name="op"></param>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Having Count(SqlOperator op, string column, ISqlExpression value)
        {
            AppendAggregate(op, Function.Count, column, value);
            return this;
        }

        /// <summary>
        /// append count(column) op value
        /// </summary>
        /// <param name="op"></param>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Having Count(SqlOperator op, string column, object value)
        {
            AppendAggregate(op, Function.Count, column, value.AsExpression());
            return this;
        }

        /// <summary>
        /// append sum(column) op value
        /// </summary>
        /// <param name="op"></param>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Having Sum(SqlOperator op, string column, ISqlExpression value)
        {
            AppendAggregate(op, Function.Sum, column, value);
            return this;
        }

        /// <summary>
        /// append sum(column) op value
        /// </summary>
        /// <param name="op"></param>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Having Sum(SqlOperator op, string column, object value)
        {
            AppendAggregate(op, Function.Sum, column, value.AsExpression());
            return this;
        }

        /// <summary>
        /// append min(column) op value
        /// </summary>
        /// <param name="op"></param>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Having Min(SqlOperator op, string column, ISqlExpression value)
        {
            AppendAggregate(op, Function.Min, column, value);
            return this;
        }

        /// <summary>
        /// append min(column) op value
        /// </summary>
        /// <param name="op"></param>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Having Min(SqlOperator op, string column, object value)
        {
            AppendAggregate(op, Function.Min, column, value.AsExpression());
            return this;
        }

        /// <summary>
        /// append max(column) op value
        /// </summary>
        /// <param name="op"></param>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Having Max(SqlOperator op, string column, ISqlExpression value)
        {
            AppendAggregate(op, Function.Max, column, value);
            return this;
        }

        /// <summary>
        /// append max(column) op value
        /// </summary>
        /// <param name="op"></param>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Having Max(SqlOperator op, string column, object value)
        {
            AppendAggregate(op, Function.Max, column, value.AsExpression());
            return this;
        }
    }
}
