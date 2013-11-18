using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kiss.Data.Expression
{
    /// <summary>
    /// sql expression node type
    /// </summary>
    public enum NodeType
    {
        //Zero        = 0,
        Text = 1,
        Procedure = 2,
        Insert = 3,
        Query = 4,
        Update = 5,
        Delete = 6,
        MarkText = 101,

        Null = 11,
        Value = 12,
        Sql = 13,

        Table = 31,
        Column = 32,
        Alias = 33,
        Condition = 34,
        Set = 35,
        Aggregate = 36,

        Select = 41,
        From = 42,
        Join = 43,
        Where = 44,
        GroupBy = 45,
        Having = 46,
        OrderBy = 47,
        Output = 48,

        Operator = 61,
        Func = 62,
        Parameter = 63,
    }
}
