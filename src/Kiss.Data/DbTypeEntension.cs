using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Xml;

namespace Kiss.Data
{
    /// <summary>
    /// DbTypeEntension
    /// </summary>
    internal static class DbTypeEntension
    {
        /// <summary>
        /// return default value of DbType
        /// </summary>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public static object Default(this DbType dbType)
        {
            if (dbType.IsNumeric())
            {
                return 0;
            }
            else if (dbType.IsDateTime())
            {
                return Defaults.DateTime;
            }
            else if (dbType.IsString())
            {
                return string.Empty;
            }
            else if (dbType.IsBoolean())
            {
                return false;
            }
            else if (dbType == DbType.Guid)
            {
                return Guid.Empty;
            }
            else if (dbType == DbType.Xml)
            {
                return string.Empty;
            }

            return null;
        }

        /// <summary>
        /// IsBoolean
        /// </summary>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public static bool IsBoolean(this DbType dbType)
        {
            return dbType == DbType.Boolean;
        }

        /// <summary>
        /// IsInteger
        /// </summary>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public static bool IsInteger(this DbType dbType)
        {
            return dbType == DbType.Byte ||
                      dbType == DbType.SByte ||
                      dbType == DbType.Int16 ||
                      dbType == DbType.Int32 ||
                      dbType == DbType.Int64 ||
                      dbType == DbType.UInt16 ||
                      dbType == DbType.UInt32 ||
                      dbType == DbType.UInt64 ||
                      dbType == DbType.VarNumeric;
        }

        /// <summary>
        /// IsNumeric
        /// </summary>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public static bool IsNumeric(this DbType dbType)
        {
            return dbType == DbType.Currency ||
                      dbType == DbType.Decimal ||
                      dbType == DbType.Double ||
                      dbType == DbType.Int16 ||
                      dbType == DbType.Int32 ||
                      dbType == DbType.Int64 ||
                      dbType == DbType.Single ||
                      dbType == DbType.UInt16 ||
                      dbType == DbType.UInt32 ||
                      dbType == DbType.UInt64 ||
                      dbType == DbType.VarNumeric;
        }

        /// <summary>
        /// IsDateTime
        /// </summary>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public static bool IsDateTime(this DbType dbType)
        {
            return dbType == DbType.DateTime ||
                    dbType == DbType.DateTime2 ||
                    dbType == DbType.DateTimeOffset ||
                    dbType == DbType.Time ||
                    dbType == DbType.Date;
        }

        /// <summary>
        /// IsString
        /// </summary>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public static bool IsString(this DbType dbType)
        {
            return dbType == DbType.AnsiString ||
                   dbType == DbType.AnsiStringFixedLength ||
                   dbType == DbType.String ||
                   dbType == DbType.StringFixedLength;

        }

        public static bool HasSize(this DbType dbType)
        {
            switch (dbType)
            {
                case DbType.AnsiString:
                case DbType.AnsiStringFixedLength:
                case DbType.String:
                case DbType.StringFixedLength:
                case DbType.Binary:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// HasPrecisionAndScale
        /// </summary>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public static bool HasPrecisionAndScale(this DbType dbType)
        {
            switch (dbType)
            {
                case DbType.Time:
                case DbType.DateTime2:
                case DbType.DateTimeOffset:
                case DbType.Currency:
                case DbType.Decimal:
                case DbType.Double:
                case DbType.Single:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// GetDbType
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static DbType GetDbType(Type type)
        {
            if (type == null)
            {
                return DbType.Object;
            }

            if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Nullable<>)))
            {
                type = type.GetGenericArguments()[0];
            }

            TypeCode code = Type.GetTypeCode(type);

            switch (code)
            {
                case TypeCode.Boolean:
                    return DbType.Boolean;
                case TypeCode.Byte:
                    return DbType.Byte;
                case TypeCode.Char:
                    return DbType.StringFixedLength;
                case TypeCode.DateTime:
                    return DbType.DateTime;
                case TypeCode.DBNull:
                    return DbType.Object;
                case TypeCode.Decimal:
                    return DbType.Decimal;
                case TypeCode.Double:
                    return DbType.Double;
                case TypeCode.Empty:
                    return DbType.Object;
                case TypeCode.Int16:
                    return DbType.Int16;
                case TypeCode.Int32:
                    return DbType.Int32;
                case TypeCode.Int64:
                    return DbType.Int64;
                case TypeCode.Object:
                    if (type == typeof(Guid))
                    {
                        return DbType.Guid;
                    }
                    else if (type == typeof(byte[]))
                    {
                        return DbType.Binary;
                    }
                    else if (type == typeof(char[]))
                    {
                        return DbType.String;
                    }
                    else if (type == typeof(TimeSpan))
                    {
                        return DbType.Int64;
                    }
                    else if (type == typeof(DateTimeOffset))
                    {
                        return DbType.DateTimeOffset;
                    }
                    if ((type == typeof(XmlDocument)) || (type == typeof(XmlElement)) || (type == typeof(XmlNode)))
                    {
                        return DbType.Xml;
                    }
                    return DbType.Object;
                case TypeCode.SByte:
                    return DbType.Byte;
                case TypeCode.Single:
                    return DbType.Single;
                case TypeCode.String:
                    return DbType.String;
                case TypeCode.UInt16:
                    return DbType.Int32;
                case TypeCode.UInt32:
                    return DbType.Int64;
                case TypeCode.UInt64:
                    return DbType.Int64;
            }

            return DbType.Object;
        }

        /// <summary>
        /// GetDbType
        /// </summary>
        /// <param name="type"></param>
        /// <param name="isNullable"></param>
        /// <returns></returns>
        public static DbType GetDbType(Type type, bool isNullable)
        {
            if (isNullable)
            {
                var nullType = Nullable.GetUnderlyingType(type);
                return nullType.IsEnum ? GetEnumDbType(nullType) : GetDbType(nullType);
            }
            return type.IsEnum ? GetEnumDbType(type) : GetDbType(type);
        }

        /// <summary>
        /// GetEnumDbType
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static DbType GetEnumDbType(Type type)
        {
            return GetDbType(Enum.GetUnderlyingType(type));
        }

    }
}
