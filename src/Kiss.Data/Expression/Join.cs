using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kiss.Data.Expression
{
    /// <summary>
    /// sql join clause
    /// </summary>
    [Serializable]
    public sealed class Join : ISqlExpression
    {
        /// <summary>
        /// join type
        /// </summary>
        public JoinType JoinType { get; set; }

        /// <summary>
        /// left expression
        /// </summary>
        public Table Left { get; set; }

        /// <summary>
        /// right expression
        /// </summary>
        public Table Right { get; set; }

        /// <summary>
        /// join conditions
        /// </summary>
        public ConditionList Conditions { get; set; }

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("JOIN {0} {1} {2} ON ({3})", Left, JoinType, Right, Conditions.Dump(false));
        }

        /// <summary>
        /// ISqlExpression.NodeType
        /// </summary>
        /// <returns></returns>
        NodeType ISqlExpression.NodeType()
        {
            return NodeType.Join;
        }

        /// <summary>
        /// Join
        /// </summary>
        public Join()
        {
            this.Conditions = new ConditionList();
        }

        /// <summary>
        /// Join
        /// </summary>
        /// <param name="joinType"></param>
        /// <param name="left"></param>
        /// <param name="leftAlias"></param>
        /// <param name="right"></param>
        /// <param name="rightAlias"></param>
        public Join(JoinType joinType, string left, string right)
            : this(joinType, left, null, right, null)
        {
        }

        /// <summary>
        /// Join
        /// </summary>
        /// <param name="joinType"></param>
        /// <param name="left"></param>
        /// <param name="leftAlias"></param>
        /// <param name="right"></param>
        /// <param name="rightAlias"></param>
        public Join(JoinType joinType, string left, string leftAlias, string right, string rightAlias)
            : this(joinType, new Table(left, leftAlias), new Table(left, leftAlias))
        {
        }

        /// <summary>
        /// Join
        /// </summary>
        /// <param name="joinType"></param>
        /// <param name="left"></param>
        /// <param name="leftAlias"></param>
        /// <param name="right"></param>
        /// <param name="rightAlias"></param>
        public Join(JoinType joinType, Table left, Table right)
        {
            this.JoinType = joinType;
            this.Left = left;
            this.Right = right;
            this.Conditions = new ConditionList();
        }

        /// <summary>
        /// leftColumn = rightColumn
        /// </summary>
        /// <param name="leftColumn"></param>
        /// <param name="rightColumn"></param>
        public void On(string leftColumn, string rightColumn)
        {
            Conditions.Condition(SqlOperator.EqualsTo, new Column(leftColumn), new Column(rightColumn));
        }

        /// <summary>
        /// leftColumn1 = rightColumn1 AND leftColumn2 = rightColumn2
        /// </summary>
        /// <param name="leftColumn1"></param>
        /// <param name="rightColumn1"></param>
        /// <param name="leftColumn2"></param>
        /// <param name="rightColumn2"></param>
        public void On(string leftColumn1, string rightColumn1, string leftColumn2, string rightColumn2)
        {
            Conditions.Condition(SqlOperator.EqualsTo, new Column(leftColumn1), new Column(rightColumn1));
            Conditions.Condition(SqlOperator.EqualsTo, new Column(leftColumn2), new Column(rightColumn2));
        }
    }
}
