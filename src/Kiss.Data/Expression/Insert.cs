using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kiss.Data.Expression
{
    /// <summary>
    /// sql insert expression
    /// </summary>
    [Serializable]
    public sealed class Insert : ISqlExpression
    {
        /// <summary>
        /// Table
        /// </summary>
        public Table Table { get; set; }

        /// <summary>
        /// Assignments
        /// </summary>
        public List<Set> Sets { get; set; }

        /// <summary>
        /// Output
        /// </summary>
        public string Output { get; set; }

        /// <summary>
        /// Insert
        /// </summary>
        /// <param name="tableName"></param>
        public Insert(string tableName) :
            this(new Table(tableName))
        {
        }

        /// <summary>
        /// Insert
        /// </summary>
        /// <param name="table"></param>
        public Insert(Table table)
        {
            this.Table = table;
            this.Sets = new List<Set>();
        }

        /// <summary>
        /// append assignment
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Insert Set(string columnName, object value)
        {
            return Set(new Set(columnName, value.AsExpression()));
        }

        /// <summary>
        /// append assignment
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        public Insert Set(Set set)
        {
            Sets.Add(set);
            return this;
        }

        /// <summary>
        /// return inserted data
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public Insert Returning(string column)
        {
            this.Output = column;
            return this;
        }

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("insert {0} {1} ", Table, Utils.PrintList(Sets));
        }

        /// <summary>
        /// ISqlExpression.NodeType
        /// </summary>
        /// <returns></returns>
        NodeType ISqlExpression.NodeType()
        {
            return NodeType.Insert;
        }
    }
}
