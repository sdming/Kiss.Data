using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kiss.Data.Expression;

namespace Kiss.Data.Driver
{
    /// <summary>
    /// MsSqlCompiler
    /// </summary>
    internal class MsSqlCompiler : SqlCompiler
    {
        /// <summary>
        /// VisitOutput
        /// </summary>
        /// <param name="insert"></param>
        protected override void VisitOutput(Insert insert)
        {
            if(string.IsNullOrEmpty(insert.Output))
            {
                return;
            }
            Writer.LineBreak();
            Writer.Write("OUTPUT ");
            string[] fields = insert.Output.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach(var f in fields)
            {
                Writer.Write("INSERTED."+f.Trim());
            }
            Writer.Write(blank);
        }

        /// <summary>
        /// VisitReturing
        /// </summary>
        /// <param name="insert"></param>
        protected override void VisitReturing(Insert insert)
        {
            return;
        }

        /// <summary>
        /// VisitUpdate
        /// </summary>
        /// <param name="u"></param>
        protected override void VisitDelete(Delete d)
        {
            if (d == null)
            {
                throw new ArgumentNullException("delete");
            }

            Writer.Write(Ansi.Delete, blank);
            if (d.Count > 0)
            {
                Writer.WriteN("TOP(", d.Count.ToString(), ") ");
            }
            Writer.Write(Ansi.From, blank);
            Writer.Write(Driver.Dialecter.QuoteIdentifer(d.Table.Name), blank);
            VisitWhere(d.Where);
            //VisitOrderBy(d.OrderBy); //sql server does not support order by 
            VisitEndStatement();
        }

        /// <summary>
        /// VisitUpdate
        /// </summary>
        /// <param name="u"></param>
        protected override void VisitUpdate(Update u)
        {
            if (u == null)
            {
                throw new ArgumentNullException("update");
            }
            if (u.Sets == null || u.Sets.Count == 0)
            {
                throw new Exception("update fields can not be empty");
            }

            Writer.Write(Ansi.Update, blank);
            if (u.Count > 0)
            {
                Writer.WriteN("TOP(", u.Count.ToString(), ") ");
            }
            Writer.WriteN(Driver.Dialecter.QuoteIdentifer(u.Table.Name), Ansi.Set);
            Writer.LineBreak();
            VisitUpdateFields(u);
            VisitWhere(u.Where);
            //VisitOrderBy(u.OrderBy); //sql server does not support order by 
            VisitEndStatement();
        }

        /// <summary>
        /// VisitQuery
        /// </summary>
        /// <param name="query"></param>
        protected override void VisitQuery(Query query)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            if(query.Count <= 0 && query.Offset <=0 )
            {
                base.VisitQuery(query);
                return;
            }
            
            //select
            Writer.Write("SELECT * FROM ( \r\n");

            Writer.Write(Ansi.Select);
            Writer.Write(blank);
            if (query.IsDistinct)
            {
                Writer.Write(Ansi.Distinct);
                Writer.Write(blank);
            }
            Writer.Write("TOP ", (query.Offset + query.Count + 1).ToString(), blank);
            VisitSelect(query.Select);

            Writer.Write(" , ROW_NUMBER() OVER (ORDER BY ");

            if (query.OrderBy == null || query.OrderBy.IsEmpty())
            {
                throw new Exception("need order by clause when use row number");
            }
            else
            {
                VisitOrderByFields(query.OrderBy);
            }
            Writer.Write(" ) AS [_kiss_rownumber] ");

            VisitFrom(query.Table);
            VisitWhere(query.Where);
            VisitGroupBy(query.GroupBy);

            if(query.GroupBy != null && !query.GroupBy.IsEmpty()) 
            {
                VisitHaving(query.Having);
            }
            VisitOrderBy(query.OrderBy);

            Writer.Write(string.Format("\r\n) [_kiss_page] WHERE [_kiss_rownumber] >= {0} AND  [_kiss_rownumber] < {1} ", 
                query.Offset, query.Offset + query.Count));

            VisitEndStatement();
        }

    }
}
