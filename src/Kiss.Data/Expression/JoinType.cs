using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kiss.Data.Expression
{
    /// <summary>
    /// sql join
    /// </summary>
    [Serializable]
    public sealed class JoinType : SqlFragment
    {
        public readonly static JoinType Cross = new JoinType() { Name = Ansi.CrossJoin, Sql = Ansi.CrossJoin };
        public readonly static JoinType Inner = new JoinType() { Name = Ansi.InnerJoin, Sql = Ansi.InnerJoin };
        public readonly static JoinType Left = new JoinType() { Name = Ansi.LeftJoin, Sql = Ansi.LeftJoin };
        public readonly static JoinType Right = new JoinType() { Name = Ansi.RightJoin, Sql = Ansi.RightJoin };
    }
}
