using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Kiss.Data;

namespace Kiss.DataTest
{

    public enum DemoEnum
    {
        None = 0,
        A = 1,
        B = 2,
        C = 3,
    }

    class TypeData
    {
        internal static Dictionary<Type, object> TypeDataMap = new Dictionary<Type, object>();
        internal static Dictionary<Type, string> TypeDataFilter = new Dictionary<Type, string>();


        static TypeData()
        {
            TypeDataMap[typeof(object)] = new object();
            TypeDataMap[typeof(DBNull)] = DBNull.Value;
            TypeDataMap[typeof(Boolean)] = true;
            TypeDataMap[typeof(Char)] = Char.Parse("C");
            TypeDataMap[typeof(SByte)] = (SByte)TypeCode.SByte;
            TypeDataMap[typeof(Byte)] = (Byte)TypeCode.Byte;
            TypeDataMap[typeof(Int16)] = (Int16)TypeCode.Int16;
            TypeDataMap[typeof(UInt16)] = (UInt16)TypeCode.UInt16;
            TypeDataMap[typeof(Int32)] = (Int32)TypeCode.Byte;
            TypeDataMap[typeof(UInt32)] = (UInt32)TypeCode.UInt32;
            TypeDataMap[typeof(Int64)] = (Int64)TypeCode.Int64;
            TypeDataMap[typeof(UInt64)] = (UInt64)TypeCode.UInt64;
            TypeDataMap[typeof(Single)] = (Single)TypeCode.Single * 1.01;
            TypeDataMap[typeof(Double)] = (Double)TypeCode.Double * 1.01;
            TypeDataMap[typeof(Decimal)] = (Decimal)TypeCode.Decimal * (Decimal)1.01;
            TypeDataMap[typeof(DateTime)] = DateTime.Parse("2004-07-24");
            TypeDataMap[typeof(String)] = "string";
            TypeDataMap[typeof(Guid)] = Guid.Parse("550e8400-e29b-41d4-a716-446655440000");

            TypeDataFilter[typeof(Boolean)] = "bool,string";
            TypeDataFilter[typeof(Char)] = "string";
            TypeDataFilter[typeof(SByte)] = "bool,numeric,string";
            TypeDataFilter[typeof(Byte)] = "bool,numeric,string";
            TypeDataFilter[typeof(Int16)] = "bool,numeric,string";
            TypeDataFilter[typeof(UInt16)] = "bool,numeric,string";
            TypeDataFilter[typeof(Int32)] = "bool,numeric,string";
            TypeDataFilter[typeof(UInt32)] = "bool,numeric,string";
            TypeDataFilter[typeof(Int64)] = "bool,numeric,string";
            TypeDataFilter[typeof(UInt64)] = "bool,numeric,string";
            TypeDataFilter[typeof(Single)] = "bool,numeric,string";
            TypeDataFilter[typeof(Double)] = "bool,numeric,string";
            TypeDataFilter[typeof(Decimal)] = "bool,numeric,string";
            TypeDataFilter[typeof(DateTime)] = "string,date";
            TypeDataFilter[typeof(String)] ="bool,numeric,string,date,guid";
            TypeDataFilter[typeof(Guid)] = "string,guid";
        }

        public static List<Type> GetTypes(string filter)
        {
            return GetTypes(
                filter.Contains("bool"),
                filter.Contains("char"),
                filter.Contains("numeric"),
                filter.Contains("date"),
                filter.Contains("string"),
                filter.Contains("guid")
                );
        }

        public static List<Type> GetTypes(bool hasBoolean, bool hasChar, bool hasNumeric, bool hasDateTime, bool hasString, bool hasGuid)
        {
            List<Type> types = new List<Type>();

            if (hasBoolean)
            {
                types.Add(typeof(Boolean));
            }

            if (hasChar)
            {
                types.Add(typeof(Char));
            }

            if (hasNumeric)
            {
                types.Add(typeof(SByte));
                types.Add(typeof(Byte));
                types.Add(typeof(Int16));
                types.Add(typeof(UInt16));
                types.Add(typeof(Int32));
                types.Add(typeof(UInt32));
                types.Add(typeof(Int64));
                types.Add(typeof(UInt64));
                types.Add(typeof(Single));
                types.Add(typeof(Double));
                types.Add(typeof(Decimal));
            }

            if (hasDateTime)
            {
                types.Add(typeof(DateTime));
            }

            if (hasString)
            {
                types.Add(typeof(String));
            }

            if (hasGuid)
            {
                types.Add(typeof(Guid));
            }

            if (hasGuid)
            {
                types.Add(typeof(Guid));
            }

            return types;
        }

        public static DataTable BuildDataTable(Type dataType)
        {
            DataTable t = new DataTable();
            t.Columns.Add(dataType.Name, dataType);
            t.Rows.Add(TypeDataMap[dataType]);
            return t;
        }

        public static DataTable BuildDataTableDbNull(Type dataType)
        {
            return BuildDataTableDbNull(dataType.Name, dataType);
        }

        public static DataTable BuildDataTableDbNull(string collumnName, Type dataType)
        {
            DataTable t = new DataTable();
            t.Columns.Add(collumnName, dataType);
            t.Rows.Add();
            return t;
        }

        public static DataTable BuildDataTable(string collumnName, Type dataType, Type convertType)
        {
            DataTable t = new DataTable();
            t.Columns.Add(collumnName, dataType);

            if (dataType == typeof(string))
            {
                t.Rows.Add(TypeDataMap[convertType].ToString());
            }
            else
            {
                t.Rows.Add(TypeDataMap[dataType]);
            }
            return t;
        }

        public static DataTable BuildDataTable(Type dataType, Type convertType)
        {
            return BuildDataTable(dataType.Name, dataType, convertType);
        }

        public static object GetTypeValue(Type dataType, Type convertType)
        {
            object data = null;
            if (dataType == typeof(string))
            {
                data = TypeDataMap[convertType].ToString();
            }
            else
            {
                data = TypeDataMap[dataType];
            }
            return DataConvert.ChangeTypeTo(data, convertType);
        }

    }
}
