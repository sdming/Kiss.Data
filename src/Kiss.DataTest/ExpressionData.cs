using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kiss.Data.Expression;

namespace Kiss.DataTest
{
    internal class ExpressionData
    {
        Dictionary<string, object> dataTypeMap = BuildDataTypeMap();

        public static Dictionary<string, object> BuildDataTypeMap()
        {
            Dictionary<string, object> m = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
            m["cbool"] = true;
            m["cint"] = 42;
            m["cfloat"] = 3.14;
            m["cnumeric"] = 1.1;
            m["cstring"] = "string";
            m["cdatetime"] = DateTime.Parse("2004-07-24");
            m["cbool"] = true;
            m["cguid"] = Guid.Parse("550e8400-e29b-41d4-a716-446655440000");
            return m;
        }

        public void SetOrderBy(OrderBy od)
        {
            od.Asc("cint", "cfloat");
            od.Desc("cnumeric", "cstring");
            od.By(SortDirection.Asc, new Column("cdatetime"));
        }

        public static Where BuildSimpleWhere()
        {
            return new Where()
                .EqualsTo("cbool", true)
                .LessOrEquals("cint", 42)
                .GreaterThan("cfloat", 3.14)
                .GreaterOrEquals("cnumeric", 1.1)
                .Like("cstring", "like")
                .GreaterThan("cdatetime", "2001-01-01")
                .IsNull("cbytes");
        }

        public void SetWhere(Where w)
        {
            w.EqualsTo("cbool", true).
                NotEquals("cbool", false);

            w.LessThan("cstring", "LessThan").
                LessOrEquals("cstring", "LessOrEquals").
                GreaterThan("cstring", "GreaterThan").
                GreaterOrEquals("cstring", "GreaterOrEquals").
                EqualsTo("cstring", "Equals").
                NotEquals("cstring", "NotEquals").
                In("cstring", new string[]{"a", "b", "c"}).
                NotIn("cstring", new string[]{"h", "i", "j"}).
                Like("cstring", "%like%").
                NotLike("cstring", "%NotLike%");

            w.LessThan("cint", 100).
                LessOrEquals("cint", 101).
                GreaterThan("cint", 200).
                GreaterOrEquals("cint", 201).
                EqualsTo("cint", 300).
                NotEquals("cint", 301).
                In("cint", new int[]{0, 1, 2, 3, 4}).
                NotIn("cint", new int[]{5, 6, 7, 8, 9});

            w.LessThan("cfloat", 1.01).
                LessOrEquals("cfloat", 1.02).
                GreaterThan("cfloat", 2.01).
                GreaterOrEquals("cfloat", 2.02).
                EqualsTo("cfloat", 3.01).
                NotEquals("cfloat", 3.02).
                In("cfloat", new double[]{10.01, 11.01, 12.01, 13.01, 14.01}).
                NotIn("cfloat", new double[]{15.01, 16.01, 17.01, 18.01, 19.01});

            w.LessThan("cnumeric", 1.1).
                LessOrEquals("cnumeric", 1.2).
                GreaterThan("cnumeric", 2.1).
                GreaterOrEquals("cnumeric", 2.2).
                EqualsTo("cnumeric", 3.1).
                NotEquals("cnumeric", 3.2).
                In("cnumeric", new object[]{10.1, 11.1, 12.1, 13.1, 14.1}).
                NotIn("cnumeric", new object[]{15.1, 16.1, 17.1, 18.1, 19.1});

            w.LessThan("cdatetime", "2001-01-01").
                LessOrEquals("cdatetime", "2001-01-02").
                GreaterThan("cdatetime", "2001-02-01").
                GreaterOrEquals("cdatetime", "2001-02-02").
                EqualsTo("cdatetime", DateTime.Parse("2001-03-01")).
                NotEquals("cdatetime", DateTime.Parse("2001-03-02")).
                In("cdatetime", new string[]{"2001-04-01 01:01:01", "2001-04-02 02:02:02"}).
                NotIn("cdatetime", new DateTime[]{DateTime.Parse("2001-04-01 01:01:01"),DateTime.Parse("2001-04-02 02:02:02")});

            w.EqualsTo("cguid", "550e8400-e29b-41d4-a716-446655440000").
                NotEquals("cguid", "550e8400-e29b-41d4-a716-446655440000");

            w.OpenParentheses().
                    OpenParentheses().
                    IsNull("cbytes").
                    Or().
                    IsNotNull("cbytes").
                    CloseParentheses().
                    Or().
                    OpenParentheses().
                    Sql(" 1!=2 ").
                    Exists(new RawSql("select count(*) from ttable where cint > 1")).
                    NotExists(new RawSql("select count(*) from ttable where cint > 10000")).
                    In("cint", new RawSql("select cint from ttable")).
                    NotIn("cint", new RawSql("select cint from ttable")).
                    CloseParentheses().
                    CloseParentheses();
        }

        public Query BuildSimpileQuery()
        {
            var q = new Query("ttable");
            
            q.Select.Column("cbool", "cint").
                Column("cfloat", "cdatetime").
                ColumnAs("cnumeric", "a_cnumeric").
                ColumnAs("cstring", "a_cstring").
                Column("cdatetime", "cguid");

            q.Where
                .EqualsTo("cbool", true)
                .LessOrEquals("cint", 42)
                .GreaterThan("cfloat", 3.14)
                .GreaterOrEquals("cnumeric",1.1)
                .Like("cstring", "like")
                .GreaterThan("cdatetime", "2001-01-01")
                .IsNull("cbytes");

            q.OrderBy
                .Asc("cint", "cfloat")
                .Desc("cnumeric", "cstring");
               
            return q;
        }

        public Query BuildQuery()
        {
            var q = new Query("ttable");
            q.Distinct();

            q.Select.Column("cbool", "cint").
                Column("cfloat", "cdatetime").
                ColumnAs("cnumeric", "a_cnumeric").
                ColumnAs("cstring", "a_cstring").
                Avg("cint", "avg_cint").
                Count("cint", "count_cint").
                Sum("cint", "sum_cint").
                Min("cint", "min_cint").
                Max("cint", "max_cint").
                Exp(new RawSql("cint - 1"), "exp_cint");

            //q.From.CrossJoin("ttable_c", "t_c").Equals("t1.cint", new Column("t_c.c_int"))
            //q.From.InnerJoin("ttable_c", "t_i").On("t1.cstring", "t_i.c_string")
            //q.From.LeftJoin("ttable_c", "t_l").On("t1.cstring", "t_l.c_string")
            //q.From.RightJoin("ttable_c", "t_r").On2("t1.cstring", "t_r.c_string", "t1.cint", "t_r.c_int")

            SetWhere(q.Where);

            q.GroupBy.
                Columns("cbool", "cint", "cnumeric", "cstring", "cfloat").
                By(new Column("cdatetime")).
                By(new RawSql("cint - 1"));

            q.Having.
               Like("cstring", "%like%").
               NotIn("cint", new int[] { 1, 2, 3, 4, 5 }).
               OpenParentheses().
               LessThan("cint", 12345).
               Or().
               GreaterOrEquals("cint", 101).
               CloseParentheses().
               EqualsTo("cnumeric", 1.1).
               IsNotNull("cbool");

            q.Having.
                Avg(SqlOperator.LessThan, "cint", 201).
                Count(SqlOperator.GreaterThan, "cint", 301).
                Sum(SqlOperator.NotEquals, "cint", 401).
                Min(SqlOperator.LessOrEquals, "cint", new RawSql("501")).
                Max(SqlOperator.GreaterOrEquals, "cint", new RawValue(601));

            SetOrderBy(q.OrderBy);

            q.Limit(3, 101);

            return q;
        }

        public Text BuildText()
        {
            var sql = @"
select * 
from ttable 
where 
        cbool = @cbool
        and cint > @cint
        and cfloat < @cfloat
        and cnumeric <> @cnumeric
        and cstring like @cstring
        and cdate = @cdate
        and cdatetime = @cdatetime
        and cbytes is null 
        and cguid = @cguid
";
            var text = new Text(sql);

            foreach (var p in dataTypeMap)
            {
                text.Set(p.Key, p.Value);
            }

            return text;
        }

        public MarkText BuildMarkText()
        {
            var sql = @"
select * 
from ttable 
where 
        cbool = {cbool}
        and cint > {cint}
        and cfloat < {cfloat} 
        and cnumeric <> {cnumeric}
        and cstring like {cstring} 
        and cdate = {cdate}
        and cdatetime = {cdatetime}
        and cbytes is null 
        and cguid = {cguid} 
";
            var text = new MarkText(sql);

            foreach (var p in dataTypeMap)
            {
                text.Set(p.Key, p.Value);
            }

            return text;
        }

        public Procedure BuildProcedure()
        {
            var p = new Procedure("sp_types");
            p.Set("cbool", true)
                .Set("cint", 123)
                .Set("cfloat", 3.14)
                .Set("cnumeric", 101.101)
                .Set("cdate", "2004-07-24")
                .Set("cdatetime", "2013-01-01 01:02:03");

            return p;
        }


        public Update BuildUpdate()
        {
            var u = new Update("ttable");

            foreach (var p in dataTypeMap)
            {
                u.Set(p.Key, p.Value);
            }

            u.Where.EqualsTo("cint", 101);
            u.OrderBy.Asc("cint");
            u.Limit(101);

            return u;
        }

        public Delete BuildDelete()
        {
            var d = new Delete("ttable");

            d.Where.EqualsTo("cint", 101);
            d.OrderBy.Asc("cint");
            d.Limit(101);

            return d;
        }

        public Insert BuildInsert()
        {
            var insert = new Insert("ttable");

            foreach (var p in dataTypeMap)
            {
                insert.Set(p.Key, p.Value);
            }

            return insert;
        }

    }

}
