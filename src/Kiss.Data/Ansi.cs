using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kiss.Data
{
    /// <summary>
    /// Ansi sql key worlds
    /// </summary>
    internal static class Ansi
    {
        public const string Split = ".";
        public const string StatementSplit = ";";
        public const string WildcardAll = "*";
        public const string WildcardAny = "%";
        public const string WildcardOne = "_";
        public const string Blank = " ";
        public const string Comma = ",";
        public const string LineBreak = "\n";

        public const string Select = "SELECT";
        public const string Top = "TOP";
        public const string Distinct = "DISTINCT";
        public const string From = "FROM";
        public const string Where = "WHERE";
        public const string GroupBy = "GROUP BY";
        public const string Having = "HAVING";
        public const string OrderBy = "ORDER BY";
        public const string Asc = "ASC";
        public const string Desc = "DESC";
        public const string Limit = "LIMIT";
        public const string Insert = "INSERT";
        public const string InsertInto = "INSERT INTO";
        public const string Values = "VALUES";
        public const string Update = "UPDATE";
        public const string Set = "SET";
        public const string Delete = "DELETE";
        public const string Output = "OUTPUT";
        public const string Using = "USING";

        public const string Join = "JOIN";
        public const string As = "AS";
        public const string On = "ON";
        public const string CrossJoin = "CROSS JOIN";
        public const string FullJoin = "FULL JOIN";
        public const string InnerJoin = "INNER JOIN";
        public const string OuterJoin = "OUTER JOIN";
        public const string LeftJoin = "LEFT JOIN";
        public const string RightJoin = "RIGHT JOIN";

        public const string And = "AND";
        public const string Or = "OR";
        public const string OpenParentheses = "(";
        public const string CloseParentheses = ")";
        public const string Null = "NULL";
        public const string IsNull = "IS NULL";
        public const string IsNotNull = "IS NOT NULL";
        public const string Is = "IS";
        public const string IsNot = "IS NOT";
        public const string LessThan = "<";
        public const string LessOrEquals = "<=";
        public const string GreaterThan = ">";
        public const string GreaterOrEquals = ">=";
        public const string EqualsTo = "=";
        public const string NotEquals = "<>";
        public const string Between = "BETWEEN";
        public const string Like = "LIKE";
        public const string NotLike = "NOT LIKE";
        public const string In = "IN";
        public const string NotIn = "NOT IN";
        public const string All = "ALL";
        public const string Some = "SOME";
        public const string Any = "ANY";
        public const string Exists = "EXISTS";
        public const string NotExists = "NOT EXISTS";

        public const string Count = "COUNT";
        public const string Sum = "SUM";
        public const string Avg = "AVG";
        public const string Min = "MIN";
        public const string Max = "MAX";

        public const string BeginTran = "BEGIN TRAN";
        public const string Commit = "COMMIT";
        public const string Rollback = "ROLLBACK";
    }

}
