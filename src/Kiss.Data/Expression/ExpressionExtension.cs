using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kiss.Data.Expression
{
    internal static class ExpressionExtension
    {
        /// <summary>
        /// copy value from Schema.SqlParameter to a Expression.Parameter
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public static void Copy(this Schema.SqlParameter source, Parameter target)
        {
            target.Name = source.Name;
            target.Direction = source.Direction;
            target.DbType = source.DbType;
            if (source.ProviderDbType.HasValue)
            {
                target.ProviderDbType = source.ProviderDbType;
            }
            if (source.Precision.HasValue)
            {
                target.Precision = source.Precision;
            }
            if (source.Scale.HasValue)
            {
                target.Scale = source.Scale;
            }
            if (source.Size.HasValue)
            {
                target.Size = source.Size;
            }
        }

        public static void Copy(this Kiss.Data.Schema.SqlField source, Parameter target)
        {
            target.DbType = source.DbType;
            if (source.ProviderDbType.HasValue)
            {
                target.ProviderDbType = source.ProviderDbType;
            }
            if (source.Precision.HasValue)
            {
                target.Precision = source.Precision;
            }
            if (source.Scale.HasValue)
            {
                target.Scale = source.Scale;
            }
            if (source.Size.HasValue)
            {
                target.Size = source.Size;
            }
        }
            

        /// <summary>
        /// AsExpression
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static ISqlExpression AsExpression(this object obj)
        {
            if (obj == null)
            {
                return SqlNull.Value;
            }

            ISqlExpression exp = obj as ISqlExpression;
            if (exp != null)
            {
                return exp;
            }

            return new RawValue(obj);
        }
    }
}
