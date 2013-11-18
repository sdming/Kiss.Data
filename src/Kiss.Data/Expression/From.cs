using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kiss.Data.Expression
{
    /// <summary>
    /// sql from clause
    /// </summary>
    [Serializable]
    public sealed class From : ISqlExpression
    {
        /// <summary>
        /// table
        /// </summary>
        public Table Table { get; set; }

        /// <summary>
        /// other tables
        /// </summary>
        public List<Table> Tables { get; set; }

        /// <summary>
        /// joins
        /// </summary>
        public List<Join> Joins { get; set; }

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("FROM ");
            sb.Append(Table);

            if (Tables != null)
            {
                for (var i = 0; i < Tables.Count; i++)
                {
                    sb.Append(", ");
                    sb.Append(Tables[i]);
                }
            }

            if (Joins != null)
            {
                for (var i = 0; i < Joins.Count; i++)
                {
                    if (Joins[i] != null)
                    {
                        sb.Append("\r\n");
                        sb.Append(Joins[i]);
                    }
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// ISqlExpression.NodeType
        /// </summary>
        /// <returns></returns>
        NodeType ISqlExpression.NodeType()
        {
            return NodeType.From;
        }

        /// <summary>
        /// then from a nother table
        /// </summary>
        /// <param name="table"></param>
        /// <param name="alias"></param>
        /// <returns></returns>
        public From ThenFrom(string table, string alias)
        {
            if (Tables == null)
            {
                Tables = new List<Table>();
            }
            Tables.Add(new Table(table, alias));
            return this;
        }

        
        /// <summary>
        /// append a join
        /// </summary>
        /// <param name="joinType"></param>
        /// <param name="toTable"></param>
        /// <param name="toTableAlias"></param>
        private Expression.Join AddJoin(JoinType joinType, string toTable, string toTableAlias)
        {
            var j = new Join(joinType, Table, new Table(toTable, toTableAlias));
            Join(j);
            return j;
        }

        /// <summary>
        /// append a join
        /// </summary>
        /// <param name="join"></param>
        /// <returns></returns>
        public From Join(Join join)
        {
            if (Joins == null)
            {
                Joins = new List<Expression.Join>();
            }
            Joins.Add(join);
            return this;
        }

        /// <summary>
        /// append a cross join
        /// </summary>
        /// <param name="toTable"></param>
        /// <param name="toTableAlias"></param>
        /// <returns></returns>
        public Join CrossJoin(string toTable, string toTableAlias)
        {
            return AddJoin(JoinType.Cross, toTable, toTableAlias);
        }

        /// <summary>
        /// append a inner join
        /// </summary>
        /// <param name="toTable"></param>
        /// <param name="toTableAlias"></param>
        /// <returns></returns>
        public Join InnerJoin(string toTable, string toTableAlias)
        {
            return AddJoin(JoinType.Inner, toTable, toTableAlias);
        }

        /// <summary>
        /// append a left join
        /// </summary>
        /// <param name="toTable"></param>
        /// <param name="toTableAlias"></param>
        /// <returns></returns>
        public Join LeftJoin(string toTable, string toTableAlias)
        {
            return AddJoin(JoinType.Left, toTable, toTableAlias);
        }

        /// <summary>
        /// append a right join
        /// </summary>
        /// <param name="toTable"></param>
        /// <param name="toTableAlias"></param>
        /// <returns></returns>
        public Join RightJoin(string toTable, string toTableAlias)
        {
            return AddJoin(JoinType.Right, toTable, toTableAlias);
        }

    }
}
