using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kiss.Data.Expression
{
    /// <summary>
    /// sort direction
    /// </summary>
    [Serializable]
    public sealed class SortDirection : SqlFragment
    {
        public readonly static SortDirection Asc = new SortDirection() { Name = Ansi.Asc, Sql = Ansi.Asc };
        public readonly static SortDirection Desc = new SortDirection() { Name = Ansi.Desc, Sql = Ansi.Desc };
    }
}
