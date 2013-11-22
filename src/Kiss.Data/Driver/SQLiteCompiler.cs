using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kiss.Data.Expression;

namespace Kiss.Data.Driver
{
    /// <summary>
    /// SQLiteCompiler
    /// </summary>
    public class SQLiteCompiler : SqlCompiler
    {
        /// <summary>
        /// VisitReturing
        /// </summary>
        /// <param name="insert"></param>
        protected override void VisitReturing(Insert insert)
        {
            Writer.Write(blank);
            Writer.Write(Ansi.StatementSplit);
            Writer.LineBreak();
            Writer.Write("select last_insert_rowid()  ");
            Writer.Write(blank);
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
    }
}
