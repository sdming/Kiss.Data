using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kiss.Data.Expression
{
    /// <summary>
    /// IRawSql
    /// </summary>
    public interface IRawSql
    {
        string ToSql();
    }
}
