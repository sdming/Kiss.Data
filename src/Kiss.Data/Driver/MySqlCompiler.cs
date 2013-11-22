using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kiss.Data.Expression;

namespace Kiss.Data.Driver
{
    public class MySqlCompiler : SqlCompiler
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
            Writer.Write("select LAST_INSERT_ID()  ");
            Writer.Write(blank);
        }
    }
}
