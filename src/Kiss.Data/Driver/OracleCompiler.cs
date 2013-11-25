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

        /// <summary>
        /// VisitInsert
        /// </summary>
        /// <param name="insert"></param>
        protected override void VisitInsert(Insert insert)
        {
            if (string.IsNullOrEmpty(insert.Output))
            {
                base.VisitInsert(insert);
                return;
            }

            Writer.Write("DECLARE ");
            string[] names = insert.Output.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (var name in names)
            {
                Writer.Write("r_p_" + name + " NUMBER " + " ; ");
            }
            Writer.LineBreak();
            Writer.Write("begin \r\n");
            WriteInsertValues(insert);
            Writer.LineBreak();
            
            var parameter = Command.CreateParameter();
            parameter.ParameterName = ":p_return_refcursor";
            parameter.Direction = System.Data.ParameterDirection.Output;
            Driver.SetParameterProviderDbType(parameter, 121); //RefCursor
            Command.Parameters.Add(parameter);

            Writer.Write("returning ");
            for(var i = 0 ; i< names.Length; i++)
            {
                if(i > 0)
                {
                    Writer.Comma();
                }
                Writer.Write(names[i]);                
            }
            Writer.Write(" into ");
            for(var i = 0 ; i< names.Length; i++)
            {
                if(i > 0)
                {
                    Writer.Comma();
                }
                Writer.Write("r_p_" + names[i]);                
            }
            Writer.Write(" ; ");
            Writer.LineBreak();
            Writer.WriteN("open", parameter.ParameterName, "for select ");
            for(var i = 0 ; i< names.Length; i++)
            {
                if(i > 0)
                {
                    Writer.Comma();
                }
                Writer.WriteN("r_p_" + names[i], "as", names[i]);                
            }
            Writer.Write(" from dual; ");
            Writer.LineBreak();
            Writer.Write("end; ");
            VisitEndStatement();
        }

        protected void SetParameterDbType(System.Data.IDataParameter parameter, string dbType)
        {
            parameter.GetType().GetProperty("OracleDbType");
        }

    }
}
