using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kiss.Data.Expression;

namespace Kiss.Data.Driver
{
    /// <summary>
    /// MsSql2012Compiler
    /// </summary>
    internal class MsSql2012Compiler : SqlCompiler
    {
        /// <summary>
        /// VisitLimit
        /// </summary>
        /// <param name="u"></param>
        protected override void VisitLimit(Query query)
        {
            if (query.Offset > 0 || query.Count > 0)
            {
                Writer.LineBreak();
                Writer.Write("OFFSET ");
                if (query.Offset > 0)
                {
                    Writer.Write(query.Offset.ToString(), " ROWS");
                }
                else
                {
                    Writer.Write("0 ROWS");
                }
                if (query.Count > 0)
                {
                    Writer.Write(" FETCH FIRST ", query.Count.ToString(), " ROWS ONLY");
                }

            }           
        }
    }
}
