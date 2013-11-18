using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Kiss.Core.Reflection;
using System.ComponentModel;

namespace Kiss.Data
{
    public static class DataConvert
    {
        public static bool ToBoolean(object value)
        {
            if (value == null || value is DBNull)
            {
                return false;
            }

            string s = value as string;
            if (s != null)
            {
                switch (s.ToLowerInvariant())
                {
                    case "y":
                        return true;
                    case "n":
                        return false;
                    case "yes":
                        return true;
                    case "no":
                        return false;
                    case "true":
                        return true;
                    case "false":
                        return false;
                    case "t":
                        return true;
                    case "f":
                        return false;
                    case "1":
                        return true;
                    case "0":
                        return false;
                    case "":
                        return false;
                    default:
                        return Convert.ToBoolean(s);  
                }
            }

            if (value is int)
            {
                int i = Convert.ToInt32(value);
                if (i > 0)
                {
                    return true;
                }
                return false;
            }

            decimal d = 0;
            if (decimal.TryParse(value.ToString(), out d))
            {
                if (d > 0)
                {
                    return true;
                }
                return false;
            }

            return Convert.ToBoolean(value.ToString());            
        }

        public static IList<T> ToList<T>(this IDataReader reader)
        {
            return ToListImpl<T>(reader, null, -1);
        }

        public static List<T> ToList<T>(this IDataReader reader, Func<string, string> mapping)
        {
            return ToListImpl<T>(reader, mapping, -1);
        }

        public static List<T> ToList<T>(this IDataReader reader, int ordinal) where T: struct
        {
            return ToListImpl<T>(reader, null, ordinal);
        }

        private static List<T> ToListSystemType<T>(IDataReader reader, int ordinal) 
        {
            List<T> list = new List<T>();

            Type t = typeof(T);            
            Type underlyingType = t;
            bool nullable = TypeSystem.IsNullableType(t);
            if (nullable)
            {
                underlyingType = TypeSystem.GetNonNullableType(t);
            }
            bool nullAssignable = TypeSystem.IsNullAssignable(t);
            TypeCode typeCode = Type.GetTypeCode(underlyingType);

            bool isGuid = (typeCode == TypeCode.Object && underlyingType == TypeSystem.TypeGuid);
            bool isEnum = underlyingType.IsEnum;
            
            DbFieldReader field = new DbFieldReader(reader);
            while (reader.Read())
            {
                if (nullAssignable)
                {
                    if (field.IsDBNull(ordinal))
                    {
                        list.Add(default(T));
                        continue;
                    }
                }

                T v;
                if (isEnum)
                {
                    if (field.IsDBNull(ordinal))
                    {
                        list.Add(default(T));
                        continue;
                    }
                    v = (T)field.ReadEnum(underlyingType, ordinal);
                    list.Add(v); 
                    continue;
                }
                
                switch (typeCode)
                {
                    case TypeCode.Empty:                       
                    case TypeCode.DBNull:
                        v = default(T); break;                        
                    case TypeCode.Object:
                        if (isGuid)
                        {
                            v = (T)(object)field.ReadGuid(ordinal); break;
                        }
                        v = (T)ChangeTypeTo(field.ReadValue(ordinal), underlyingType); break;
                    default:
                        v = (T)ChangeTypeTo(field.ReadValue(ordinal, typeCode), underlyingType); break;                        
                }

                list.Add(v);
            }

            return list;
        }

        private static List<T> ToListImpl<T>(IDataReader reader, Func<string, string> mapping, int ordinal)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            if (reader.IsClosed || reader.FieldCount == 0)
            {
                throw new Exception("reader is closed or reader does not has fields");
            }

            if(ordinal < 0)
            {
                ordinal = 0;
            }
            if (ordinal > reader.FieldCount - 1)
            {
                throw new Exception(string.Format("field index is invalid, total:{0}, want:{1}", reader.FieldCount, ordinal));
            }

            try
            {
                Type type = typeof(T);
                if (TypeSystem.IsSystemType(type) || type.IsEnum )
                {
                    return ToListSystemType<T>(reader, ordinal);
                }
                if (type.IsEnum)
                {
                    return ToListSystemType<T>(reader, ordinal);
                }
                if (TypeSystem.IsNullableType(type))
                {
                    return ToListSystemType<T>(reader, ordinal);
                }
                else
                {
                    DataReaderConvert<T> convert = new DataReaderConvert<T>(reader);
                    return convert.Convert(reader);
                }
            }
            finally
            {
                reader.Close();
            }
        }

        public static T ChangeTypeTo<T>(object value)
        {
            if (value is DBNull || value == null)
            {
                return default(T);
            }

            return (T)ChangeTypeTo(value, typeof(T));
        }

        public static object ChangeTypeTo(object value, Type conversionType)
        {            
            if (value == DBNull.Value || value == null)
            {
                if (TypeSystem.IsNullAssignable(conversionType))
                {
                    return Activator.CreateInstance(conversionType);
                }
                else
                {
                    return null;
                }
            }

            TypeCode code = Type.GetTypeCode(conversionType);
            if (code == TypeCode.Boolean)
            {
                return ToBoolean(value);
            }

            if (TypeSystem.IsNullableType(conversionType))
            {
                NullableConverter nullableConverter = new NullableConverter(conversionType);
                conversionType = nullableConverter.UnderlyingType;
            }

            return System.Data.Linq.DBConvert.ChangeType(value, conversionType);
        }
    }

}
