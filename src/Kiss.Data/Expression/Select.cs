using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kiss.Data.Expression
{
    /// <summary>
    /// sql select clause
    /// </summary>
    [Serializable]
    public sealed class Select : ISqlExpression
    {
        /// <summary>
        /// fields in select clause
        /// </summary>
        public List<Field> Fields { get; set; }

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (Fields != null && Fields.Count > 0)
            {
                for (var i = 0; i < Fields.Count; i++)
                {
                    if (i > 0)
                    {
                        sb.Append(", ");
                    }
                    sb.Append(Fields[i]);
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
            return NodeType.Select;
        }

        /// <summary>
        /// Select
        /// </summary>
        public Select()
        {
            this.Fields = new List<Field>();
        }

        /// <summary>
        /// append a field
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="alias"></param>
        internal void Append(ISqlExpression expression, string alias)
        {
            if (Fields == null)
            {
                Fields = new List<Field>();
            }
            Fields.Add(new Field(expression, alias));
        }

        /// <summary>
        /// append columns
        /// </summary>
        /// <param name="column"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public Select Column(string column, params string[] columns)
        {
            Append(new Column(column), null);
            if (columns != null && columns.Length > 0)
            {
                for (var i = 0; i < columns.Length; i++)
                {
                    Append(new Column(columns[i]), null);
                }
            }
            return this;
        }

        /// <summary>
        /// append a column with alias
        /// </summary>
        /// <param name="column"></param>
        /// <param name="alias"></param>
        /// <returns></returns>
        public Select ColumnAs(string column, string alias)
        {
            Append(new Column(column), alias);
            return this;
        }

        /// <summary>
        /// append *
        /// </summary>
        /// <returns></returns>
        public Select All()
        {
            Append(new RawSql(Ansi.WildcardAll), null);
            return this;
        }


        /// <summary>
        /// append a expression
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="alias"></param>
        /// <returns></returns>
        public Select Exp(ISqlExpression expression, string alias)
        {
            Append(expression, alias);
            return this;
        }


        /// <summary>
        /// append a aggregate
        /// </summary>
        /// <param name="aggregate"></param>
        /// <param name="expression"></param>
        /// <param name="alias"></param>
        /// <returns></returns>
        internal Select AppendAggregate(Function aggregate, ISqlExpression expression, string alias)
        {
            Append(new Aggregate(aggregate, expression), alias);
            return this;
        }

        /// <summary>
        /// append Avg()
        /// </summary>
        /// <param name="column"></param>
        /// <param name="alias"></param>
        /// <returns></returns>
        public Select Avg(string column, string alias)
        {
            return AppendAggregate(Function.Avg, new Column(column), alias);
        }

        /// <summary>
        /// append a count()
        /// </summary>
        /// <param name="column"></param>
        /// <param name="alias"></param>
        /// <returns></returns>
        public Select Count(string column, string alias)
        {
            if (string.IsNullOrWhiteSpace(column))
            {
                return AppendAggregate(Function.Count, new RawSql(Ansi.WildcardAll), alias);
            }

            return AppendAggregate(Function.Count, new Column(column), alias);
        }

        /// <summary>
        /// append a sum()
        /// </summary>
        /// <param name="column"></param>
        /// <param name="alias"></param>
        /// <returns></returns>
        public Select Sum(string column, string alias)
        {
            return AppendAggregate(Function.Sum, new Column(column), alias);
        }

        /// <summary>
        /// append a min()
        /// </summary>
        /// <param name="column"></param>
        /// <param name="alias"></param>
        /// <returns></returns>
        public Select Min(string column, string alias)
        {
            return AppendAggregate(Function.Min, new Column(column), alias);
        }

        /// <summary>
        /// append a max()
        /// </summary>
        /// <param name="column"></param>
        /// <param name="alias"></param>
        /// <returns></returns>
        public Select Max(string column, string alias)
        {
            return AppendAggregate(Function.Max, new Column(column), alias);
        }

    }
}
