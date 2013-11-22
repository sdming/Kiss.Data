using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Kiss.Core;
using Kiss.Core.Reflection;

namespace Kiss.Data
{
    public class DataReaderConvert<T> 
    {
        #region properties
        public Type TargetType {get; private set;}
        private bool IsAnonymous { get; set; }
        private TypeMeta TargetMeta { get; set; }
        private int FieldCount {get; set;}
        private ColumnInfo[] Columns { get; set; }

        private class ColumnInfo
        {
            public int SourceIndex;
            public Type SourceType;
            public TypeCode SourceTypeCode;
            public bool Enable;
            public MemberMeta TargetMeta;
            public TypeCode TargetUnderlyingTypeCode;
            public bool TargetIsNullable;
            public bool TargetIsEnum;
            public bool TargetIsGuid;
            public Type TargetUnderlyingType;
        }

        #endregion

        #region DataReaderConvert
        public DataReaderConvert(IDataReader reader)
            : this(reader, null)
        {
        }

        public DataReaderConvert(IDataReader reader, Func<string, string> mapping)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            if (reader.IsClosed)
            {
                throw new Exception("reader is closed");
            }

            this.TargetType = typeof(T);
            this.FieldCount = reader.FieldCount;
            if (TargetType.Name.Contains("AnonymousType"))
            {
                this.IsAnonymous = true;
            }
            this.TargetMeta = TypeMetaFactory.CreateTypeMeta(TargetType);
            CreateColumnsMapping(mapping, reader);
          
        }

        private void CreateColumnsMapping(Func<string, string> mapping, IDataReader reader)
        {
            Columns =  new ColumnInfo[FieldCount];       
            for (int i = 0; i < FieldCount; i++)
            {
                ColumnInfo column = new ColumnInfo();
                column.SourceIndex = i;
                column.SourceType = reader.GetFieldType(i);
                column.SourceTypeCode = Type.GetTypeCode(reader.GetFieldType(i));
                Columns[i] = column;

                string targetName = reader.GetName(i);
                if (mapping != null)
                {
                    targetName = mapping(targetName);
                }

                if (string.IsNullOrEmpty(targetName))
                {
                    column.Enable = false;
                }
                else
                {
                    column.TargetMeta = TargetMeta.TryGetMember(targetName);
                    column.Enable = column.TargetMeta != null;
                }

                if (!column.Enable)
                {
                    continue;
                }

                column.TargetIsNullable = column.TargetMeta.IsNullable;  
                  
                if (column.TargetIsNullable)
                {
                    column.TargetUnderlyingTypeCode = Type.GetTypeCode(column.TargetMeta.MemberType.GetGenericArguments()[0]);
                    column.TargetIsEnum = Nullable.GetUnderlyingType(column.TargetMeta.MemberType).IsEnum;
                    if (column.TargetIsEnum)
                    {
                        column.TargetUnderlyingType = Nullable.GetUnderlyingType(column.TargetMeta.MemberType);
                    }
                    column.TargetIsGuid = column.TargetUnderlyingTypeCode == TypeCode.Object && column.TargetUnderlyingType == TypeSystem.TypeGuid;
                }
                else
                {
                    column.TargetUnderlyingTypeCode = column.TargetMeta.MemberTypeCode;
                    column.TargetIsEnum = column.TargetMeta.MemberType.IsEnum;
                    if (column.TargetIsEnum)
                    {
                        column.TargetUnderlyingType = column.TargetMeta.MemberType;
                    }
                    column.TargetIsGuid = column.TargetUnderlyingTypeCode == TypeCode.Object && column.TargetUnderlyingType == TypeSystem.TypeGuid;
                }                               
            }
        }

        #endregion

        public List<T> Convert(IDataReader reader)
        {
            if(reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            if (reader.IsClosed)
            {
                throw new Exception("reader is closed");
            }

            DbFieldReader fieldReader = new DbFieldReader(reader);
            List<T> list = new List<T>();

            try
            {
                while (reader.Read())
                {
                    T item;
                    if (IsAnonymous)
                    {
                        item = CreateAnonymous(reader);
                    }
                    else
                    {
                        item = CreateClass(fieldReader);
                    }

                    list.Add(item);
                }
            }
            catch 
            {
                //log excetion ??
                throw;
            }
            finally
            {
                if (!reader.IsClosed)
                {
                    reader.Close();
                }
            }

            return list;
        }

       
        private T CreateAnonymous(IDataReader reader)
        {
            //http://stackoverflow.com/questions/478013/how-do-i-create-and-access-a-new-instance-of-an-anonymous-class-passed-as-a-param
            var properties = TargetMeta.Properties();
            int index = 0;
            object[] objArray = new object[properties.Count];

            foreach (PropertyInfo info in properties)
            {
                int i = Array.FindIndex<ColumnInfo>(Columns, (x) => x.Enable && x.TargetMeta.Name == info.Name);
                if( i >= 0)
                {
                    objArray[index++] = reader.GetValue(i);
                }
            }
            return (T)Activator.CreateInstance(TargetType, objArray);
        }

        private T CreateClass(DbFieldReader reader)
        {
            //T instance = Activator.CreateInstance<T>();
            T instance = (T)TargetMeta.CreateInstance();

            for (int i = 0; i < FieldCount; i++)
            {
                ColumnInfo column = Columns[i];
                if (!column.Enable)
                {
                    continue;
                }
                if (reader.IsDBNull(column.SourceIndex))
                {
                    continue;
                }
                SetValue(reader, instance, column);
            }
            return instance;
        }
        
        private void SetValue(DbFieldReader reader, object instance, ColumnInfo column)
        {
            var ordinal = column.SourceIndex;
            MemberMeta member = column.TargetMeta;

            if (column.TargetIsEnum)
            {
                object v = reader.ReadEnum(column.TargetUnderlyingType, ordinal);
                column.TargetMeta.SetValue(instance, v);
                return;
            }

            switch (column.TargetUnderlyingTypeCode)
            {
                case TypeCode.Empty:
                    return;
                case TypeCode.DBNull:
                    return;
                //case TypeCode.Object:
                //    member.SetValue(instance, reader.ReadValue(ordinal)); return;
                case TypeCode.Boolean:
                    if (member.IsNullable)
                    {
                        member.SetValue<Boolean?>(instance, reader.ReadBoolean(ordinal)); return;
                    }
                    else
                    {
                        member.SetValue<Boolean>(instance, reader.ReadBoolean(ordinal)); return;
                    }
                case TypeCode.Char:
                    if (member.IsNullable)
                    {
                        member.SetValue<Char?>(instance, reader.ReadChar(ordinal)); return;
                    }
                    else
                    {
                        member.SetValue<Char>(instance, reader.ReadChar(ordinal)); return;
                    }
                case TypeCode.SByte:
                    if (member.IsNullable)
                    {
                        member.SetValue<SByte?>(instance, reader.ReadSByte(ordinal)); return;
                    }
                    else
                    {
                        member.SetValue<SByte>(instance, reader.ReadSByte(ordinal)); return;
                    }
                case TypeCode.Byte:
                    if (member.IsNullable)
                    {
                        member.SetValue<Byte?>(instance, reader.ReadByte(ordinal)); return;
                    }
                    else
                    {
                        member.SetValue<Byte>(instance, reader.ReadByte(ordinal)); return;
                    }
                case TypeCode.Int16:
                    if (member.IsNullable)
                    {
                        member.SetValue<Int16?>(instance, reader.ReadInt16(ordinal)); return;
                    }
                    else
                    {
                        member.SetValue<Int16>(instance, reader.ReadInt16(ordinal)); return;
                    }
                case TypeCode.UInt16:
                    if (member.IsNullable)
                    {
                        member.SetValue<UInt16?>(instance, reader.ReadUInt16(ordinal)); return;
                    }
                    else
                    {
                        member.SetValue<UInt16>(instance, reader.ReadUInt16(ordinal)); return;
                    }
                case TypeCode.Int32:
                    if (member.IsNullable)
                    {
                        member.SetValue<Int32?>(instance, reader.ReadInt32(ordinal)); return;
                    }
                    else
                    {
                        member.SetValue<Int32>(instance, reader.ReadInt32(ordinal)); return;
                    }
                case TypeCode.UInt32:
                    if (member.IsNullable)
                    {
                        member.SetValue<UInt32?>(instance, reader.ReadUInt32(ordinal)); return;
                    }
                    else
                    {
                        member.SetValue<UInt32>(instance, reader.ReadUInt32(ordinal)); return;
                    }
                case TypeCode.Int64:
                    if (member.IsNullable)
                    {
                        member.SetValue<Int64?>(instance, reader.ReadInt64(ordinal)); return;
                    }
                    else
                    {
                        member.SetValue<Int64>(instance, reader.ReadInt64(ordinal)); return;
                    }
                case TypeCode.UInt64:
                    if (member.IsNullable)
                    {
                        member.SetValue<UInt64?>(instance, reader.ReadUInt64(ordinal)); return;
                    }
                    else
                    {
                        member.SetValue<UInt64>(instance, reader.ReadUInt64(ordinal)); return;
                    }
                case TypeCode.Single:
                    if (member.IsNullable)
                    {
                        member.SetValue<Single?>(instance, reader.ReadSingle(ordinal)); return;
                    }
                    else
                    {
                        member.SetValue<Single>(instance, reader.ReadSingle(ordinal)); return;
                    }
                case TypeCode.Double:
                    if (member.IsNullable)
                    {
                        member.SetValue<Double?>(instance, reader.ReadDouble(ordinal)); return;
                    }
                    else
                    {
                        member.SetValue<Double>(instance, reader.ReadDouble(ordinal)); return;
                    }
                case TypeCode.Decimal:
                    if (member.IsNullable)
                    {
                        member.SetValue<Decimal?>(instance, reader.ReadDecimal(ordinal)); return;
                    }
                    else
                    {
                        member.SetValue<Decimal>(instance, reader.ReadDecimal(ordinal)); return;
                    }
                case TypeCode.DateTime:
                    if (member.IsNullable)
                    {
                        member.SetValue<DateTime?>(instance, reader.ReadDateTime(ordinal)); return;
                    }
                    else
                    {
                        member.SetValue<DateTime>(instance, reader.ReadDateTime(ordinal)); return;
                    }
                case TypeCode.String:
                    member.SetValue<String>(instance, reader.ReadString(ordinal)); return;
                default:
                    if (column.TargetIsGuid)
                    {
                        if (member.IsNullable)
                        {

                        }
                        else
                        {
                            member.SetValue<Guid>(instance, reader.ReadGuid(ordinal)); return;
                        }
                    }
                    member.SetValue(instance, DataConvert.ChangeTypeTo(reader.ReadValue(ordinal, column.TargetUnderlyingTypeCode), member.MemberType)); return;
            }
        }
    }
}
