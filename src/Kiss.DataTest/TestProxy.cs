using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Kiss.Data;
using NUnit.Framework;

namespace Kiss.DataTest
{
    public interface IPorxyTest
    {
        IDataReader usp_query(int cint);

        [DbProcedure(Name = "usp_exec")]
        IDataReader Exec([DbParameter(Name = "cint")] int c);

        IDataReader usp_inout(int x, ref int y, out int sum);
    }

    [TestFixture]
    public class TestProxy
    {
        protected virtual string DbName()
        {
            return "mssql";
        }

        [Test]
        public void TestQuery()
        {
            using(DbContent db = new DbContent(DbName()))
            {
                IPorxyTest proxy = FunctionProxy<IPorxyTest>.Create(db);
                var reader = proxy.usp_query(101);
                Assert.IsTrue(reader.FieldCount > 0);
            }            
        }

        [Test]
        public void TestExec()
        {
            using (DbContent db = new DbContent(DbName()))
            {
                IPorxyTest proxy = FunctionProxy<IPorxyTest>.Create(db);
                var reader = proxy.Exec(101);
                Assert.IsTrue(reader.RecordsAffected >= -1);
            }
        }

        [Test]
        public void TestInOut()
        {
            int x = 3;
            int y = 7;
            int sum;

            using (DbContent db = new DbContent(DbName()))
            {
                IPorxyTest proxy = FunctionProxy<IPorxyTest>.Create(db);
                var reader = proxy.usp_inout(x, ref y, out sum);
            }
            
            Assert.IsTrue(y == 14);
            Assert.IsTrue(sum == 10);
        }
    }
}
