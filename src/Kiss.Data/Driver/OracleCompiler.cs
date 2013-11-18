using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kiss.Data.Expression;

namespace Kiss.Data.Driver
{
    /// <summary>
    /// OracleCompiler
    /// </summary>
    public class OracleCompiler : SqlCompiler
    {

        /// <summary>
        /// VisitDelete
        /// </summary>
        /// <param name="d"></param>
        protected override void VisitDelete(Delete d)
        {
            if (d == null)
            {
                throw new ArgumentNullException("delete");
            }

            Writer.WriteN(Ansi.Delete, Ansi.From, Driver.Dialecter.QuoteIdentifer(d.Table.Name));
            VisitWhere(d.Where);
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

            Writer.WriteN(Ansi.Update, Driver.Dialecter.QuoteIdentifer(u.Table.Name), Ansi.Set);
            Writer.LineBreak();
            VisitUpdateFields(u);
            VisitWhere(u.Where);
            VisitEndStatement();
        }

        protected override void VisitEndStatement()
        {
            //don't need ; 
        }

        protected override void VisitQuery(Query query)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            if (query.Count <= 0 && query.Offset <= 0)
            {
                base.VisitQuery(query);
                return;
            }

            if (query.Offset > 0 && query.Count > 0)
            {
                Writer.Write("select * from ( select kiss_row_.*, rownum kiss_rownum_ from ( \r\n");
            }
            else
            {
                Writer.Write("select * from ( \r\n");
            }

            Writer.Write(Ansi.Select);
            Writer.Write(blank);

            if (query.IsDistinct)
            {
                Writer.Write(Ansi.Distinct);
                Writer.Write(blank);
            }

            VisitSelect(query.Select);
            VisitFrom(query.Table);
            VisitWhere(query.Where);
            VisitGroupBy(query.GroupBy);

            if (query.GroupBy != null && !query.GroupBy.IsEmpty())
            {
                VisitHaving(query.Having);
            }
            VisitOrderBy(query.OrderBy);


            if (query.Offset > 0 && query.Count > 0)
            {
                Writer.Write(" \r\n) kiss_row_ where rownum <=" + query.Count.ToString() + ") where kiss_rownum_ >" + query.Offset.ToString() );
            }
            else if (query.Count > 0)
            {
                Writer.Write(" \r\n) where rownum <=" + query.Count.ToString() );
            }
            else
            {
                Writer.Write(" \r\n) where rownum > " + query.Offset.ToString());
            }

            VisitEndStatement();

        }

//        SELECT * FROM
//(
//SELECT A.*, rownum r
//FROM
//(
//SELECT *
//FROM a_matrix_navigation_map

//) A
//WHERE rownum <= 10
//) B
//WHERE r > 0


        

    }
}
