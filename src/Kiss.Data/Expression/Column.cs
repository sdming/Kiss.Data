using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kiss.Data.Expression
{
    /// <summary>
    /// column
    /// </summary>
    [Serializable]
    public struct Column : ISqlExpression
    {
        /// <summary>
        /// column name, like: table.coumn, column, table.*, *
        /// </summary>
        public string Name;

        public Column(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// ISqlExpression.NodeType
        /// </summary>
        /// <returns></returns>
        NodeType ISqlExpression.NodeType()
        {
            return NodeType.Column;
        }

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }

        //// Split split column and return table and column
        //func (c Column) Split() (table string, column string) {
        //        s := string(c)
        //        if s == "" {
        //                return "", ""
        //        }
        //        i := strings.Index(s, ".")
        //        if i < 0 {
        //                return "", s
        //        }
        //        return s[0:i], s[i+1:]
        //}

    }
}
