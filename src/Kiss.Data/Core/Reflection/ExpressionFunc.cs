using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;

namespace Kiss.Data.Core.Reflection
{
    public class ExpressionFunc<T>
    {

        public static Func<T, object> BuildPropertyGetFunc(PropertyInfo propertyInfo)
        {
            ParameterExpression sourceParamter = System.Linq.Expressions.Expression.Parameter(typeof(T), "instance");
            System.Linq.Expressions.Expression source = propertyInfo.GetGetMethod(true).IsStatic ? null : CastOrConvertExpression(sourceParamter, propertyInfo.ReflectedType);
            System.Linq.Expressions.Expression property = System.Linq.Expressions.Expression.Property(source, propertyInfo);
            System.Linq.Expressions.Expression result = CastOrConvertExpression(property, typeof(object));
            return System.Linq.Expressions.Expression.Lambda<Func<T, object>>(result, sourceParamter).Compile();
        }

        public static Action<T, object> BuildPropertySetFunc(PropertyInfo propertyInfo)
        {
            ParameterExpression sourceParameter = System.Linq.Expressions.Expression.Parameter(typeof(T), "instance");
            System.Linq.Expressions.Expression source = propertyInfo.GetGetMethod(true).IsStatic ? null : CastOrConvertExpression(sourceParameter, propertyInfo.DeclaringType);
            System.Linq.Expressions.Expression property = System.Linq.Expressions.Expression.Property(source, propertyInfo);
            ParameterExpression valueParameter = System.Linq.Expressions.Expression.Parameter(typeof(object), "value");
            System.Linq.Expressions.Expression value = CastOrConvertExpression(valueParameter, property.Type);
            BinaryExpression assign = System.Linq.Expressions.Expression.Assign(property, value);
            return System.Linq.Expressions.Expression.Lambda<Action<T, object>>(assign, sourceParameter, valueParameter).Compile();
        }


        public static Func<T, object> BuildFieldGetFunc(FieldInfo fieldInfo)
        {
            ParameterExpression sourceParamter = System.Linq.Expressions.Expression.Parameter(typeof(T), "instance");
            System.Linq.Expressions.Expression source = fieldInfo.IsStatic ? null : CastOrConvertExpression(sourceParamter, fieldInfo.DeclaringType);
            System.Linq.Expressions.Expression field = System.Linq.Expressions.Expression.Field(source, fieldInfo);
            System.Linq.Expressions.Expression result = CastOrConvertExpression(field, typeof(object));
            return System.Linq.Expressions.Expression.Lambda<Func<T, object>>(result, sourceParamter).Compile();

        }

        public static Action<T, object> BuildFieldSetFunc(FieldInfo fieldInfo)
        {
            ParameterExpression sourceParameter = System.Linq.Expressions.Expression.Parameter(typeof(T), "instance");
            System.Linq.Expressions.Expression source = fieldInfo.IsStatic ? null : CastOrConvertExpression(sourceParameter, fieldInfo.DeclaringType);
            System.Linq.Expressions.Expression field = System.Linq.Expressions.Expression.Field(source, fieldInfo);
            ParameterExpression valueParameter = System.Linq.Expressions.Expression.Parameter(typeof(object), "value");
            System.Linq.Expressions.Expression value = CastOrConvertExpression(valueParameter, field.Type);
            BinaryExpression assign = System.Linq.Expressions.Expression.Assign(field, value);

            return System.Linq.Expressions.Expression.Lambda<Action<T, object>>(assign, sourceParameter, valueParameter).Compile();
        }

        private static System.Linq.Expressions.Expression CastOrConvertExpression(System.Linq.Expressions.Expression expression, Type targetType)
        {
            System.Linq.Expressions.Expression result;
            Type expressionType = expression.Type;

            if (targetType == expressionType)
            {
                result = expression;
            }
            else
            {
                if (targetType.IsValueType && !IsNullableType(targetType))
                {
                    result = System.Linq.Expressions.Expression.Convert(expression, targetType);
                }
                else
                {
                    result = System.Linq.Expressions.Expression.TypeAs(expression, targetType);
                }
            }

            return result;
        }

        private static bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
        }
    }
}
