using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kiss.Data.Expression
{
    /// <summary>
    /// ConditionList
    /// </summary>
    [Serializable]
    public class ConditionList
    {
        /// <summary>
        /// Conditions
        /// </summary>
        internal List<ISqlExpression> Conditions;

        /// <summary>
        /// needLogicOperator
        /// </summary>
        private bool needLogicOperator;

        /// <summary>
        /// whether is empty
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            return Conditions == null || Conditions.Count == 0;
        }

        /// <summary>
        /// ItemsCount
        /// </summary>
        public int ItemsCount()
        {
            if (Conditions == null)
            {
                return 0;
            }
            return Conditions.Count;
        }



        protected void Ensure()
        {
            if (this.Conditions == null)
            {
                this.Conditions = new List<ISqlExpression>();
            }
        }

        /// <summary>
        /// dump as string
        /// </summary>
        /// <returns></returns>
        internal string Dump(bool indent)
        {
            StringBuilder sb = new StringBuilder();
            var deep = 0;
            string delimiter = indent ? "\r\n" : " ";
            string indentChars = "\t";

            if (Conditions != null)
            {
                for (var i = 0; i < Conditions.Count; i++)
                {
                    var item = Conditions[i];
                    if (i > 0)
                    {
                        sb.Append(delimiter);
                    }
                    if (item == SqlOperator.CloseParentheses)
                    {
                        deep--;
                    }

                    if (deep > 0 && indent)
                    {
                        for (var index = 0; index < deep; index++)
                        {
                            sb.Append(indentChars);
                        }
                    }

                    sb.AppendFormat("{0}", item);
                    if (item == SqlOperator.OpenParentheses)
                    {
                        deep++;
                    }

                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Dump(true);
        }

        /// <summary>
        /// CheckLogicOperator
        /// </summary>
        internal void CheckLogicOperator()
        {
            if (needLogicOperator)
            {
                Ensure();
                Conditions.Add(SqlOperator.And);
            }
        }

        /// <summary>
        /// Append
        /// </summary>
        /// <param name="expression"></param>
        public void Append(ISqlExpression expression)
        {
            Ensure();
            CheckLogicOperator();
            Conditions.Add(expression);
            needLogicOperator = true;
        }

        /// <summary>
        /// append (left op right)
        /// </summary>
        /// <param name="op"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public void Condition(SqlOperator op, ISqlExpression left, ISqlExpression right)
        {
            Append(new Condition() { Left = left, Op = op, Right = right });
        }

        /// <summary>
        /// append 'and'
        /// </summary>
        public void And()
        {
            if (needLogicOperator)
            {
                Ensure();
                Conditions.Add(SqlOperator.And);
                needLogicOperator = false;
            }
        }

        /// <summary>
        /// append 'or'
        /// </summary>
        public void Or()
        {
            if (needLogicOperator)
            {
                Ensure();
                Conditions.Add(SqlOperator.Or);
                needLogicOperator = false;
            }
        }

        /// <summary>
        /// append '('
        /// </summary>
        public void OpenParentheses()
        {
            Ensure();
            CheckLogicOperator();
            Conditions.Add(SqlOperator.OpenParentheses);
            needLogicOperator = false;
        }

        /// <summary>
        /// append ')'
        /// </summary>
        public void CloseParentheses()
        {
            Ensure();
            Conditions.Add(SqlOperator.CloseParentheses);
            needLogicOperator = true;
        }

        /// <summary>
        /// append raw sql
        /// </summary>
        /// <param name="sql"></param>
        public void Sql(string sql)
        {
            Append(new RawSql(sql));
        }

        /// <summary>
        /// Exists
        /// </summary>
        /// <param name="expression"></param>
        public void Exists(ISqlExpression expression)
        {
            Condition(SqlOperator.Exists, null, expression);
        }

        /// <summary>
        /// NotExists
        /// </summary>
        /// <param name="expression"></param>
        public void NotExists(ISqlExpression expression)
        {
            Condition(SqlOperator.NotExists, null, expression);
        }

        /// <summary>
        /// Compare
        /// </summary>
        /// <param name="op"></param>
        /// <param name="column"></param>
        /// <param name="value"></param>
        public void Compare(SqlOperator op, string column, object value)
        {
            Condition(op, new Column(column), value.AsExpression());
        }

        /// <summary>
        /// Like
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        public void Like(string column, object value)
        {
            Condition(SqlOperator.Like, new Column(column), value.AsExpression());
        }

        /// <summary>
        /// NotLike
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        public void NotLike(string column, object value)
        {
            Condition(SqlOperator.NotLike, new Column(column), value.AsExpression());
        }

        /// <summary>
        /// LessOrEquals
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        public void LessOrEquals(string column, object value)
        {
            Condition(SqlOperator.LessOrEquals, new Column(column), value.AsExpression());
        }

        /// <summary>
        /// LessThan
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        public void LessThan(string column, object value)
        {
            Condition(SqlOperator.LessThan, new Column(column), value.AsExpression());
        }

        /// <summary>
        /// GreaterOrEquals
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        public void GreaterOrEquals(string column, object value)
        {
            Condition(SqlOperator.GreaterOrEquals, new Column(column), value.AsExpression());
        }

        /// <summary>
        /// GreaterThan
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        public void GreaterThan(string column, object value)
        {
            Condition(SqlOperator.GreaterThan, new Column(column), value.AsExpression());
        }

        /// <summary>
        /// EqualsTo
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        public void EqualsTo(string column, object value)
        {
            Condition(SqlOperator.EqualsTo, new Column(column), value.AsExpression());
        }

        /// <summary>
        /// NotEquals
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        public void NotEquals(string column, object value)
        {
            Condition(SqlOperator.NotEquals, new Column(column), value.AsExpression());
        }

        /// <summary>
        /// IsNull
        /// </summary>
        /// <param name="column"></param>
        public void IsNull(string column)
        {
            Condition(SqlOperator.IsNull, new Column(column), null);
        }

        /// <summary>
        /// IsNotNull
        /// </summary>
        /// <param name="column"></param>
        public void IsNotNull(string column)
        {
            Condition(SqlOperator.IsNotNull, new Column(column), null);
        }

        /// <summary>
        /// In
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        public void In(string column, object value)
        {
            Condition(SqlOperator.In, new Column(column), value.AsExpression());
        }

        /// <summary>
        /// NotIn
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        public void NotIn(string column, object value)
        {
            Condition(SqlOperator.NotIn, new Column(column), value.AsExpression());
        }
    }
}
