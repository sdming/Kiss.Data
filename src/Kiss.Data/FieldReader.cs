using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Data;

namespace Kiss.Data
{
    /// <summary>
    /// IQToolkit
    /// </summary>
    public abstract class FieldReader
    {
        protected TypeCode[] typeCodes;

        public FieldReader()
        {
        }

        protected abstract int FieldCount { get; }
        protected abstract string GetName(int ordinal);
        protected abstract Type GetFieldType(int ordinal);
        protected abstract int GetOrdinal(string name);
        protected abstract object GetValue(int ordinal);        
        public abstract bool IsDBNull(int ordinal);

        #region virtual gets
        protected virtual Boolean GetBoolean(int ordinal)
        {
            return Convert.ToBoolean(GetValue(ordinal));
        }

        protected virtual Char GetChar(int ordinal)
        {
            return Convert.ToChar(GetValue(ordinal));
        }

        protected virtual SByte GetSByte(int ordinal)
        {
            return Convert.ToSByte(GetValue(ordinal));
        }

        protected virtual Byte GetByte(int ordinal)
        {
            return Convert.ToByte(GetValue(ordinal));
        }

        protected virtual Int16 GetInt16(int ordinal)
        {
            return Convert.ToInt16(GetValue(ordinal));
        }

        protected virtual UInt16 GetUInt16(int ordinal)
        {
            return Convert.ToUInt16(GetValue(ordinal));
        }

        protected virtual Int32 GetInt32(int ordinal)
        {
            return Convert.ToInt32(GetValue(ordinal));
        }

        protected virtual UInt32 GetUInt32(int ordinal)
        {
            return Convert.ToUInt32(GetValue(ordinal));
        }

        protected virtual Int64 GetInt64(int ordinal)
        {
            return Convert.ToInt64(GetValue(ordinal));
        }

        protected virtual UInt64 GetUInt64(int ordinal)
        {
            return Convert.ToUInt64(GetValue(ordinal));
        }

        protected virtual Single GetSingle(int ordinal)
        {
            return Convert.ToSingle(GetValue(ordinal));
        }

        protected virtual Double GetDouble(int ordinal)
        {
            return Convert.ToDouble(GetValue(ordinal));
        }

        protected virtual Decimal GetDecimal(int ordinal)
        {
            return Convert.ToDecimal(GetValue(ordinal));
        }

        protected virtual DateTime GetDateTime(int ordinal)
        {
            return Convert.ToDateTime(GetValue(ordinal));
        }

        protected virtual String GetString(int ordinal)
        {
            return Convert.ToString(GetValue(ordinal));
        }

        protected virtual Guid GetGuid(int ordinal)
        {
            return Guid.Parse(Convert.ToString(GetValue(ordinal)));
        }

        protected virtual bool IsEmpty(int ordinal)
        {
            return IsDBNull(ordinal);
        }

        //protected virtual TypeCode GetTypeCode(int ordinal)
        //{
        //    Type type = this.GetFieldType(ordinal);
        //    return Type.GetTypeCode(type);
        //}

        #endregion

        #region read
        protected T ChangeType<T>(object v)
        {
            return System.Data.Linq.DBConvert.ChangeType<T>(v);
        }

        public object ReadEnum(Type enumType, int ordinal)
        {
            switch (typeCodes[ordinal])
            {
                case TypeCode.Byte:
                    return Enum.ToObject(enumType, GetByte(ordinal));
                case TypeCode.Int16:
                    return Enum.ToObject(enumType, GetInt16(ordinal));
                case TypeCode.Int32:
                    return Enum.ToObject(enumType, GetInt32(ordinal));
                case TypeCode.Int64:
                    return Enum.ToObject(enumType, GetInt64(ordinal));
                case TypeCode.SByte:
                    return Enum.ToObject(enumType, GetSByte(ordinal));
                case TypeCode.UInt16:
                    return Enum.ToObject(enumType, GetUInt16(ordinal));
                case TypeCode.UInt32:
                    return Enum.ToObject(enumType, GetUInt32(ordinal));
                case TypeCode.UInt64:
                    return Enum.ToObject(enumType, GetUInt64(ordinal));
                default:
                    return Enum.Parse(enumType, this.GetValue(ordinal).ToString(), true);
            }
        }

        public T ReadValue<T>(int ordinal)
        {
            if (this.IsEmpty(ordinal))
            {
                return default(T);
            }

            switch (typeCodes[ordinal])
            {
                case TypeCode.Empty:
                    return default(T);
                case TypeCode.DBNull:
                    return default(T);
                default:
                    return ChangeType<T>(GetValue(ordinal));                
            }
        }

        public object ReadValue(int ordinal)
        {
            if (this.IsEmpty(ordinal))
            {
                return null;
            }
            return GetValue(ordinal);
        }

        public object ReadValue(int ordinal, TypeCode typeCode)
        {
            switch (typeCode)
            {
                case TypeCode.Empty:
                    return null;
                case TypeCode.DBNull:
                    return null;
                case TypeCode.Object:
                    return ReadValue(ordinal);
                case TypeCode.Boolean:
                    return ReadBoolean(ordinal); 
                case TypeCode.Char:
                    return ReadChar(ordinal);
                case TypeCode.SByte:
                    return ReadSByte(ordinal); 
                case TypeCode.Byte:
                    return ReadByte(ordinal); 
                case TypeCode.Int16:
                    return ReadInt16(ordinal);
                case TypeCode.UInt16:
                    return ReadUInt16(ordinal);
                case TypeCode.Int32:
                    return ReadInt32(ordinal);
                case TypeCode.UInt32:
                    return ReadUInt32(ordinal); 
                case TypeCode.Int64:
                    return ReadInt64(ordinal); 
                case TypeCode.UInt64:
                    return ReadUInt64(ordinal);
                case TypeCode.Single:
                    return ReadSingle(ordinal);
                case TypeCode.Double:
                    return ReadDouble(ordinal); 
                case TypeCode.Decimal:
                    return ReadDecimal(ordinal); 
                case TypeCode.DateTime:
                    return ReadDateTime(ordinal);
                case TypeCode.String:
                    return ReadString(ordinal); 
                default:
                    return ReadValue(ordinal);
            }
        }

        public Guid ReadGuid(int ordinal)
        {            
            if(IsEmpty(ordinal))
            {
                return default(Guid);
            }
            return Guid.Parse(Convert.ToString(GetValue(ordinal)));
        }
        #endregion

        #region generated code

        public Boolean ReadBoolean(int ordinal)
        {
            if (this.IsEmpty(ordinal))
            {
                return default(Boolean);
            }

            Boolean v;
            switch (typeCodes[ordinal])
            {
                case TypeCode.Empty:
                    v = default(Boolean); break;
                case TypeCode.DBNull:
                    v = default(Boolean); break;
                case TypeCode.Object:
                    v = Convert.ToBoolean(GetValue(ordinal)); break;
                case TypeCode.Boolean:
                    v = Convert.ToBoolean(GetBoolean(ordinal)); break;
                case TypeCode.Char:
                    v = Convert.ToBoolean(GetChar(ordinal)); break;
                case TypeCode.SByte:
                    v = Convert.ToBoolean(GetSByte(ordinal)); break;
                case TypeCode.Byte:
                    v = Convert.ToBoolean(GetByte(ordinal)); break;
                case TypeCode.Int16:
                    v = Convert.ToBoolean(GetInt16(ordinal)); break;
                case TypeCode.UInt16:
                    v = Convert.ToBoolean(GetUInt16(ordinal)); break;
                case TypeCode.Int32:
                    v = Convert.ToBoolean(GetInt32(ordinal)); break;
                case TypeCode.UInt32:
                    v = Convert.ToBoolean(GetUInt32(ordinal)); break;
                case TypeCode.Int64:
                    v = Convert.ToBoolean(GetInt64(ordinal)); break;
                case TypeCode.UInt64:
                    v = Convert.ToBoolean(GetUInt64(ordinal)); break;
                case TypeCode.Single:
                    v = Convert.ToBoolean(GetSingle(ordinal)); break;
                case TypeCode.Double:
                    v = Convert.ToBoolean(GetDouble(ordinal)); break;
                case TypeCode.Decimal:
                    v = Convert.ToBoolean(GetDecimal(ordinal)); break;
                case TypeCode.DateTime:
                    v = Convert.ToBoolean(GetDateTime(ordinal)); break;
                case TypeCode.String:
                    v = Convert.ToBoolean(GetString(ordinal)); break;
                default:
                    v = Convert.ToBoolean(GetValue(ordinal)); break;
            }
            return v;
        }

        public Char ReadChar(int ordinal)
        {
            if (this.IsEmpty(ordinal))
            {
                return default(Char);
            }

            Char v;
            switch (typeCodes[ordinal])
            {
                case TypeCode.Empty:
                    v = default(Char); break;
                case TypeCode.DBNull:
                    v = default(Char); break;
                case TypeCode.Object:
                    v = Convert.ToChar(GetValue(ordinal)); break;
                case TypeCode.Boolean:
                    v = Convert.ToChar(GetBoolean(ordinal)); break;
                case TypeCode.Char:
                    v = Convert.ToChar(GetChar(ordinal)); break;
                case TypeCode.SByte:
                    v = Convert.ToChar(GetSByte(ordinal)); break;
                case TypeCode.Byte:
                    v = Convert.ToChar(GetByte(ordinal)); break;
                case TypeCode.Int16:
                    v = Convert.ToChar(GetInt16(ordinal)); break;
                case TypeCode.UInt16:
                    v = Convert.ToChar(GetUInt16(ordinal)); break;
                case TypeCode.Int32:
                    v = Convert.ToChar(GetInt32(ordinal)); break;
                case TypeCode.UInt32:
                    v = Convert.ToChar(GetUInt32(ordinal)); break;
                case TypeCode.Int64:
                    v = Convert.ToChar(GetInt64(ordinal)); break;
                case TypeCode.UInt64:
                    v = Convert.ToChar(GetUInt64(ordinal)); break;
                case TypeCode.Single:
                    v = Convert.ToChar(GetSingle(ordinal)); break;
                case TypeCode.Double:
                    v = Convert.ToChar(GetDouble(ordinal)); break;
                case TypeCode.Decimal:
                    v = Convert.ToChar(GetDecimal(ordinal)); break;
                case TypeCode.DateTime:
                    v = Convert.ToChar(GetDateTime(ordinal)); break;
                case TypeCode.String:
                    v = Convert.ToChar(GetString(ordinal)); break;
                default:
                    v = Convert.ToChar(GetValue(ordinal)); break;
            }
            return v;
        }

        public SByte ReadSByte(int ordinal)
        {
            if (this.IsEmpty(ordinal))
            {
                return default(SByte);
            }

            SByte v;
            switch (typeCodes[ordinal])
            {
                case TypeCode.Empty:
                    v = default(SByte); break;
                case TypeCode.DBNull:
                    v = default(SByte); break;
                case TypeCode.Object:
                    v = Convert.ToSByte(GetValue(ordinal)); break;
                case TypeCode.Boolean:
                    v = Convert.ToSByte(GetBoolean(ordinal)); break;
                case TypeCode.Char:
                    v = Convert.ToSByte(GetChar(ordinal)); break;
                case TypeCode.SByte:
                    v = Convert.ToSByte(GetSByte(ordinal)); break;
                case TypeCode.Byte:
                    v = Convert.ToSByte(GetByte(ordinal)); break;
                case TypeCode.Int16:
                    v = Convert.ToSByte(GetInt16(ordinal)); break;
                case TypeCode.UInt16:
                    v = Convert.ToSByte(GetUInt16(ordinal)); break;
                case TypeCode.Int32:
                    v = Convert.ToSByte(GetInt32(ordinal)); break;
                case TypeCode.UInt32:
                    v = Convert.ToSByte(GetUInt32(ordinal)); break;
                case TypeCode.Int64:
                    v = Convert.ToSByte(GetInt64(ordinal)); break;
                case TypeCode.UInt64:
                    v = Convert.ToSByte(GetUInt64(ordinal)); break;
                case TypeCode.Single:
                    v = Convert.ToSByte(GetSingle(ordinal)); break;
                case TypeCode.Double:
                    v = Convert.ToSByte(GetDouble(ordinal)); break;
                case TypeCode.Decimal:
                    v = Convert.ToSByte(GetDecimal(ordinal)); break;
                case TypeCode.DateTime:
                    v = Convert.ToSByte(GetDateTime(ordinal)); break;
                case TypeCode.String:
                    v = Convert.ToSByte(GetString(ordinal)); break;
                default:
                    v = Convert.ToSByte(GetValue(ordinal)); break;
            }
            return v;
        }

        public Byte ReadByte(int ordinal)
        {
            if (this.IsEmpty(ordinal))
            {
                return default(Byte);
            }

            Byte v;
            switch (typeCodes[ordinal])
            {
                case TypeCode.Empty:
                    v = default(Byte); break;
                case TypeCode.DBNull:
                    v = default(Byte); break;
                case TypeCode.Object:
                    v = Convert.ToByte(GetValue(ordinal)); break;
                case TypeCode.Boolean:
                    v = Convert.ToByte(GetBoolean(ordinal)); break;
                case TypeCode.Char:
                    v = Convert.ToByte(GetChar(ordinal)); break;
                case TypeCode.SByte:
                    v = Convert.ToByte(GetSByte(ordinal)); break;
                case TypeCode.Byte:
                    v = Convert.ToByte(GetByte(ordinal)); break;
                case TypeCode.Int16:
                    v = Convert.ToByte(GetInt16(ordinal)); break;
                case TypeCode.UInt16:
                    v = Convert.ToByte(GetUInt16(ordinal)); break;
                case TypeCode.Int32:
                    v = Convert.ToByte(GetInt32(ordinal)); break;
                case TypeCode.UInt32:
                    v = Convert.ToByte(GetUInt32(ordinal)); break;
                case TypeCode.Int64:
                    v = Convert.ToByte(GetInt64(ordinal)); break;
                case TypeCode.UInt64:
                    v = Convert.ToByte(GetUInt64(ordinal)); break;
                case TypeCode.Single:
                    v = Convert.ToByte(GetSingle(ordinal)); break;
                case TypeCode.Double:
                    v = Convert.ToByte(GetDouble(ordinal)); break;
                case TypeCode.Decimal:
                    v = Convert.ToByte(GetDecimal(ordinal)); break;
                case TypeCode.DateTime:
                    v = Convert.ToByte(GetDateTime(ordinal)); break;
                case TypeCode.String:
                    v = Convert.ToByte(GetString(ordinal)); break;
                default:
                    v = Convert.ToByte(GetValue(ordinal)); break;
            }
            return v;
        }

        public Int16 ReadInt16(int ordinal)
        {
            if (this.IsEmpty(ordinal))
            {
                return default(Int16);
            }

            Int16 v;
            switch (typeCodes[ordinal])
            {
                case TypeCode.Empty:
                    v = default(Int16); break;
                case TypeCode.DBNull:
                    v = default(Int16); break;
                case TypeCode.Object:
                    v = Convert.ToInt16(GetValue(ordinal)); break;
                case TypeCode.Boolean:
                    v = Convert.ToInt16(GetBoolean(ordinal)); break;
                case TypeCode.Char:
                    v = Convert.ToInt16(GetChar(ordinal)); break;
                case TypeCode.SByte:
                    v = Convert.ToInt16(GetSByte(ordinal)); break;
                case TypeCode.Byte:
                    v = Convert.ToInt16(GetByte(ordinal)); break;
                case TypeCode.Int16:
                    v = Convert.ToInt16(GetInt16(ordinal)); break;
                case TypeCode.UInt16:
                    v = Convert.ToInt16(GetUInt16(ordinal)); break;
                case TypeCode.Int32:
                    v = Convert.ToInt16(GetInt32(ordinal)); break;
                case TypeCode.UInt32:
                    v = Convert.ToInt16(GetUInt32(ordinal)); break;
                case TypeCode.Int64:
                    v = Convert.ToInt16(GetInt64(ordinal)); break;
                case TypeCode.UInt64:
                    v = Convert.ToInt16(GetUInt64(ordinal)); break;
                case TypeCode.Single:
                    v = Convert.ToInt16(GetSingle(ordinal)); break;
                case TypeCode.Double:
                    v = Convert.ToInt16(GetDouble(ordinal)); break;
                case TypeCode.Decimal:
                    v = Convert.ToInt16(GetDecimal(ordinal)); break;
                case TypeCode.DateTime:
                    v = Convert.ToInt16(GetDateTime(ordinal)); break;
                case TypeCode.String:
                    v = Convert.ToInt16(GetString(ordinal)); break;
                default:
                    v = Convert.ToInt16(GetValue(ordinal)); break;
            }
            return v;
        }

        public UInt16 ReadUInt16(int ordinal)
        {
            if (this.IsEmpty(ordinal))
            {
                return default(UInt16);
            }

            UInt16 v;
            switch (typeCodes[ordinal])
            {
                case TypeCode.Empty:
                    v = default(UInt16); break;
                case TypeCode.DBNull:
                    v = default(UInt16); break;
                case TypeCode.Object:
                    v = Convert.ToUInt16(GetValue(ordinal)); break;
                case TypeCode.Boolean:
                    v = Convert.ToUInt16(GetBoolean(ordinal)); break;
                case TypeCode.Char:
                    v = Convert.ToUInt16(GetChar(ordinal)); break;
                case TypeCode.SByte:
                    v = Convert.ToUInt16(GetSByte(ordinal)); break;
                case TypeCode.Byte:
                    v = Convert.ToUInt16(GetByte(ordinal)); break;
                case TypeCode.Int16:
                    v = Convert.ToUInt16(GetInt16(ordinal)); break;
                case TypeCode.UInt16:
                    v = Convert.ToUInt16(GetUInt16(ordinal)); break;
                case TypeCode.Int32:
                    v = Convert.ToUInt16(GetInt32(ordinal)); break;
                case TypeCode.UInt32:
                    v = Convert.ToUInt16(GetUInt32(ordinal)); break;
                case TypeCode.Int64:
                    v = Convert.ToUInt16(GetInt64(ordinal)); break;
                case TypeCode.UInt64:
                    v = Convert.ToUInt16(GetUInt64(ordinal)); break;
                case TypeCode.Single:
                    v = Convert.ToUInt16(GetSingle(ordinal)); break;
                case TypeCode.Double:
                    v = Convert.ToUInt16(GetDouble(ordinal)); break;
                case TypeCode.Decimal:
                    v = Convert.ToUInt16(GetDecimal(ordinal)); break;
                case TypeCode.DateTime:
                    v = Convert.ToUInt16(GetDateTime(ordinal)); break;
                case TypeCode.String:
                    v = Convert.ToUInt16(GetString(ordinal)); break;
                default:
                    v = Convert.ToUInt16(GetValue(ordinal)); break;
            }
            return v;
        }

        public Int32 ReadInt32(int ordinal)
        {
            if (this.IsEmpty(ordinal))
            {
                return default(Int32);
            }

            Int32 v;
            switch (typeCodes[ordinal])
            {
                case TypeCode.Empty:
                    v = default(Int32); break;
                case TypeCode.DBNull:
                    v = default(Int32); break;
                case TypeCode.Object:
                    v = Convert.ToInt32(GetValue(ordinal)); break;
                case TypeCode.Boolean:
                    v = Convert.ToInt32(GetBoolean(ordinal)); break;
                case TypeCode.Char:
                    v = Convert.ToInt32(GetChar(ordinal)); break;
                case TypeCode.SByte:
                    v = Convert.ToInt32(GetSByte(ordinal)); break;
                case TypeCode.Byte:
                    v = Convert.ToInt32(GetByte(ordinal)); break;
                case TypeCode.Int16:
                    v = Convert.ToInt32(GetInt16(ordinal)); break;
                case TypeCode.UInt16:
                    v = Convert.ToInt32(GetUInt16(ordinal)); break;
                case TypeCode.Int32:
                    v = Convert.ToInt32(GetInt32(ordinal)); break;
                case TypeCode.UInt32:
                    v = Convert.ToInt32(GetUInt32(ordinal)); break;
                case TypeCode.Int64:
                    v = Convert.ToInt32(GetInt64(ordinal)); break;
                case TypeCode.UInt64:
                    v = Convert.ToInt32(GetUInt64(ordinal)); break;
                case TypeCode.Single:
                    v = Convert.ToInt32(GetSingle(ordinal)); break;
                case TypeCode.Double:
                    v = Convert.ToInt32(GetDouble(ordinal)); break;
                case TypeCode.Decimal:
                    v = Convert.ToInt32(GetDecimal(ordinal)); break;
                case TypeCode.DateTime:
                    v = Convert.ToInt32(GetDateTime(ordinal)); break;
                case TypeCode.String:
                    v = Convert.ToInt32(GetString(ordinal)); break;
                default:
                    v = Convert.ToInt32(GetValue(ordinal)); break;
            }
            return v;
        }

        public UInt32 ReadUInt32(int ordinal)
        {
            if (this.IsEmpty(ordinal))
            {
                return default(UInt32);
            }

            UInt32 v;
            switch (typeCodes[ordinal])
            {
                case TypeCode.Empty:
                    v = default(UInt32); break;
                case TypeCode.DBNull:
                    v = default(UInt32); break;
                case TypeCode.Object:
                    v = Convert.ToUInt32(GetValue(ordinal)); break;
                case TypeCode.Boolean:
                    v = Convert.ToUInt32(GetBoolean(ordinal)); break;
                case TypeCode.Char:
                    v = Convert.ToUInt32(GetChar(ordinal)); break;
                case TypeCode.SByte:
                    v = Convert.ToUInt32(GetSByte(ordinal)); break;
                case TypeCode.Byte:
                    v = Convert.ToUInt32(GetByte(ordinal)); break;
                case TypeCode.Int16:
                    v = Convert.ToUInt32(GetInt16(ordinal)); break;
                case TypeCode.UInt16:
                    v = Convert.ToUInt32(GetUInt16(ordinal)); break;
                case TypeCode.Int32:
                    v = Convert.ToUInt32(GetInt32(ordinal)); break;
                case TypeCode.UInt32:
                    v = Convert.ToUInt32(GetUInt32(ordinal)); break;
                case TypeCode.Int64:
                    v = Convert.ToUInt32(GetInt64(ordinal)); break;
                case TypeCode.UInt64:
                    v = Convert.ToUInt32(GetUInt64(ordinal)); break;
                case TypeCode.Single:
                    v = Convert.ToUInt32(GetSingle(ordinal)); break;
                case TypeCode.Double:
                    v = Convert.ToUInt32(GetDouble(ordinal)); break;
                case TypeCode.Decimal:
                    v = Convert.ToUInt32(GetDecimal(ordinal)); break;
                case TypeCode.DateTime:
                    v = Convert.ToUInt32(GetDateTime(ordinal)); break;
                case TypeCode.String:
                    v = Convert.ToUInt32(GetString(ordinal)); break;
                default:
                    v = Convert.ToUInt32(GetValue(ordinal)); break;
            }
            return v;
        }

        public Int64 ReadInt64(int ordinal)
        {
            if (this.IsEmpty(ordinal))
            {
                return default(Int64);
            }

            Int64 v;
            switch (typeCodes[ordinal])
            {
                case TypeCode.Empty:
                    v = default(Int64); break;
                case TypeCode.DBNull:
                    v = default(Int64); break;
                case TypeCode.Object:
                    v = Convert.ToInt64(GetValue(ordinal)); break;
                case TypeCode.Boolean:
                    v = Convert.ToInt64(GetBoolean(ordinal)); break;
                case TypeCode.Char:
                    v = Convert.ToInt64(GetChar(ordinal)); break;
                case TypeCode.SByte:
                    v = Convert.ToInt64(GetSByte(ordinal)); break;
                case TypeCode.Byte:
                    v = Convert.ToInt64(GetByte(ordinal)); break;
                case TypeCode.Int16:
                    v = Convert.ToInt64(GetInt16(ordinal)); break;
                case TypeCode.UInt16:
                    v = Convert.ToInt64(GetUInt16(ordinal)); break;
                case TypeCode.Int32:
                    v = Convert.ToInt64(GetInt32(ordinal)); break;
                case TypeCode.UInt32:
                    v = Convert.ToInt64(GetUInt32(ordinal)); break;
                case TypeCode.Int64:
                    v = Convert.ToInt64(GetInt64(ordinal)); break;
                case TypeCode.UInt64:
                    v = Convert.ToInt64(GetUInt64(ordinal)); break;
                case TypeCode.Single:
                    v = Convert.ToInt64(GetSingle(ordinal)); break;
                case TypeCode.Double:
                    v = Convert.ToInt64(GetDouble(ordinal)); break;
                case TypeCode.Decimal:
                    v = Convert.ToInt64(GetDecimal(ordinal)); break;
                case TypeCode.DateTime:
                    v = Convert.ToInt64(GetDateTime(ordinal)); break;
                case TypeCode.String:
                    v = Convert.ToInt64(GetString(ordinal)); break;
                default:
                    v = Convert.ToInt64(GetValue(ordinal)); break;
            }
            return v;
        }

        public UInt64 ReadUInt64(int ordinal)
        {
            if (this.IsEmpty(ordinal))
            {
                return default(UInt64);
            }

            UInt64 v;
            switch (typeCodes[ordinal])
            {
                case TypeCode.Empty:
                    v = default(UInt64); break;
                case TypeCode.DBNull:
                    v = default(UInt64); break;
                case TypeCode.Object:
                    v = Convert.ToUInt64(GetValue(ordinal)); break;
                case TypeCode.Boolean:
                    v = Convert.ToUInt64(GetBoolean(ordinal)); break;
                case TypeCode.Char:
                    v = Convert.ToUInt64(GetChar(ordinal)); break;
                case TypeCode.SByte:
                    v = Convert.ToUInt64(GetSByte(ordinal)); break;
                case TypeCode.Byte:
                    v = Convert.ToUInt64(GetByte(ordinal)); break;
                case TypeCode.Int16:
                    v = Convert.ToUInt64(GetInt16(ordinal)); break;
                case TypeCode.UInt16:
                    v = Convert.ToUInt64(GetUInt16(ordinal)); break;
                case TypeCode.Int32:
                    v = Convert.ToUInt64(GetInt32(ordinal)); break;
                case TypeCode.UInt32:
                    v = Convert.ToUInt64(GetUInt32(ordinal)); break;
                case TypeCode.Int64:
                    v = Convert.ToUInt64(GetInt64(ordinal)); break;
                case TypeCode.UInt64:
                    v = Convert.ToUInt64(GetUInt64(ordinal)); break;
                case TypeCode.Single:
                    v = Convert.ToUInt64(GetSingle(ordinal)); break;
                case TypeCode.Double:
                    v = Convert.ToUInt64(GetDouble(ordinal)); break;
                case TypeCode.Decimal:
                    v = Convert.ToUInt64(GetDecimal(ordinal)); break;
                case TypeCode.DateTime:
                    v = Convert.ToUInt64(GetDateTime(ordinal)); break;
                case TypeCode.String:
                    v = Convert.ToUInt64(GetString(ordinal)); break;
                default:
                    v = Convert.ToUInt64(GetValue(ordinal)); break;
            }
            return v;
        }

        public Single ReadSingle(int ordinal)
        {
            if (this.IsEmpty(ordinal))
            {
                return default(Single);
            }

            Single v;
            switch (typeCodes[ordinal])
            {
                case TypeCode.Empty:
                    v = default(Single); break;
                case TypeCode.DBNull:
                    v = default(Single); break;
                case TypeCode.Object:
                    v = Convert.ToSingle(GetValue(ordinal)); break;
                case TypeCode.Boolean:
                    v = Convert.ToSingle(GetBoolean(ordinal)); break;
                case TypeCode.Char:
                    v = Convert.ToSingle(GetChar(ordinal)); break;
                case TypeCode.SByte:
                    v = Convert.ToSingle(GetSByte(ordinal)); break;
                case TypeCode.Byte:
                    v = Convert.ToSingle(GetByte(ordinal)); break;
                case TypeCode.Int16:
                    v = Convert.ToSingle(GetInt16(ordinal)); break;
                case TypeCode.UInt16:
                    v = Convert.ToSingle(GetUInt16(ordinal)); break;
                case TypeCode.Int32:
                    v = Convert.ToSingle(GetInt32(ordinal)); break;
                case TypeCode.UInt32:
                    v = Convert.ToSingle(GetUInt32(ordinal)); break;
                case TypeCode.Int64:
                    v = Convert.ToSingle(GetInt64(ordinal)); break;
                case TypeCode.UInt64:
                    v = Convert.ToSingle(GetUInt64(ordinal)); break;
                case TypeCode.Single:
                    v = Convert.ToSingle(GetSingle(ordinal)); break;
                case TypeCode.Double:
                    v = Convert.ToSingle(GetDouble(ordinal)); break;
                case TypeCode.Decimal:
                    v = Convert.ToSingle(GetDecimal(ordinal)); break;
                case TypeCode.DateTime:
                    v = Convert.ToSingle(GetDateTime(ordinal)); break;
                case TypeCode.String:
                    v = Convert.ToSingle(GetString(ordinal)); break;
                default:
                    v = Convert.ToSingle(GetValue(ordinal)); break;
            }
            return v;
        }

        public Double ReadDouble(int ordinal)
        {
            if (this.IsEmpty(ordinal))
            {
                return default(Double);
            }

            Double v;
            switch (typeCodes[ordinal])
            {
                case TypeCode.Empty:
                    v = default(Double); break;
                case TypeCode.DBNull:
                    v = default(Double); break;
                case TypeCode.Object:
                    v = Convert.ToDouble(GetValue(ordinal)); break;
                case TypeCode.Boolean:
                    v = Convert.ToDouble(GetBoolean(ordinal)); break;
                case TypeCode.Char:
                    v = Convert.ToDouble(GetChar(ordinal)); break;
                case TypeCode.SByte:
                    v = Convert.ToDouble(GetSByte(ordinal)); break;
                case TypeCode.Byte:
                    v = Convert.ToDouble(GetByte(ordinal)); break;
                case TypeCode.Int16:
                    v = Convert.ToDouble(GetInt16(ordinal)); break;
                case TypeCode.UInt16:
                    v = Convert.ToDouble(GetUInt16(ordinal)); break;
                case TypeCode.Int32:
                    v = Convert.ToDouble(GetInt32(ordinal)); break;
                case TypeCode.UInt32:
                    v = Convert.ToDouble(GetUInt32(ordinal)); break;
                case TypeCode.Int64:
                    v = Convert.ToDouble(GetInt64(ordinal)); break;
                case TypeCode.UInt64:
                    v = Convert.ToDouble(GetUInt64(ordinal)); break;
                case TypeCode.Single:
                    v = Convert.ToDouble(GetSingle(ordinal)); break;
                case TypeCode.Double:
                    v = Convert.ToDouble(GetDouble(ordinal)); break;
                case TypeCode.Decimal:
                    v = Convert.ToDouble(GetDecimal(ordinal)); break;
                case TypeCode.DateTime:
                    v = Convert.ToDouble(GetDateTime(ordinal)); break;
                case TypeCode.String:
                    v = Convert.ToDouble(GetString(ordinal)); break;
                default:
                    v = Convert.ToDouble(GetValue(ordinal)); break;
            }
            return v;
        }

        public Decimal ReadDecimal(int ordinal)
        {
            if (this.IsEmpty(ordinal))
            {
                return default(Decimal);
            }

            Decimal v;
            switch (typeCodes[ordinal])
            {
                case TypeCode.Empty:
                    v = default(Decimal); break;
                case TypeCode.DBNull:
                    v = default(Decimal); break;
                case TypeCode.Object:
                    v = Convert.ToDecimal(GetValue(ordinal)); break;
                case TypeCode.Boolean:
                    v = Convert.ToDecimal(GetBoolean(ordinal)); break;
                case TypeCode.Char:
                    v = Convert.ToDecimal(GetChar(ordinal)); break;
                case TypeCode.SByte:
                    v = Convert.ToDecimal(GetSByte(ordinal)); break;
                case TypeCode.Byte:
                    v = Convert.ToDecimal(GetByte(ordinal)); break;
                case TypeCode.Int16:
                    v = Convert.ToDecimal(GetInt16(ordinal)); break;
                case TypeCode.UInt16:
                    v = Convert.ToDecimal(GetUInt16(ordinal)); break;
                case TypeCode.Int32:
                    v = Convert.ToDecimal(GetInt32(ordinal)); break;
                case TypeCode.UInt32:
                    v = Convert.ToDecimal(GetUInt32(ordinal)); break;
                case TypeCode.Int64:
                    v = Convert.ToDecimal(GetInt64(ordinal)); break;
                case TypeCode.UInt64:
                    v = Convert.ToDecimal(GetUInt64(ordinal)); break;
                case TypeCode.Single:
                    v = Convert.ToDecimal(GetSingle(ordinal)); break;
                case TypeCode.Double:
                    v = Convert.ToDecimal(GetDouble(ordinal)); break;
                case TypeCode.Decimal:
                    v = Convert.ToDecimal(GetDecimal(ordinal)); break;
                case TypeCode.DateTime:
                    v = Convert.ToDecimal(GetDateTime(ordinal)); break;
                case TypeCode.String:
                    v = Convert.ToDecimal(GetString(ordinal)); break;
                default:
                    v = Convert.ToDecimal(GetValue(ordinal)); break;
            }
            return v;
        }

        public DateTime ReadDateTime(int ordinal)
        {
            if (this.IsEmpty(ordinal))
            {
                return default(DateTime);
            }

            DateTime v;
            switch (typeCodes[ordinal])
            {
                case TypeCode.Empty:
                    v = default(DateTime); break;
                case TypeCode.DBNull:
                    v = default(DateTime); break;
                case TypeCode.Object:
                    v = Convert.ToDateTime(GetValue(ordinal)); break;
                case TypeCode.Boolean:
                    v = Convert.ToDateTime(GetBoolean(ordinal)); break;
                case TypeCode.Char:
                    v = Convert.ToDateTime(GetChar(ordinal)); break;
                case TypeCode.SByte:
                    v = Convert.ToDateTime(GetSByte(ordinal)); break;
                case TypeCode.Byte:
                    v = Convert.ToDateTime(GetByte(ordinal)); break;
                case TypeCode.Int16:
                    v = Convert.ToDateTime(GetInt16(ordinal)); break;
                case TypeCode.UInt16:
                    v = Convert.ToDateTime(GetUInt16(ordinal)); break;
                case TypeCode.Int32:
                    v = Convert.ToDateTime(GetInt32(ordinal)); break;
                case TypeCode.UInt32:
                    v = Convert.ToDateTime(GetUInt32(ordinal)); break;
                case TypeCode.Int64:
                    v = Convert.ToDateTime(GetInt64(ordinal)); break;
                case TypeCode.UInt64:
                    v = Convert.ToDateTime(GetUInt64(ordinal)); break;
                case TypeCode.Single:
                    v = Convert.ToDateTime(GetSingle(ordinal)); break;
                case TypeCode.Double:
                    v = Convert.ToDateTime(GetDouble(ordinal)); break;
                case TypeCode.Decimal:
                    v = Convert.ToDateTime(GetDecimal(ordinal)); break;
                case TypeCode.DateTime:
                    v = Convert.ToDateTime(GetDateTime(ordinal)); break;
                case TypeCode.String:
                    v = Convert.ToDateTime(GetString(ordinal)); break;
                default:
                    v = Convert.ToDateTime(GetValue(ordinal)); break;
            }
            return v;
        }

        public String ReadString(int ordinal)
        {
            if (this.IsEmpty(ordinal))
            {
                return default(String);
            }

            String v;
            switch (typeCodes[ordinal])
            {
                case TypeCode.Empty:
                    v = default(String); break;
                case TypeCode.DBNull:
                    v = default(String); break;
                case TypeCode.Object:
                    v = Convert.ToString(GetValue(ordinal)); break;
                case TypeCode.Boolean:
                    v = Convert.ToString(GetBoolean(ordinal)); break;
                case TypeCode.Char:
                    v = Convert.ToString(GetChar(ordinal)); break;
                case TypeCode.SByte:
                    v = Convert.ToString(GetSByte(ordinal)); break;
                case TypeCode.Byte:
                    v = Convert.ToString(GetByte(ordinal)); break;
                case TypeCode.Int16:
                    v = Convert.ToString(GetInt16(ordinal)); break;
                case TypeCode.UInt16:
                    v = Convert.ToString(GetUInt16(ordinal)); break;
                case TypeCode.Int32:
                    v = Convert.ToString(GetInt32(ordinal)); break;
                case TypeCode.UInt32:
                    v = Convert.ToString(GetUInt32(ordinal)); break;
                case TypeCode.Int64:
                    v = Convert.ToString(GetInt64(ordinal)); break;
                case TypeCode.UInt64:
                    v = Convert.ToString(GetUInt64(ordinal)); break;
                case TypeCode.Single:
                    v = Convert.ToString(GetSingle(ordinal)); break;
                case TypeCode.Double:
                    v = Convert.ToString(GetDouble(ordinal)); break;
                case TypeCode.Decimal:
                    v = Convert.ToString(GetDecimal(ordinal)); break;
                case TypeCode.DateTime:
                    v = Convert.ToString(GetDateTime(ordinal)); break;
                case TypeCode.String:
                    v = Convert.ToString(GetString(ordinal)); break;
                default:
                    v = Convert.ToString(GetValue(ordinal)); break;
            }
            return v;
        }


        #endregion
    }

    public class DbFieldReader : FieldReader
    {
        IDataReader reader;

        public IDataReader Source
        {
            get { return reader; }
        }

        public DbFieldReader(IDataReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            this.reader = reader;
            Init();
        }

        private void Init()
        {
            int count = reader.FieldCount;
            typeCodes = new TypeCode[count];
            for (var i = 0; i < count; i++)
            {
                typeCodes[i] = Type.GetTypeCode(reader.GetFieldType(i));
            }
        }

        #region override
        protected override int FieldCount
        {
            get { return this.reader.FieldCount; }
        }

        protected override string GetName(int ordinal)
        {
            return this.reader.GetName(ordinal);
        }

        protected override Type GetFieldType(int ordinal)
        {
            return this.reader.GetFieldType(ordinal);
        }

        protected override int GetOrdinal(string name)
        {
            return this.reader.GetOrdinal(name);
        }

        public override bool IsDBNull(int ordinal)
        {
            return this.reader.IsDBNull(ordinal);
        }

        protected override object GetValue(int ordinal)
        {
            return this.reader.GetValue(ordinal);
        }

        protected override Boolean GetBoolean(int ordinal)
        {
            return this.reader.GetBoolean(ordinal);
        }

        protected override Char GetChar(int ordinal)
        {
            return this.reader.GetChar(ordinal);
        }

        protected override Byte GetByte(int ordinal)
        {
            return this.reader.GetByte(ordinal);
        }

        protected override Int16 GetInt16(int ordinal)
        {
            return this.reader.GetInt16(ordinal);
        }

        protected override Int32 GetInt32(int ordinal)
        {
            return this.reader.GetInt32(ordinal);
        }

        protected override Int64 GetInt64(int ordinal)
        {
            return this.reader.GetInt64(ordinal);
        }

        protected override Single GetSingle(int ordinal)
        {
            return this.reader.GetFloat(ordinal);
        }

        protected override Double GetDouble(int ordinal)
        {
            return this.reader.GetDouble(ordinal);
        }

        protected override Decimal GetDecimal(int ordinal)
        {
            return this.reader.GetDecimal(ordinal);
        }

        protected override DateTime GetDateTime(int ordinal)
        {
            return this.reader.GetDateTime(ordinal);
        }

        protected override String GetString(int ordinal)
        {
            return this.reader.GetString(ordinal);
        }

        protected override Guid GetGuid(int ordinal)
        {
            return this.reader.GetGuid(ordinal);
        }

        #endregion
    }
}



