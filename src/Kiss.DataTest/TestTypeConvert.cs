using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using System.Diagnostics;
using Kiss.Core.Reflection;
using Kiss.Data;
using NUnit.Framework;

namespace Kiss.DataTest
{
    [TestFixture]
    public class TestTypeConvert
    {
        [Test]
        public void TestStructNullConvert()
        {
            Type sourceType = typeof(DataTypeStruct);
            var fields = sourceType.GetFields();

            foreach (var f in fields)
            {
                Type targetType = f.FieldType;
                if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    targetType = targetType.GetGenericArguments()[0];
                }

                if (targetType.IsEnum)
                {
                    continue;
                }

                var filter = TypeData.TypeDataFilter[targetType];
                var types = TypeData.GetTypes(filter);

                foreach (var t in types)
                {
                    var table = TypeData.BuildDataTableDbNull(f.Name, t);
                    var reader = table.CreateDataReader();
                    var list = DataConvert.ToList<DataTypeStruct>(reader, null);

                    var actual = f.GetValue(list[0]);
                    object want = null;
                    if (!TypeSystem.IsNullAssignable(f.FieldType))
                    {
                        want = Activator.CreateInstance(f.FieldType);
                    }
                    if (!Convert.ToString(want).Equals(Convert.ToString(actual), StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("Error:{0} != {1}", want, actual);
                        Assert.Fail("source type:{0}, target type:{1}; want:{2}, actual:{3}", t, targetType, want, actual);
                    }
                }
            }

        }

        public void TestStructConvert()
        {
            Type sourceType = typeof(DataTypeStruct);
            var fields = sourceType.GetFields();

            foreach (var f in fields)
            {
                Type targetType = f.FieldType;
                if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    targetType = targetType.GetGenericArguments()[0];
                }

                if (targetType.IsEnum)
                {
                    continue;
                }

                var filter = TypeData.TypeDataFilter[targetType];
                var types = TypeData.GetTypes(filter);

                foreach (var t in types)
                {
                    var table = TypeData.BuildDataTable(f.Name, t, targetType);
                    var reader = table.CreateDataReader();
                    var list = DataConvert.ToList<DataTypeStruct>(reader, null);

                    var actual = f.GetValue(list[0]);
                    var want = table.Rows[0][0];
                    if (!AssertEquals(want, actual, f.FieldType))
                    {
                        Assert.Fail("source type:{0}, target type:{1}; want:{2}, actual:{3}", t, targetType, want, actual);
                    }
                }
            }

        }

        public void TestClassEnumConvert()
        {
            DataTable table = new DataTable();
            table.Columns.Add("P_Enum", typeof(int));
            table.Rows.Add(1);
            table.Rows.Add();
            var reader = table.CreateDataReader();
            var list = DataConvert.ToList<DataTypeClass>(reader);

            if (list[0].P_Enum != DemoEnum.A)
            {
                Console.WriteLine("Error:{0} != {1}", DemoEnum.A, list[0].P_Enum);
                Assert.Fail("enum test fail, want:{0}, actual:{1}", DemoEnum.A, list[0].P_Enum);
            }

            if (list[1].P_Enum != DemoEnum.None)
            {
                Console.WriteLine("Error:{0} != {1}", DemoEnum.None, list[1].P_Enum);
                Assert.Fail("enum test fail, want:{0}, actual:{1}", DemoEnum.None, list[1].P_Enum);
            }

            table = new DataTable();
            table.Columns.Add("P_Enum", typeof(string));
            table.Rows.Add("A");
            table.Rows.Add();
            reader = table.CreateDataReader();
            list = DataConvert.ToList<DataTypeClass>(reader);

            if (list[0].P_Enum != DemoEnum.A)
            {
                Console.WriteLine("Error:{0} != {1}", DemoEnum.A, list[0].P_Enum);
                Assert.Fail("enum test fail, want:{0}, actual:{1}", DemoEnum.A, list[0].P_Enum);
            }

            if (list[1].P_Enum != DemoEnum.None)
            {
                Console.WriteLine("Error:{0} != {1}", DemoEnum.None, list[1].P_Enum);
                Assert.Fail("enum test fail, want:{0}, actual:{1}", DemoEnum.None, list[1].P_Enum);
            }
        }

        public void TestClassNullableEnumConvert()
        {
            DataTable table = new DataTable();
            table.Columns.Add("NP_Enum", typeof(int));
            table.Rows.Add(1);
            table.Rows.Add();
            var reader = table.CreateDataReader();
            var list = DataConvert.ToList<DataTypeClass>(reader);

            if (list[0].NP_Enum != DemoEnum.A)
            {
                Console.WriteLine("Error:{0} != {1}", DemoEnum.A, list[0].NP_Enum);
                Assert.Fail("enum test fail, want:{0}, actual:{1}", DemoEnum.A, list[0].NP_Enum);
            }

            if (list[1].NP_Enum != null)
            {
                Console.WriteLine("Error:{0} != {1}", null, list[1].NP_Enum);
                Assert.Fail("enum test fail, want:{0}, actual:{1}", null, list[1].NP_Enum);
            }

            table = new DataTable();
            table.Columns.Add("NP_Enum", typeof(string));
            table.Rows.Add("A");
            table.Rows.Add();
            reader = table.CreateDataReader();
            list = DataConvert.ToList<DataTypeClass>(reader);

            if (list[0].NP_Enum != DemoEnum.A)
            {
                Console.WriteLine("Error:{0} != {1}", DemoEnum.A, list[0].NP_Enum);
                Assert.Fail("enum test fail, want:{0}, actual:{1}", DemoEnum.A, list[0].NP_Enum);
            }

            if (list[1].NP_Enum != null)
            {
                Console.WriteLine("Error:{0} != {1}", null, list[1].NP_Enum);
                Assert.Fail("enum test fail, want:{0}, actual:{1}", null, list[1].NP_Enum);
            }
        }

        public void TestEnumConvert()
        {
            DataTable table = new DataTable();
            table.Columns.Add("int", typeof(int));
            table.Rows.Add(1);
            table.Rows.Add();
            var reader = table.CreateDataReader();
            var list = DataConvert.ToList<DemoEnum>(reader);

            if (list[0] != DemoEnum.A)
            {
                Console.WriteLine("Error:{0} != {1}", DemoEnum.A, list[0]);
                Assert.Fail("enum test fail, want:{0}, actual:{1}", DemoEnum.A,  list[0]);
            }

            if (list[1] != DemoEnum.None)
            {
                Console.WriteLine("Error:{0} != {1}", DemoEnum.None, list[1]);
                Assert.Fail("enum test fail, want:{0}, actual:{1}", DemoEnum.None,  list[1]);
            }

            table = new DataTable();
            table.Columns.Add("string", typeof(string));
            table.Rows.Add("A");
            table.Rows.Add();
            reader = table.CreateDataReader();
            list = DataConvert.ToList<DemoEnum>(reader);

            if (list[0] != DemoEnum.A)
            {
                Console.WriteLine("Error:{0} != {1}", DemoEnum.A, list[0]);
                Assert.Fail("enum test fail, want:{0}, actual:{1}", DemoEnum.A, list[0]);
            }

            if (list[1] != DemoEnum.None)
            {
                Console.WriteLine("Error:{0} != {1}", DemoEnum.None, list[1]);
                Assert.Fail("enum test fail, want:{0}, actual:{1}", DemoEnum.None, list[1]);
            }
        }

        public void TestNullableEnumConvert()
        {
            DataTable table = new DataTable();
            table.Columns.Add("int", typeof(int));
            table.Rows.Add(1);
            table.Rows.Add();
            var reader = table.CreateDataReader();
            var list = DataConvert.ToList<DemoEnum?>(reader);

            if (list[0] != DemoEnum.A)
            {
                Console.WriteLine("Error:{0} != {1}", DemoEnum.A, list[0]);
                Assert.Fail("enum test fail, want:{0}, actual:{1}", DemoEnum.A, list[0]);
            }

            if (list[1] != null)
            {
                Console.WriteLine("Error:{0} != {1}", null, list[1]);
                Assert.Fail("enum test fail, want:{0}, actual:{1}", null, list[1]);
            }

            table = new DataTable();
            table.Columns.Add("string", typeof(string));
            table.Rows.Add("A");
            table.Rows.Add();
            reader = table.CreateDataReader();
            list = DataConvert.ToList<DemoEnum?>(reader);

            if (list[0] != DemoEnum.A)
            {
                Console.WriteLine("Error:{0} != {1}", DemoEnum.A, list[0]);
                Assert.Fail("enum test fail, want:{0}, actual:{1}", DemoEnum.A, list[0]);
            }

            if (list[1] != null)
            {
                Console.WriteLine("Error:{0} != {1}", null, list[1]);
                Assert.Fail("enum test fail, want:{0}, actual:{1}", null, list[1]);
            }
        }

        public void TestClassNullConvert()
        {
            Type sourceType = typeof(DataTypeClass);
            var properties = sourceType.GetProperties();

            foreach (var p in properties)
            {
                Type targetType = p.PropertyType;
                if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    targetType = targetType.GetGenericArguments()[0];
                }

                if (targetType.IsEnum)
                {
                    continue;
                }

                var filter = TypeData.TypeDataFilter[targetType];
                var types = TypeData.GetTypes(filter);

                foreach (var t in types)
                {
                    var table = TypeData.BuildDataTableDbNull(p.Name, t);
                    var reader = table.CreateDataReader();
                    var list = DataConvert.ToList<DataTypeClass>(reader, null);

                    var actual = p.GetValue(list[0], null);
                    object want = null;
                    if (!TypeSystem.IsNullAssignable(p.PropertyType))
                    {
                        want = Activator.CreateInstance(p.PropertyType);
                    }
                    if (!Convert.ToString(want).Equals(Convert.ToString(actual), StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("Error:{0} != {1}", want, actual);
                        Assert.Fail("source type:{0}, target type:{1}; want:{2}, actual:{3}", t, targetType, want, actual);
                    }
                }
            }

        }

        public void TestClassConvert()
        {
            Type sourceType = typeof(DataTypeClass);
            var properties = sourceType.GetProperties();

            foreach (var p in properties)
            {
                Type targetType = p.PropertyType;
                if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    targetType = targetType.GetGenericArguments()[0];
                }

                if (targetType.IsEnum)
                {
                    continue;
                }

                var filter = TypeData.TypeDataFilter[targetType];
                var types = TypeData.GetTypes(filter);

                foreach (var t in types)
                {                    
                    var table = TypeData.BuildDataTable(p.Name, t, targetType);
                    var reader = table.CreateDataReader();
                    var list = DataConvert.ToList<DataTypeClass>(reader, null);

                    var actual = p.GetValue(list[0], null);
                    var want = table.Rows[0][0];
                    if (!AssertEquals(want, actual, p.PropertyType))
                    {
                        Assert.Fail("source type:{0}, target type:{1}; want:{2}, actual:{3}", t, targetType, want, actual);
                    }
                }
            }

        }

        public void TestNullableSystemTypeConvert()
        {
            SystemTypeConvert<Boolean?>("bool,string");
            SystemTypeConvert<Char?>("string");
            SystemTypeConvert<SByte?>("bool,numeric,string");
            SystemTypeConvert<Byte?>("bool,numeric,string");
            SystemTypeConvert<Int16?>("bool,numeric,string");
            SystemTypeConvert<UInt16?>("bool,numeric,string");
            SystemTypeConvert<Int32?>("bool,numeric,string");
            SystemTypeConvert<UInt32?>("bool,numeric,string");
            SystemTypeConvert<Int64?>("bool,numeric,string");
            SystemTypeConvert<UInt64?>("bool,numeric,string");
            SystemTypeConvert<Single?>("bool,numeric,string");
            SystemTypeConvert<Double?>("bool,numeric,string");
            SystemTypeConvert<Decimal?>("bool,numeric,string");
            SystemTypeConvert<DateTime?>("string,date");
            SystemTypeConvert<Guid?>("string,guid");
        }

        public void TestNullableSystemTypeNullConvert()
        {
            SystemTypeNullConvert<Boolean?>("bool,string");
            SystemTypeNullConvert<Char?>("string");
            SystemTypeNullConvert<SByte?>("bool,numeric,string");
            SystemTypeNullConvert<Byte?>("bool,numeric,string");
            SystemTypeNullConvert<Int16?>("bool,numeric,string");
            SystemTypeNullConvert<UInt16?>("bool,numeric,string");
            SystemTypeNullConvert<Int32?>("bool,numeric,string");
            SystemTypeNullConvert<UInt32?>("bool,numeric,string");
            SystemTypeNullConvert<Int64?>("bool,numeric,string");
            SystemTypeNullConvert<UInt64?>("bool,numeric,string");
            SystemTypeNullConvert<Single?>("bool,numeric,string");
            SystemTypeNullConvert<Double?>("bool,numeric,string");
            SystemTypeNullConvert<Decimal?>("bool,numeric,string");
            SystemTypeNullConvert<DateTime?>("string,date");
            SystemTypeNullConvert<Guid?>("string,guid");
        }

        public void TestSystemTypeConvert()
        {
            SystemTypeConvert<Boolean>("bool,string");
            SystemTypeConvert<Char>("string");
            SystemTypeConvert<SByte>("bool,numeric,string");
            SystemTypeConvert<Byte>("bool,numeric,string");
            SystemTypeConvert<Int16>("bool,numeric,string");
            SystemTypeConvert<UInt16>("bool,numeric,string");
            SystemTypeConvert<Int32>("bool,numeric,string");
            SystemTypeConvert<UInt32>("bool,numeric,string");
            SystemTypeConvert<Int64>("bool,numeric,string");
            SystemTypeConvert<UInt64>("bool,numeric,string");
            SystemTypeConvert<Single>("bool,numeric,string");
            SystemTypeConvert<Double>("bool,numeric,string");
            SystemTypeConvert<Decimal>("bool,numeric,string");
            SystemTypeConvert<DateTime>("string,date");
            SystemTypeConvert<String>("bool,numeric,string,date,guid");
            SystemTypeConvert<Guid>("string,guid");
        }

        public void TestSystemTypeNullConvert()
        {
            SystemTypeNullConvert<Boolean>("bool,string");
            SystemTypeNullConvert<Char>("string");
            SystemTypeNullConvert<SByte>("bool,numeric,string");
            SystemTypeNullConvert<Byte>("bool,numeric,string");
            SystemTypeNullConvert<Int16>("bool,numeric,string");
            SystemTypeNullConvert<UInt16>("bool,numeric,string");
            SystemTypeNullConvert<Int32>("bool,numeric,string");
            SystemTypeNullConvert<UInt32>("bool,numeric,string");
            SystemTypeNullConvert<Int64>("bool,numeric,string");
            SystemTypeNullConvert<UInt64>("bool,numeric,string");
            SystemTypeNullConvert<Single>("bool,numeric,string");
            SystemTypeNullConvert<Double>("bool,numeric,string");
            SystemTypeNullConvert<Decimal>("bool,numeric,string");
            SystemTypeNullConvert<DateTime>("string,date");
            SystemTypeNullConvert<String>("bool,numeric,string,date,guid");
            SystemTypeNullConvert<Guid>("string,guid");
        }

        public void SystemTypeNullConvert<T>(string filter)
        {
            Type targetType = typeof(T);
            var types = TypeData.GetTypes(filter);

            foreach (var t in types)
            {
                var table = TypeData.BuildDataTableDbNull(t);
                var reader = table.CreateDataReader();
                var list = DataConvert.ToList<T>(reader);

                for (var i = 0; i < table.Rows.Count; i++)
                {
                    object want = default(T);
                    object actual = list[i];
                    if (!Convert.ToString(want).Equals(Convert.ToString(actual), StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("Error:{0} != {1}", want, actual);
                        Assert.Fail("source type:{0}, target type:{1}; want:{2}, actual:{3}", t, targetType, want, actual);
                    }
                }
            }
        }

        public void SystemTypeConvert<T>(string filter)
        {
            Type targetType = typeof(T);
            if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                targetType = targetType.GetGenericArguments()[0];
            }
            var types = TypeData.GetTypes(filter);

            foreach (var t in types)
            {
                var table = TypeData.BuildDataTable(t, targetType);
                var reader = table.CreateDataReader();
                var list = DataConvert.ToList<T>(reader);

                for (var i = 0; i < table.Rows.Count; i++)
                {
                    object want = table.Rows[i][0];
                    object actual = list[i];
                    if (!AssertEquals(want, actual, targetType))
                    {
                        Assert.Fail("source type:{0}, target type:{1}; want:{2}, actual:{3}", t, targetType, want, actual);
                    }
                }
            }

        }

        private bool AssertEquals(object want, object actual, Type targetType)
        {
            if (want is DateTime || actual is DateTime)
            {
                if (Convert.ToDateTime(want).ToString("yyyy-MM-dd HH:mm:ss") == Convert.ToDateTime(actual).ToString("yyyy-MM-dd HH:mm:ss"))
                {
                    return true;
                }
            }

            if (want == actual)
            {
                return true;
            }
            
            IConvertible c = want as IConvertible;
            if (c != null)
            {
                want = System.Data.Linq.DBConvert.ChangeType(want, targetType);
            }
            if (Convert.ToString(want).Equals(Convert.ToString(actual), StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            return want == actual;
        }

        public void BenchSystemCoreConvertInt32(TextWriter writer, int count)
        {
            DataTable table = new DataTable();
            table.Columns.Add(typeof(int).Name, typeof(int));

            for (var i = 0; i < count; i++)
            {
                table.Rows.Add(i);
            }
            var reader = table.CreateDataReader();

            Console.WriteLine("bench: {0}", "convert int");
            writer.WriteLine("start: {0}", DateTime.Now.ToString("hh:mm:ss:fff"));
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var list = DataConvert.ToList<int>(reader);
            sw.Stop();
            writer.WriteLine("end: count:{0}, ms:{1}, {2}",
                    count, sw.ElapsedMilliseconds, DateTime.Now.ToString("hh:mm:ss:fff"));
        }

        public void BenchSystemCoreConvertString(TextWriter writer, int count)
        {
            DataTable table = new DataTable();
            table.Columns.Add(typeof(string).Name, typeof(string));

            for (var i = 0; i < count; i++)
            {
                table.Rows.Add(i.ToString());
            }
            var reader = table.CreateDataReader();

            Console.WriteLine("bench: {0}", "convert string");
            writer.WriteLine("start: {0}", DateTime.Now.ToString("hh:mm:ss:fff"));
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var list = DataConvert.ToList<string>(reader);
            sw.Stop();
            writer.WriteLine("end: count:{0}, ms:{1}, {2}",
                    count, sw.ElapsedMilliseconds, DateTime.Now.ToString("hh:mm:ss:fff"));
        }

        public void BenchSystemBaseClassConvert(TextWriter writer, int count)
        {
            DataTable table = new DataTable();
            table.Columns.Add("CBool", typeof(Boolean));
            table.Columns.Add("CInt", typeof(Int32));
            table.Columns.Add("CNumeric", typeof(Double));
            table.Columns.Add("CString", typeof(String));
            table.Columns.Add("CDatetime", typeof(DateTime));

            for (var i = 0; i < count; i++)
            {
                table.Rows.Add(true, i, i * 1.1, i.ToString(), DateTime.Now);
            }
            var reader = table.CreateDataReader();

            Console.WriteLine("bench: {0}", "convert class");
            writer.WriteLine("start: {0}", DateTime.Now.ToString("hh:mm:ss:fff"));
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var list = DataConvert.ToList<DataBaseTypeClass>(reader);
            sw.Stop();
            writer.WriteLine("end: count:{0}, ms:{1}, {2}",
                    count, sw.ElapsedMilliseconds, DateTime.Now.ToString("hh:mm:ss:fff"));
        }
    }
}
