using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Kiss.Data;

namespace Kiss.DataTest.Driver
{    
    public abstract class TestTextBase
    {
        protected abstract string DbName();

        protected virtual object GetCell(string column, object pk)
        {
            using (DbContent db = new DbContent(DbName()))
            {
                return db.Table("tTablE").ReadCell(column, "pk", pk);
            }
        }

        protected virtual string UpdateSql()
        {
            return @"
update TTABLE set cint = @cint where pk = @pk
";
        }

        protected virtual string ReadSql()
        {
            return @"
select * from TTABLE where pk = @pk
";
        }

        protected virtual string ScalarSql()
        {
            return @"
select cint from TTABLE where pk = @pk
";
        }

        protected virtual Kiss.Core.IDataObjectAdapter CreateAdapter()
        {
            return Kiss.Core.Adapter.Dictionary();
        }

        public void TestAdapter()
        {
            var list = DataUtils.BuildTestData(DbName(), 10);
            int index = 0;

            using (DbContent db = new DbContent(DbName()))
            {
                var ae = list[index];
                ae.CInt = DataUtils.NextInt32();
                var data = CreateAdapter();
                data.Set("CinT", ae.CInt);
                data.Set("pK", ae.PK);
                db.TextNonQuery(UpdateSql(), data);
                object actual = GetCell("cint", ae.PK);
                object want = ae.CInt;
                Assert.AreEqual(want, actual);
            }


            using (DbContent db = new DbContent(DbName()))
            {
                index++;
                var ae = list[index];
                var data = CreateAdapter();
                data.Set("pK", ae.PK);
                var reader = db.TextReader(ReadSql(), data);
                reader.Read();
                Assert.AreEqual(ae.CInt, reader["cint"]);
                reader.Dispose();
            }

            using (DbContent db = new DbContent(DbName()))
            {
                index++;
                var ae = list[index];
                var data = CreateAdapter();
                data.Set("pK", ae.PK);
                var i = db.TextScalar(ScalarSql(), data);
                Assert.AreEqual(ae.CInt, i);
            }


        }

        public void TestParams()
        {
            var list = DataUtils.BuildTestData(DbName(), 10);
            int index = 0;

            using (DbContent db = new DbContent(DbName()))
            {
                var ae = list[index];
                ae.CInt = DataUtils.NextInt32();                
                db.TextNonQuery(UpdateSql(), ae.CInt, ae.PK);
                object actual = GetCell("cint", ae.PK);
                object want = ae.CInt;
                Assert.AreEqual(want, actual);
            }


            using (DbContent db = new DbContent(DbName()))
            {
                index++;
                var ae = list[index];
                var reader = db.TextReader(ReadSql(), ae.PK);
                reader.Read();
                Assert.AreEqual(ae.CInt, reader["cint"]);
                reader.Dispose();
            }

            using (DbContent db = new DbContent(DbName()))
            {
                index++;
                var ae = list[index];
                var i = db.TextScalar(ScalarSql(), ae.PK);
                Assert.AreEqual(ae.CInt, i);
            }


        }
    }

    [TestFixture]
    public class TestTextMsSql : TestTextBase
    {
        protected override string DbName()
        {
            return "mssql";
        }
    }

    [TestFixture]
    public class TestTextSqLite : TestTextBase
    {
        protected override string DbName()
        {
            return "sqlite";
        }
    }

    [TestFixture]
    public class TestTextOracle : TestTextBase
    {
        protected override string UpdateSql()
        {
            return @"
update TTABLE set cint = :cint where pk = :pk
";
        }

        protected override string ReadSql()
        {
            return @"
select * from TTABLE where pk = :pk
";
        }

        protected override string ScalarSql()
        {
            return @"
select cint from TTABLE where pk = :pk
";
        }

        protected override string DbName()
        {
            return "oracle";
        }

    }

    [TestFixture]
    public class TestTextPostgres : TestTextBase
    {
        protected override string DbName()
        {
            return "postgres";
        }

    }

    [TestFixture]
    public class TestTextMySql : TestTextBase
    {
        protected override string DbName()
        {
            return "mysql";
        }

        protected override string UpdateSql()
        {
            return @"
update TTABLE set cint = ?cint where pk = ?pk
";
        }

        protected override string ReadSql()
        {
            return @"
select * from TTABLE where pk = ?pk
";
        }

        protected override string ScalarSql()
        {
            return @"
select cint from TTABLE where pk = ?pk
";
        }
    }
}
