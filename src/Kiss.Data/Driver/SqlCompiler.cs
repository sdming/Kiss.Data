using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kiss.Data.Expression;
using System.Data.Common;
using System.Collections;
using Kiss.Data.Schema;
using System.Data;

namespace Kiss.Data.Driver
{
    /// <summary>
    /// SqlCompiler
    /// </summary>
    public class SqlCompiler
    {
        /// <summary>
        /// writer
        /// </summary>
        internal SqlWriter Writer;

        /// <summary>
        /// driver
        /// </summary>
        internal SqlDriver Driver;

        /// <summary>
        /// parameterIndex
        /// </summary>
        protected int parameterIndex;

        /// <summary>
        /// ParameterPrefix
        /// </summary>
        protected const string ParameterPrefix = "P_";

        /// <summary>
        /// schema
        /// </summary>
        internal IDbSchemaProvider Schema;

        /// <summary>
        /// blank char
        /// </summary>
        protected const string blank = " ";

        /// <summary>
        /// command
        /// </summary>
        internal DbCommand Command;

        /// <summary>
        /// SqlCompiler
        /// </summary>
        public SqlCompiler()
        {
        }

        /// <summary>
        /// SqlCompiler
        /// </summary>
        public SqlCompiler(SqlDriver driver, DbCommand command, SqlWriter writer, IDbSchemaProvider schema)
        {
            this.Driver = driver;
            this.Writer = writer;
            this.Command = command;
            this.Schema = schema;
        }

        /// <summary>
        /// Compile
        /// </summary>
        public void Compile(ISqlExpression expression)
        {
            switch (expression.NodeType())
            {
                case NodeType.Query:
                    var q = expression as Query;
                    //SetTableSchema(q.Table);
                    VisitQuery(q);
                    break;
                case NodeType.Update:
                    var u = expression as Update;
                    //SetTableSchema(u.Table);
                    VisitUpdate(expression as Update);
                    break;
                case NodeType.Insert:
                    var i = expression as Insert;
                    //SetTableSchema(i.Table);
                    VisitInsert(expression as Insert);
                    break;
                case NodeType.Delete:
                    var d = expression as Delete;
                    //SetTableSchema(d.Table);
                    VisitDelete(expression as Delete);
                    break;
                default:
                    throw new Exception("do not support expression type:" + expression.NodeType().ToString());
            }

        }

        /////// <summary>
        /////// table
        /////// </summary>
        ////protected Table table;

        ///// <summary>
        ///// tableSchema
        ///// </summary>
        //protected SqlTable tableSchema;

        //protected void SetTableSchema(Table t)
        //{

        //}

        /// <summary>
        /// VisitInsert
        /// </summary>
        /// <param name="insert"></param>
        protected virtual void VisitInsert(Insert insert)
        {
            if (string.IsNullOrEmpty(insert.Table.Name))
            {
                throw new Exception("table name can not be empty");
            }
            if (insert.Sets == null || insert.Sets.Count == 0)
            {
                throw new Exception("columns to insert can not be empty");
            }

            Writer.WriteN(Ansi.InsertInto, Driver.Dialecter.QuoteIdentifer(insert.Table.Name));
            Writer.OpenParentheses();
            for (var i = 0; i < insert.Sets.Count; i++)
            {
                if (i > 0)
                {
                    Writer.Comma();
                }

                VisitColumn(insert.Sets[i].Column);
            }
            Writer.CloseParentheses();

            Writer.LineBreak();
            Writer.Write(Ansi.Values);
            Writer.OpenParentheses();
            for (var i = 0; i < insert.Sets.Count; i++)
            {
                if (i > 0)
                {
                    Writer.Comma();
                }

                VisitExp(insert.Sets[i].Value);
            }
            Writer.CloseParentheses();
            VisitEndStatement();
        }

        /// <summary>
        /// VisitExp
        /// </summary>
        /// <param name="exp"></param>
        protected virtual void VisitExp(ISqlExpression exp)
        {
            if (exp == null)
            {
                return;
            }

            switch (exp.NodeType())
            {
                case NodeType.Null:
                    Writer.Write(Ansi.Null);
                    break;
                case NodeType.Sql:
                    Writer.Write((exp as IRawSql).ToSql());
                    break;
                case NodeType.Operator:
                    Writer.Write((exp as IRawSql).ToSql());
                    break;
                case NodeType.Insert:
                    VisitInsert((Insert)exp);
                    break;
                case NodeType.Query:
                    VisitQuery((Query)exp);
                    break;
                case NodeType.Update:
                    VisitUpdate((Update)exp);
                    break;
                case NodeType.Delete:
                    VisitDelete((Delete)exp);
                    break;
                case NodeType.Value:
                    VisitValue((RawValue)exp);
                    break;
                case NodeType.Table:
                    VisitTable((Table)exp);
                    break;
                case NodeType.Column:
                    VisitColumn((Column)exp);
                    break;
                case NodeType.Condition:
                    VisitCondition((Condition)exp);
                    break;
                case NodeType.Aggregate:
                    VisitAggregate((Aggregate)exp);
                    break;
                case NodeType.Select:
                    VisitSelect((Select)exp);
                    break;
                case NodeType.From:
                    VisitFrom((From)exp);
                    break;
                case NodeType.Join:
                    VisitJoin((Join)exp);
                    break;
                case NodeType.Where:
                    VisitWhere((Where)exp);
                    break;
                case NodeType.GroupBy:
                    VisitGroupBy((GroupBy)exp);
                    break;
                case NodeType.Having:
                    VisitHaving((Having)exp);
                    break;
                case NodeType.OrderBy:
                    VisitOrderBy((OrderBy)exp);
                    break;
                case NodeType.Parameter:
                    VisitParameter((Parameter)exp);
                    break;
                case NodeType.Text:
                    throw new NotImplementedException("NodeType.Text");
                case NodeType.Procedure:
                    throw new NotImplementedException("NodeType.Procedure");                
                case NodeType.Output:
                    throw new NotImplementedException("NodeType.Output");

            }
        }

        /// <summary>
        /// VisitAggregate
        /// </summary>
        /// <param name="a"></param>
        protected virtual void VisitAggregate(Aggregate a) 
        {
            if(a.Exression == null || string.IsNullOrEmpty(a.Func.Name))
            {
                return;
            }
            
            Writer.Write(a.Func.Name);
            Writer.OpenParentheses();
            VisitExp(a.Exression);
            Writer.CloseParentheses();
        }


        /// <summary>
        /// VisitParameter
        /// </summary>
        /// <param name="p"></param>
        protected virtual void VisitParameter(Parameter p)
        {
            string name = Driver.Dialecter.QuoteParameter(p.Name);
            Writer.Write(name, " ");

            var parameter = Command.CreateParameter();
            parameter.ParameterName = name;
            if (p.Direction > 0)
            {
                parameter.Direction = p.Direction;
            }
            if (p.DbType.HasValue)
            {
                Driver.SetParameterType(parameter, p.DbType.Value, p.Size, p.Precision, p.Scale);
                Driver.SetParameterValue(parameter, p.Value);
            }
            else
            {
                if (p.Value == null)
                {
                    parameter.Value = DBNull.Value;
                }
                else
                {
                    parameter.Value = p.Value;
                }
            }
            Command.Parameters.Add(parameter);
        }

        /// <summary>
        /// VisitValue
        /// </summary>
        /// <param name="v"></param>
        protected virtual void VisitValue(RawValue v)
        {
            string name = Driver.Dialecter.QuoteParameter(ParameterPrefix + (parameterIndex++).ToString());
            Writer.Write(name);

            //var parameter = Command.CreateParameter();
            //parameter.ParameterName = name;
            //if (v.Value == null)
            //{
            //    parameter.Value = DBNull.Value;
            //}
            //else
            //{
            //    parameter.Value = v.Value;
            //}
            var parameter = Driver.CreateParameter(name, v.Value);
            Command.Parameters.Add(parameter);
        }

        /// <summary>
        /// VisitColumn
        /// </summary>
        /// <param name="c"></param>
        protected virtual void VisitColumn(Column c)
        {
            Writer.Write(Driver.Dialecter.QuoteIdentifer(c.Name));
        }

        /// <summary>
        /// VisitTable
        /// </summary>
        /// <param name="t"></param>
        protected virtual void VisitTable(Table t)
        {
            if(string.IsNullOrEmpty(t.Name) && string.IsNullOrEmpty(t.Alias))
            {
                throw new Exception("table name can not be empty");
            }

            if(string.IsNullOrEmpty(t.Alias))
            {
                Writer.Write(Driver.Dialecter.QuoteIdentifer(t.Name));
            }
            else if(string.IsNullOrEmpty(t.Name))
            {
                Writer.Write(t.Alias);
            }
            else
            {
                Writer.WriteN(Driver.Dialecter.QuoteIdentifer(t.Name), Ansi.As, t.Alias);
            }
        }

        /// <summary>
        /// VisitCondition
        /// </summary>
        /// <param name="c"></param>
        protected virtual void VisitCondition(Condition c)
        {
            if (c.Right == null && c.Left == null)
            {
                Writer.Write(c.Op.ToSql());
            }
            else if (c.Left == null)
            {
                Writer.Write(c.Op.ToSql());
                Writer.Write("(");
                VisitExp(c.Right);
                Writer.Write(")");
            }
            else if (c.Right == null)
            {
                VisitExp(c.Left);
                Writer.Write(blank);
                Writer.Write(c.Op.ToSql());
            }
            else
            {
                if (c.Op == SqlOperator.In || c.Op == SqlOperator.NotIn)
                {
                    VisitIn(c);
                }
                else
                {
                    VisitExp(c.Left);
                    Writer.Write(blank);
                    Writer.Write(c.Op.ToSql());
                    Writer.Write(blank);
                    VisitExp(c.Right);
                }
            }
        }

        /// <summary>
        /// VisitIn
        /// </summary>
        /// <param name="c"></param>
        protected virtual void VisitIn(Condition c)
        {
            VisitExp(c.Left);
            Writer.Write(blank);
            Writer.Write(c.Op.ToSql());
            Writer.Write(blank);

            Writer.OpenParentheses();
            if (c.Right == null)
            {
                Writer.Write(Ansi.Null);
            }
            else
            {
                if (c.Right is RawValue)
                {
                    VisitSlice(((RawValue)c.Right).Value);
                }
                else
                {
                    VisitExp(c.Right);
                }
            }
            Writer.CloseParentheses();
        }

        /// <summary>
        /// VisitSlice
        /// </summary>
        /// <param name="v"></param>
        protected virtual void VisitSlice(object v)
        {
            if(v == null)
            {
                Writer.Write(Ansi.Null);
                return;
            }

            IList list = v as IList;
            if(list == null)
            {
                throw new Exception("value is not a list");
            }

            bool split = false;
            for(var i = 0; i < list.Count; i++)
            {
                if(split)
                {
                    Writer.Comma();
                }
                split = true;

                object item = list[i];
                ISqlExpression exp = v as ISqlExpression;
                if(exp != null)
                {
                    VisitExp(exp);
                }
                else
                {
                    string name = Driver.Dialecter.QuoteParameter(ParameterPrefix + (parameterIndex++).ToString());
                    Writer.Write(name);

                    var parameter = Command.CreateParameter();
                    parameter.ParameterName = name;
                    if (item == null)
                    {
                        parameter.Value = DBNull.Value;
                    }
                    else
                    {
                        parameter.Value = item;
                    }
                    Command.Parameters.Add(parameter);
                }
            }

        }

        /// <summary>
        /// VisitConditionList
        /// </summary>
        /// <param name="c"></param>
        protected virtual void VisitConditionList(ConditionList c)
        {            
            if (c.Conditions == null)
            {
                return;
            }

            var deep = 0;
            for (var i = 0; i < c.Conditions.Count; i++)
            {
                var item = c.Conditions[i];
                if (i > 0)
                {
                    Writer.LineBreak();
                }
                if (item == SqlOperator.CloseParentheses)
                {
                    deep--;
                }
                if(deep > 0)
                {
                    for(var index = 0; index < deep; index++)
                    {
                        Writer.Write("\t");
                    }                     
                }
                VisitExp(item);
                if(item == SqlOperator.OpenParentheses)
                {
                    deep++;
                }

            }

            Writer.Write(blank);
        }

        /// <summary>
        /// VisitJoin
        /// </summary>
        /// <param name="j"></param>
        protected virtual void VisitJoin(Join j)
        {
            Writer.Write(j.JoinType.ToSql());
            Writer.Write(blank);
            VisitTable(j.Right);
            Writer.Write(blank);

            if (j.Conditions!= null && !j.Conditions.IsEmpty())
            {
                Writer.Write(Ansi.On);
                for (var i = 0; i < j.Conditions.Conditions.Count; i++)
                {
                    Writer.Write(blank);
                    VisitExp(j.Conditions.Conditions[i]);
                    Writer.Write(blank);
                }
            }
        }

        /// <summary>
        /// VisitFrom
        /// </summary>
        /// <param name="f"></param>
        protected virtual void VisitFrom(From f)
        {
            Writer.Write("\r\n", Ansi.From, blank);

            var split = false;
            VisitTable(f.Table);
            split = true;

            if (f.Tables != null)
            {
                for (var i = 0; i < f.Tables.Count; i++)
                {
                    if (split)
                    {
                        Writer.Comma();
                    }
                    split = true;
                    VisitTable(f.Tables[i]);
                }
            }

            if (f.Joins != null)
            {
                for (var i = 0; i < f.Joins.Count; i++)
                {
                    Writer.LineBreak();
                    VisitJoin(f.Joins[i]);
                }
            }
            Writer.Write(blank);
        }


        /// <summary>
        /// VisitFrom
        /// </summary>
        /// <param name="f"></param>
        protected virtual void VisitFrom(Table f)
        {
            Writer.Write("\r\n", Ansi.From, blank);
            VisitTable(f);
        }

        /// <summary>
        /// VisitWhere
        /// </summary>
        /// <param name="w"></param>
        protected virtual void VisitWhere(Where w)
        {
            if (w == null || w.IsEmpty())
            {
                return;
            }
            Writer.Write("\r\n", Ansi.Where, "\r\n");
            VisitConditionList(w.Conditions);
        }

        /// <summary>
        /// VisitField
        /// </summary>
        /// <param name="f"></param>
        protected virtual void VisitField(Field f)
        {
            if (f == null )
            {
                return;
            }
            VisitExp(f.Expression);
            if (!string.IsNullOrEmpty(f.Alias))
            {
                Writer.Write(blank, Ansi.As, blank);
                Writer.Write(Driver.Dialecter.QuoteIdentifer(f.Alias));
            }
        }

        /// <summary>
        /// VisitSelect
        /// </summary>
        /// <param name="f"></param>
        protected virtual void VisitSelect(Select s)
        {
            if (s == null || s.Fields == null || s.Fields.Count == 0)
            {
                Writer.Write(Ansi.WildcardAll);
                Writer.Write(blank);
                return;
            }

            var split = false;
            for (var i = 0; i < s.Fields.Count; i++)
            {
                if (split)
                {
                    Writer.Comma();
                }
                split = true;
                VisitField(s.Fields[i]);
            }
            Writer.Write(blank);
        }

        /// <summary>
        /// VisitHaving
        /// </summary>
        /// <param name="h"></param>
        protected virtual void VisitHaving(Having h)
        {
            if (h == null || h.Conditions == null || h.Conditions.IsEmpty())
            {
                return;
            }

            Writer.Write("\r\n", Ansi.Having, "\r\n");
            VisitConditionList(h.Conditions);
        }

        /// <summary>
        /// VisitGroupBy
        /// </summary>
        /// <param name="h"></param>
        protected virtual void VisitGroupBy(GroupBy g)
        {
            if (g == null || g.IsEmpty())
            {
                return;
            }

            Writer.LineBreak();
            Writer.Write(Ansi.GroupBy);
            Writer.Write(blank);

            var split = false;
            for (var i = 0; i < g.Fields.Count; i++)
            {
                if (split)
                {
                    Writer.Comma();
                }
                split = true;
                VisitExp(g.Fields[i]);
            }
            Writer.Write(blank);
        }

        

        /// <summary>
        /// VisitOrderBy
        /// </summary>
        /// <param name="o"></param>
        protected virtual void VisitOrderBy(OrderBy o)
        {
            if (o == null || o.IsEmpty())
            {
                return;
            }

            Writer.LineBreak();
            Writer.Write(Ansi.OrderBy);
            Writer.Write(blank);
            VisitOrderByFields(o);
            Writer.Write(blank);
        }

        /// <summary>
        /// VisitOrderByFields
        /// </summary>
        /// <param name="o"></param>
        protected virtual void VisitOrderByFields(OrderBy o)
        {            
            var split = false;
            for (var i = 0; i < o.Fields.Count; i++)
            {
                if (split)
                {
                    Writer.Comma();
                }
                split = true;
                VisitExp(o.Fields[i].Expression);
                Writer.Write(blank);
                Writer.Write(o.Fields[i].Direction.ToSql());
            }
        }

        /// <summary>
        /// VisitQuery
        /// </summary>
        /// <param name="query"></param>
        protected virtual void VisitQuery(Query query)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
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

            if(query.GroupBy != null && !query.GroupBy.IsEmpty()) 
            {
                VisitHaving(query.Having);
            }
            VisitOrderBy(query.OrderBy);
            if (query.Offset > 0 || query.Count > 0)
            {
                VisitLimit(query);
            }
            VisitEndStatement();

        }
        
        /// <summary>
        /// VisitLimit
        /// </summary>
        /// <param name="u"></param>
        protected virtual void VisitLimit(Query query)
        {            
            Writer.LineBreak();
            Writer.Write(Ansi.Limit, blank, query.Offset.ToString());
            Writer.Write(",", query.Count.ToString());
        }

        /// <summary>
        /// VisitUpdate
        /// </summary>
        /// <param name="u"></param>
        protected virtual void VisitUpdateFields(Update u)
        {
            var split = false;
            for (var i = 0; i < u.Sets.Count; i++)
            {
                if (split)
                {
                    Writer.Comma();
                }
                split = true;

                VisitColumn(u.Sets[i].Column);
                Writer.Write(blank, Ansi.EqualsTo, blank);
                VisitExp(u.Sets[i].Value);
            }
        }

        /// <summary>
        /// VisitLimit
        /// </summary>
        /// <param name="u"></param>
        protected virtual void VisitLimit(Update u)
        {
            Writer.LineBreak();
            Writer.WriteN(Ansi.Limit, u.Count.ToString());
        }

        /// <summary>
        /// VisitUpdate
        /// </summary>
        /// <param name="u"></param>
        protected virtual void VisitUpdate(Update u)
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
            VisitOrderBy(u.OrderBy);
            if(u.Count > 0)
            {
                VisitLimit(u);
            }
            VisitEndStatement();
        }

        /// <summary>
        /// VisitLimit
        /// </summary>
        /// <param name="d"></param>
        protected virtual void VisitLimit(Delete d)
        {
            Writer.LineBreak();
            Writer.WriteN(Ansi.Limit, d.Count.ToString());
        }

        /// <summary>
        /// VisitDelete
        /// </summary>
        /// <param name="d"></param>
        protected virtual void VisitDelete(Delete d)
        {
            if (d == null)
            {
                throw new ArgumentNullException("delete");
            }

            Writer.WriteN(Ansi.Delete, Ansi.From, Driver.Dialecter.QuoteIdentifer(d.Table.Name));
            VisitWhere(d.Where);
            VisitOrderBy(d.OrderBy);
            if(d.Count > 0)
            {
                VisitLimit(d);
            }
            VisitEndStatement();
        }

        /// <summary>
        /// VisitEndStatement
        /// </summary>
        protected virtual void VisitEndStatement()
        {
            Writer.Write(blank);
            Writer.Write(Ansi.StatementSplit);
        }


    }


}
