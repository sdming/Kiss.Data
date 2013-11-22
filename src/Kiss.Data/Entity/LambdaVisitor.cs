using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Collections.ObjectModel;
using System.Reflection;
using k = Kiss.Data.Expression;

namespace Kiss.Data.Entity
{
    public class LambdaVisitor<T>
    {
        private static readonly Type entityType = typeof(T);

        private Kiss.Data.Expression.Where where;
        private Func<string, string> mapping;

        public LambdaVisitor()
        {
        }

        public Kiss.Data.Expression.Where Compile(LambdaExpression exp, Func<string, string> fieldToColumnMapping)
        {
            if (exp == null)
            {
                return null;
            }
            this.mapping = fieldToColumnMapping;

            this.where = new k.Where();
            Visit(exp.Body);
            return this.where;
        }

        public Kiss.Data.Expression.Where Compile(LambdaExpression exp)
        {
            return Compile(exp, null);
        }

        protected void VisitLambda(LambdaExpression lambda)
        {
            Visit(lambda.Body);
        }

        protected void Visit(System.Linq.Expressions.Expression exp)
        {
            if (exp == null)
            {
                return;
            }

            switch (exp.NodeType)
            {

                case ExpressionType.AndAlso:
                case ExpressionType.OrElse:
                    VisitAndOr(exp as BinaryExpression); break;
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                    VisitOperate(exp as BinaryExpression); break;
                case ExpressionType.Lambda:
                    VisitLambda((LambdaExpression)exp); break;
                //case ExpressionType.Call:
                //case ExpressionType.And:
                //case ExpressionType.Or:
                //case ExpressionType.Negate:
                //case ExpressionType.NegateChecked:
                //case ExpressionType.Not:
                //case ExpressionType.Convert:
                //case ExpressionType.ConvertChecked:
                //case ExpressionType.ArrayLength:
                //case ExpressionType.Quote:
                //case ExpressionType.TypeAs:
                //case ExpressionType.Add:
                //case ExpressionType.AddChecked:
                //case ExpressionType.Subtract:
                //case ExpressionType.SubtractChecked:
                //case ExpressionType.Multiply:
                //case ExpressionType.MultiplyChecked:
                //case ExpressionType.Divide:
                //case ExpressionType.Modulo:
                //case ExpressionType.Coalesce:
                //case ExpressionType.ArrayIndex:
                //case ExpressionType.RightShift:
                //case ExpressionType.LeftShift:
                //case ExpressionType.ExclusiveOr:
                //case ExpressionType.TypeIs:
                //case ExpressionType.New:
                //case ExpressionType.MemberInit:
                //case ExpressionType.ListInit:
                //case ExpressionType.NewArrayInit:
                //case ExpressionType.NewArrayBounds:
                //case ExpressionType.Conditional:
                //case ExpressionType.Constant:
                //case ExpressionType.Parameter:
                //case ExpressionType.MemberAccess:
                //case ExpressionType.Invoke:
                default:
                    throw new Exception(string.Format("Vist: unhandled expression type: '{0}'", exp.NodeType));
            }
        }

        protected void VisitAndOr(BinaryExpression b)
        {
            where.OpenParentheses();
            Visit(b.Left);
            where.CloseParentheses();

            switch (b.NodeType)
            {
                case ExpressionType.AndAlso:
                    where.And(); break;
                case ExpressionType.OrElse:
                    where.Or(); break;
                default:
                    throw new Exception(string.Format("VisitAndOr:unhandled expression type: '{0}'", b.NodeType));
            }

            where.OpenParentheses();
            Visit(b.Right);
            where.CloseParentheses();
        }

        protected void VisitOperate(BinaryExpression b)
        {
            k.Condition c = new k.Condition();
            switch (b.NodeType)
            {
                case ExpressionType.LessThan:
                    c.Op = k.SqlOperator.LessThan; break;
                case ExpressionType.LessThanOrEqual:
                    c.Op = k.SqlOperator.LessOrEquals; break;
                case ExpressionType.GreaterThan:
                    c.Op = k.SqlOperator.GreaterThan; break;
                case ExpressionType.GreaterThanOrEqual:
                    c.Op = k.SqlOperator.GreaterOrEquals; break;
                case ExpressionType.Equal:
                    c.Op = k.SqlOperator.EqualsTo; break;
                case ExpressionType.NotEqual:
                    c.Op = k.SqlOperator.NotEquals; break;
                default:
                    throw new Exception(string.Format("VisitOperate: unhandled expression type: '{0}'", b.NodeType));
            }
            c.Left = GetSqlExpression(b.Left);
            c.Right = GetSqlExpression(b.Right);
            this.where.Conditions.Append(c);
        }

        protected k.ISqlExpression GetSqlExpression(object o)
        {
            if (o == null)
            {
                return null;
            }
            var exp = o as System.Linq.Expressions.Expression;
            if (exp != null)
            {
                var v = GetValue(exp);
                if( v == null)
                {
                    return null;
                }
                var sql = v as k.ISqlExpression;
                if (sql != null)
                {
                    return sql;
                }
                return new k.RawValue(v);
            }

            return new k.RawValue(o);
        }

        protected object GetValue(System.Linq.Expressions.Expression exp)
        {
            if(exp == null)
            {
                return null;
            }
            switch (exp.NodeType)
            {
                case ExpressionType.MemberAccess:
                    return GetMemberValue((MemberExpression)exp);
                case ExpressionType.Constant:
                    return GetConstantValue((ConstantExpression)exp);
                case ExpressionType.Call:
                    return GetMethodCall((MethodCallExpression)exp);
                case ExpressionType.Convert:
                    return GetValue(((UnaryExpression)exp).Operand);
                default:
                    throw new Exception(string.Format("GetValue: unhandled expression type: '{0}'", exp.NodeType));
            }
        }

        protected object GetConstantValue(ConstantExpression c)
        {
            return c.Value;
        }

        protected object GetMemberValue(MemberExpression m)
        {
            if (m.Member.DeclaringType == entityType)
            {
                return new k.Column(m.Member.Name);
            }
            
            if (m.Member.MemberType == MemberTypes.Property)
            {
                PropertyInfo p = m.Member as PropertyInfo;
                if (p.GetGetMethod().IsStatic)
                {
                    return p.GetValue(null, null);
                }
                object instance = GetValue(m.Expression);
                return p.GetValue(instance, null);
            }

            if (m.Member.MemberType == MemberTypes.Field)
            {
                FieldInfo f = m.Member as FieldInfo;
                if (f.IsStatic)
                {
                    return f.GetValue(null);
                }
                object instance = GetValue(m.Expression);
                return f.GetValue(instance);
            }

            throw new Exception(string.Format("GetMemberValue: unhandled member type: '{0}'", m.Member.MemberType));
        }

        protected object GetMethodCall(MethodCallExpression m)
        {
            object[] args = new object[m.Arguments.Count];
            for (var i = 0; i < m.Arguments.Count; i++)
            {
                var a = m.Arguments[i];
                switch (a.NodeType)
                {
                    case ExpressionType.Constant:
                        args[i] = GetConstantValue((ConstantExpression)a); break;
                    case ExpressionType.MemberAccess:
                        args[i] = GetMemberValue((MemberExpression)a); break;                        
                    default:
                        throw new Exception("do not support method call arguments:" + a.NodeType);
                }
            }
            if (m.Method.IsStatic)
            {
                return m.Method.Invoke(null, args);
            }
            var instance = GetValue(m.Object);
            return m.Method.Invoke(instance, args);
        }

    }
}
