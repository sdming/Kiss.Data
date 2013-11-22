using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kiss.Data.Expression;
using Kiss.Data;
using NUnit.Framework;

namespace Kiss.DataTest.Driver
{
    public abstract class TestInsertBase
    {
        protected abstract string DbName();

        protected virtual string TableName()
        {
            return "ttable";
        }

        protected void DoInsertColumn(string column, Type targetType, string filter)
        {            
            var types = TypeData.GetTypes(filter);
            foreach (var t in types)
            {
                var insert = new Insert(TableName());
                object v = TypeData.GetTypeValue(t, targetType);
                insert.Set(column, v);

                object key = null;
                string keyColumn = "pk";                
                insert.Returning(keyColumn);
                
                object actual = null;
                using (DbContent db = new DbContent(DbName()))
                {
                    key = db.ExecuteScalar(insert);
                    //var reader = db.ExecuteReader(insert);
                    //while (reader.Read())
                    //{
                    //    key = reader.GetValue(0);
                    //}

                    actual = db.Table("ttable").ReadCell(column, keyColumn, key);
                }
                object want = DataConvert.ChangeTypeTo(v, actual.GetType());

                if (!AUtils.EqualsTo(actual, want))
                {
                    Assert.Fail("column:{0}, target type:{1}, source type:{2}, want:{3}, actual:{4} ", column, targetType, t, want, actual);
                }
                //Console.WriteLine("insert column:{0}, want:{1}, actual:{2}", column, want, actual);
                
            }
        }

        protected virtual void TestInsert()
        {
            DoInsertColumn("cbool", typeof(bool), "bool,string");
            DoInsertColumn("cint", typeof(int), "bool,numeric,string");
            DoInsertColumn("cfloat", typeof(float), "numeric,string");
            DoInsertColumn("cnumeric", typeof(decimal), "numeric,string");
            DoInsertColumn("cstring", typeof(string), "bool,numeric,string,date,guid");
            DoInsertColumn("cdatetime", typeof(DateTime), "string,date");
            DoInsertColumn("cguid", typeof(Guid), "string,guid");

        }

    }

    [TestFixture]
    public class TestInsertMsSql : TestInsertBase
    {
        protected override string DbName()
        {
            return "mssql";
        }

        [Test]
        public void Test()
        {
            base.TestInsert();
        }
    }

    [TestFixture]
    public class TestInsertSQLite : TestInsertBase
    {
        protected override string DbName()
        {
            return "sqlite";
        }

        [Test]
        public void Test()
        {
            base.TestInsert();
        }
    }

    [TestFixture]
    public class TestInsertMySql : TestInsertBase
    {
        protected override string DbName()
        {
            return "mysql";
        }

        [Test]
        public void Test()
        {
            base.TestInsert();
        }
    }

    [TestFixture]
    public class TestInsertPostgres : TestInsertBase
    {
        protected override string DbName()
        {
            return "postgres";
        }

        [Test]
        public void Test()
        {
            base.TestInsert();
        }
    }
    [TestFixture]
    public class TestInsertOracle : TestInsertBase
    {
        protected override string DbName()
        {
            return "oracle";
        }

        [Test]
        public void Test()
        {
            base.TestInsert();
        }
    }

}
