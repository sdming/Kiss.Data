using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kiss.Data.Expression;

namespace Kiss.Data.Driver
{
    /// <summary>
    /// PostgreCompiler
    /// </summary>
    public class PostgresCompiler : SqlCompiler
    {
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
        /// VisitLimit
        /// </summary>
        /// <param name="u"></param>
        protected override void VisitLimit(Query query)
        {
            Writer.LineBreak();
            
            if (query.Count > 0)
            {
                Writer.Write(" LIMIT  ");
                Writer.Write(query.Count.ToString());
                Writer.Write(blank);
            }
            if (query.Offset > 0)
            {
                Writer.Write(" OFFSET ");
                Writer.Write(query.Offset.ToString());
                Writer.Write(blank);
            }
               
        }
    }
}
