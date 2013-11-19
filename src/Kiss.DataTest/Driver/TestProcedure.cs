using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using Kiss.Data;
using System.Data;
using Kiss.Data.Schema;
using Kiss.Data.Expression;
using Kiss.Core;
using NUnit.Framework;

namespace Kiss.DataTest.Driver
{
    public abstract class TestProcedureBase
    {
        protected abstract string DbName();

        protected IDataObjectAdapter CreateIAdapter()
        {
            return Kiss.Core.Adapter.Dictionary();
        }

        public DataTable Query(string procedureName, IDataObjectAdapter parameters)
        {
            using (DbContent db = new DbContent(DbName()))
            {
                var reader = db.ProcedureReader(procedureName, parameters);
                DataTable t = new DataTable(); 
                t.Load(reader);   
                return t;
            }
        }

        public int ExecuteNonQuery(string procedureName, IDataObjectAdapter parameters)
        {
            using (DbContent db = new DbContent(DbName()))
            {
                return db.ProcedureNonQuery(procedureName, parameters);
            }
        }

        public IExecuteResult ExecuteResult(string procedureName, IDataObjectAdapter parameters)
        {
            using (DbContent db = new DbContent(DbName()))
            {
                IExecuteResult r;
                db.ProcedureNonQuery(procedureName, parameters, out r);
                return r;
            }
        }
    }

    [TestFixture]
    public class TestProcedureMsSql : TestProcedureBase
    {
        protected override string DbName()
        {
            return "mssql";
        }

        [Test]
        public void TestQuery()
        {
            var parameters = CreateIAdapter();
            parameters.Set("cint", 101);
            var table = Query("usp_query", parameters);
            Assert.NotNull(table);
        }

        [Test]
        public void TestExecute()
        {
            var parameters = CreateIAdapter();
            parameters.Set("cint", 101);
            var i = ExecuteNonQuery("usp_exec", parameters);
            Assert.True(i > -1);
        }

        [Test]
        public void TestResult()
        {
            var parameters = CreateIAdapter();
            parameters.Set("x", 3);
            parameters.Set("y", 7);
            var i = ExecuteResult("usp_inout", parameters);
            i.Output()["y"] = 14;
            i.Output()["sum"] = 10;
        }
    }

    [TestFixture]
    public class TestProcedureMySql : TestProcedureBase
    {
        protected override string DbName()
        {
            return "mysql";
        }

        [Test]
        public void TestQuery()
        {
            var parameters = CreateIAdapter();
            parameters.Set("p_cint", 101);
            var table = Query("usp_query", parameters);
            Assert.NotNull(table);
        }

        [Test]
        public void TestExecute()
        {
            var parameters = CreateIAdapter();
            parameters.Set("p_cint", 101);
            var i = ExecuteNonQuery("usp_exec", parameters);
            Assert.True(i > -1);
        }

        [Test]
        public void TestResult()
        {
            var parameters = CreateIAdapter();
            parameters.Set("x", 3);
            parameters.Set("y", 7);
            var i = ExecuteResult("usp_inout", parameters);
            i.Output()["y"] = 14;
            i.Output()["sum"] = 10;
        }
    }

    [TestFixture]
    public class TestProcedurePostgres : TestProcedureBase
    {
        protected override string DbName()
        {
            return "postgres";
        }

        [Test]
        public void TestQuery()
        {
            var parameters = CreateIAdapter();
            parameters.Set("cint", 101);
            var table = Query("fn_query", parameters);
            Assert.NotNull(table);
        }

        [Test]
        public void TestExecute()
        {
            var parameters = CreateIAdapter();
            parameters.Set("cint", 101);
            var i = ExecuteNonQuery("fn_exec", parameters);
            Assert.True(i >= -1);
        }

        [Test]
        public void TestResult()
        {
            var parameters = CreateIAdapter();
            parameters.Set("x", 3);
            parameters.Set("y", 7);
            var i = ExecuteResult("fn_inout", parameters);
            i.Output()["y"] = 14;
            i.Output()["sum"] = 10;
        }
    }

    [TestFixture]
    public class TestProcedureOracle : TestProcedureBase
    {
        protected override string DbName()
        {
            return "oracle";
        }

        [Test]
        public void TestQuery()
        {
            var parameters = CreateIAdapter();
            parameters.Set("v_cint", 101);
            var table = Query("sp_query", parameters);
            Assert.NotNull(table);
        }

        [Test]
        public void TestExecute()
        {
            var parameters = CreateIAdapter();
            parameters.Set("v_cint", 101);
            var i = ExecuteNonQuery("sp_exec", parameters);
            Assert.True(i >= -1);
        }

        [Test]
        public void TestResult()
        {
            var parameters = CreateIAdapter();
            parameters.Set("x", 3);
            parameters.Set("y", 7);
            var i = ExecuteResult("sp_inout", parameters);
            i.Output()["y"] = 14;
            i.Output()["sum"] = 10;
        }
    }
}
