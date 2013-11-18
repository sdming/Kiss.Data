using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Kiss.Core.Reflection
{
    /// <summary>
    /// IQToolkit
    /// </summary>
    public static class TypeSystem
    {
        public readonly static Type TypeGuid = typeof(Guid);
        public readonly static Type TypeByteArray = typeof(Byte[]);
        public readonly static Type TypeCharArray = typeof(Char[]);
        
        public static bool IsSimpleType(Type type)
        {
            if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Nullable<>)))
            {
                type = type.GetGenericArguments()[0];
            }
            if (type.IsEnum)
            {
                return true;
            }
            if (type == typeof(Guid))
            {
                return true;
            }
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Object:
                    return ((typeof(TimeSpan) == type) || (typeof(DateTimeOffset) == type));

                case TypeCode.Boolean:
                case TypeCode.Char:
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                case TypeCode.DateTime:
                case TypeCode.String:
                    return true;
            }
            return false;
        }

        public static bool IsSystemType(Type type)
        {
            return type == typeof(String) ||
                    type == typeof(Boolean) ||
                    type == typeof(Boolean?) ||
                    type == typeof(Char) ||
                    type == typeof(Char?) ||
                    type == typeof(SByte) ||
                    type == typeof(SByte?) ||
                    type == typeof(Byte) ||
                    type == typeof(Byte?) ||
                    type == typeof(Int16) ||
                    type == typeof(Int16?) ||
                    type == typeof(UInt16) ||
                    type == typeof(UInt16?) ||
                    type == typeof(Int32) ||
                    type == typeof(Int32?) ||
                    type == typeof(UInt32) ||
                    type == typeof(UInt32?) ||
                    type == typeof(Int64) ||
                    type == typeof(Int64?) ||
                    type == typeof(UInt64) ||
                    type == typeof(UInt64?) ||
                    type == typeof(Single) ||
                    type == typeof(Single?) ||
                    type == typeof(Double) ||
                    type == typeof(Double?) ||
                    type == typeof(Double) ||
                    type == typeof(Double?) ||
                    type == typeof(Decimal) ||
                    type == typeof(Decimal?) ||
                    type == typeof(DateTime) ||
                    type == typeof(DateTime?) ||
                    type == typeof(Guid) ||
                    type == typeof(Guid?);
        }

        public static Type GetSequenceType(Type elementType)
        {
            return typeof(IEnumerable<>).MakeGenericType(elementType);
        }

        public static bool IsNullableType(Type type)
        {
            return type != null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static bool IsNullAssignable(Type type)
        {
            return !type.IsValueType || IsNullableType(type);
        }

        public static MethodInfo FindMethod(Type type, string name, BindingFlags flags, Type[] argTypes)
        {
            return FindMethod(type, name, flags, argTypes, true);
        }

        public static MethodInfo FindMethod(Type type, string name, BindingFlags flags, Type[] argTypes, bool allowInherit)
        {
            while (type != typeof(object))
            {
                MethodInfo info = type.GetMethod(name, flags | BindingFlags.DeclaredOnly, null, argTypes, null);
                if ((info != null) || !allowInherit)
                {
                    return info;
                }
                type = type.BaseType;
            }
            return null;
        }

        public static void GetGettableFieldOrPropertyMember(MemberInfo member, out Type memberType)
        {
            FieldInfo info = member as FieldInfo;
            if (info == null)
            {
                PropertyInfo info2 = member as PropertyInfo;
                if (info2 == null)
                {
                    throw new Exception(string.Format("ArgumentMustBeFieldInfoOrPropertInfo({0});", member));
                }
                if (!info2.CanRead)
                {
                    throw new Exception(string.Format("PropertyDoesNotHaveGetter({0});", info2));
                }
                memberType = info2.PropertyType;
            }
            else
            {
                memberType = info.FieldType;
            }
        }

        public static bool IsBoolean(Type type)
        {
            type = GetNonNullableType(type);
            return (type == typeof(bool));
        }

        public static void GetSettableFieldOrPropertyMember(MemberInfo member, out Type memberType)
        {
            FieldInfo info = member as FieldInfo;
            if (info == null)
            {
                PropertyInfo info2 = member as PropertyInfo;
                if (info2 == null)
                {
                    throw new Exception(string.Format("ArgumentMustBeFieldInfoOrPropertInfo({0});", member));
                }
                if (!info2.CanWrite)
                {
                    throw new Exception(string.Format("PropertyDoesNotHaveSetter({0});", info2));
                }
                memberType = info2.PropertyType;
            }
            else
            {
                memberType = info.FieldType;
            }
        }

        public static bool IsArithmetic(Type type)
        {
            type = GetNonNullableType(type);
            if (!type.IsEnum)
            {
                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.Byte:
                    case TypeCode.SByte:
                    case TypeCode.Int16:
                    case TypeCode.UInt16:
                    case TypeCode.Int32:
                    case TypeCode.UInt32:
                    case TypeCode.Int64:
                    case TypeCode.UInt64:
                    case TypeCode.Single:
                    case TypeCode.Double:
                    case TypeCode.Decimal:
                        return true;
                }
            }
            return false;
        }

        public static object GetDefault(Type type)
        {
            bool isNullable = !type.IsValueType || IsNullableType(type);
            if (!isNullable)
                return Activator.CreateInstance(type);
            return null;
        }

        public static bool IsInteger(Type type)
        {
            Type nnType = GetNonNullableType(type);
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Byte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsReadOnly(MemberInfo member)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    return (((FieldInfo)member).Attributes & FieldAttributes.InitOnly) != 0;
                case MemberTypes.Property:
                    PropertyInfo pi = (PropertyInfo)member;
                    return !pi.CanWrite || pi.GetSetMethod() == null;
                default:
                    return true;
            }
        }

        public static Type GetMemberType(MemberInfo mi)
        {
            FieldInfo fi = mi as FieldInfo;
            if (fi != null) return fi.FieldType;
            PropertyInfo pi = mi as PropertyInfo;
            if (pi != null) return pi.PropertyType;
            EventInfo ei = mi as EventInfo;
            if (ei != null) return ei.EventHandlerType;
            MethodInfo meth = mi as MethodInfo;  
            if (meth != null) return meth.ReturnType;
            return null;
        }

        public static Type GetNullAssignableType(Type type)
        {
            if (!IsNullAssignable(type))
            {
                return typeof(Nullable<>).MakeGenericType(type);
            }
            return type;
        }

        public static Type GetNonNullableType(Type type)
        {
            if (IsNullableType(type))
            {
                return type.GetGenericArguments()[0];
            }
            return type;
        }

        public static bool IsNullableEnum(Type type)
        {
            var enumType = Nullable.GetUnderlyingType(type);

            return enumType != null && enumType.IsEnum;
        }

        public static Type FindIEnumerable(Type seqType)
        {
            if (seqType == null || seqType == typeof(string))
                return null;
            if (seqType.IsArray)
                return typeof(IEnumerable<>).MakeGenericType(seqType.GetElementType());
            if (seqType.IsGenericType)
            {
                foreach (Type arg in seqType.GetGenericArguments())
                {
                    Type ienum = typeof(IEnumerable<>).MakeGenericType(arg);
                    if (ienum.IsAssignableFrom(seqType))
                        return ienum;
                }
            }
            Type[] ifaces = seqType.GetInterfaces();
            if (ifaces != null && ifaces.Length > 0)
            {
                foreach (Type iface in ifaces)
                {
                    Type ienum = FindIEnumerable(iface);
                    if (ienum != null)
                        return ienum;
                }
            }
            if (seqType.BaseType != null && seqType.BaseType != typeof(object))
                return FindIEnumerable(seqType.BaseType);
            return null;
        }

        public static bool IsEnumerableType(Type enumerableType)
        {
            return (FindGenericType(typeof(IEnumerable<>), enumerableType) != null);
        }

        public static Type FindGenericType(Type definition, Type type)
        {
            while ((type != null) && (type != typeof(object)))
            {
                if (type.IsGenericType && (type.GetGenericTypeDefinition() == definition))
                {
                    return type;
                }
                if (definition.IsInterface)
                {
                    foreach (Type type2 in type.GetInterfaces())
                    {
                        Type type3 = FindGenericType(definition, type2);
                        if (type3 != null)
                        {
                            return type3;
                        }
                    }
                }
                type = type.BaseType;
            }
            return null;
        }

        public static bool IsKindOfGeneric(Type type, Type definition)
        {
            return (FindGenericType(definition, type) != null);
        }

        public static bool IsNumeric(Type type)
        {
            type = GetNonNullableType(type);
            if (!type.IsEnum)
            {
                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.Char:
                    case TypeCode.SByte:
                    case TypeCode.Byte:
                    case TypeCode.Int16:
                    case TypeCode.UInt16:
                    case TypeCode.Int32:
                    case TypeCode.UInt32:
                    case TypeCode.Int64:
                    case TypeCode.UInt64:
                    case TypeCode.Single:
                    case TypeCode.Double:
                        return true;
                }
            }
            return false;
        }

        public static bool IsSameOrSubclass(Type type, Type subType)
        {
            if (type != subType)
            {
                return subType.IsSubclassOf(type);
            }
            return true;
        }

        public static bool IsScalar(Type type)
        {
            type = GetNonNullableType(type);
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Empty:
                case TypeCode.DBNull:
                    return false;
                case TypeCode.Object:
                    return
                        type == typeof(DateTimeOffset) ||
                        type == typeof(TimeSpan) ||
                        type == typeof(Guid) ||
                        type == typeof(byte[]);
                default:
                    return true;
            }
        }

        public static Type FindLoadedType(string typeName)
        {
            foreach (var assem in AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = assem.GetType(typeName, false, true);
                if (type != null)
                    return type;
            }
            return null;
        }

        public static Assembly Load(string name)
        {
            try
            {
                return Assembly.LoadFrom(name);
            }
            catch { return null; }
        }

        public static Assembly GetAssemblyForNamespace(string nspace)
        {
            foreach (var assem in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assem.FullName.Contains(nspace))
                {
                    return assem;
                }
            }

            return Load(nspace + ".dll");
        }

        public static IEnumerable<Type> FindInstancesIn(Type type, string assemblyName)
        {
            Assembly assembly = GetAssemblyForNamespace(assemblyName);
            if (assembly != null)
            {
                foreach (var atype in assembly.GetTypes())
                {
                    if (string.Compare(atype.Namespace, assemblyName, true) == 0
                        && type.IsAssignableFrom(atype))
                    {
                        yield return atype;
                    }
                }
            }
        }

        public static Type GetElementType(Type seqType)
        {
            Type type = FindIEnumerable(seqType);
            if (type == null)
            {
                return seqType;
            }
            return type.GetGenericArguments()[0];
        }

        public static Type GetFlatSequenceType(Type elementType)
        {
            Type type = FindIEnumerable(elementType);
            if (type != null)
            {
                return type;
            }
            return typeof(IEnumerable<>).MakeGenericType(new Type[] { elementType });
        }

        public static bool HasIEnumerable(Type seqType)
        {
            return (FindIEnumerable(seqType) != null);
        }

        public static bool IsPrivate(PropertyInfo pi)
        {
            MethodInfo info = pi.GetGetMethod() ?? pi.GetSetMethod();
            if (info != null)
            {
                return info.IsPrivate;
            }
            return true;
        }

        public static bool IsSequenceType(Type seqType)
        {
            return ((((seqType != typeof(string)) && (seqType != typeof(byte[]))) && (seqType != typeof(char[]))) && (FindIEnumerable(seqType) != null));
        }
    }
}
