using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kiss.Data;
using Kiss.Core;
using NUnit.Framework;
using System.Data;

namespace Kiss.DataTest
{
    [TestFixture]
    public abstract class TestSqlContentCommand
    {
        #region driver
        protected virtual string ParameterPrefix()
        {
            return "@";
        }

        protected virtual string ParameterName(string name)
        {
            return ParameterPrefix() + name;
        }

        protected abstract string DbName();

        protected virtual string TableName()
        {
            return "ttable";
        }

        protected virtual string Current()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        protected virtual string QuoteString(string s)
        {
            return "'" + s + "'";
        }

        protected IDataObjectAdapter CreateIAdapter()
        {
            return Kiss.Core.Adapter.Dictionary();
        }
        #endregion

        #region procedure
        //noquery
        //reader
        //Scalar
        //output
        #endregion

        #region text
        
        [Test]
        public void TestTextNonQuery_IAdapter()
        {
            string commandText = string.Format("update {0} set cstring = {1} where cint = {2}",
                TableName(), ParameterName("cstring"), ParameterName("cint"));
            IDataObjectAdapter parameters = CreateIAdapter();
            parameters.Set("cint", 101);
            parameters.Set("cstring", Current());

            using(DbContent db = new DbContent(DbName()))
            {
                var i = db.TextNonQuery(commandText, parameters);
                Assert.GreaterOrEqual(i, -1);
            }
        }

        [Test]
        public void TestTextNonQuery_IAdapterNull()
        {
            string commandText = string.Format("update {0} set cstring = {1} where cint > 1 ",
                TableName(), QuoteString(Current()));
            IDataObjectAdapter parameters = null;

            using (DbContent db = new DbContent(DbName()))
            {
                var i = db.TextNonQuery(commandText, parameters);
                Assert.GreaterOrEqual(i, -1);
            }
        }

        [Test]
        public void TestTextNonQuery_Params()
        {
            string commandText = string.Format("update {0} set cstring = {1} where cint = {2}",
                TableName(), ParameterName("cstring"), ParameterName("cint"));
            
            using (DbContent db = new DbContent(DbName()))
            {
                var i = db.TextNonQuery(commandText, Current(), 101);
                Assert.GreaterOrEqual(i, -1);
            }
        }

        [Test]
        public void TestTextNonQuery_ParamsNull()
        {
            string commandText = string.Format("update {0} set cstring = {1} where cint > 1 ",
                TableName(), QuoteString(Current()));

            using (DbContent db = new DbContent(DbName()))
            {
                var i = db.TextNonQuery(commandText);
                Assert.GreaterOrEqual(i, -1);
            }
        }

        [Test]
        public void TestTextReader_IAdapter()
        {
            string commandText = string.Format("select * from {0} where cint > {1}",
                TableName(), ParameterName("cint"));
            IDataObjectAdapter parameters = CreateIAdapter();
            parameters.Set("cint", 101);

            using (DbContent db = new DbContent(DbName()))
            {
                var reader = db.TextReader(commandText, parameters);
                Assert.GreaterOrEqual(reader.FieldCount, 1);
            }
        }

        [Test]
        public void TestTextReader_IAdapterNull()
        {
            string commandText = string.Format("select * from {0} where cint > {1}",
                TableName(), 101);
            IDataObjectAdapter parameters = null;
            
            using (DbContent db = new DbContent(DbName()))
            {
                var reader = db.TextReader(commandText, parameters);
                Assert.GreaterOrEqual(reader.FieldCount, 1);
            }
        }

        [Test]
        public void TestTextReader_Params()
        {
            string commandText = string.Format("select * from {0} where cint > {1}",
                TableName(), ParameterName("cint"));
            
            using (DbContent db = new DbContent(DbName()))
            {
                var reader = db.TextReader(commandText, 101);
                Assert.GreaterOrEqual(reader.FieldCount, 1);
            }
        }

        [Test]
        public void TestTextReader_ParamsNull()
        {
            string commandText = string.Format("select * from {0} where cint > {1}",
                TableName(), 101);

            using (DbContent db = new DbContent(DbName()))
            {
                var reader = db.TextReader(commandText);
                Assert.GreaterOrEqual(reader.FieldCount, 1);
            }
        }

        [Test]
        public void TestTextScalar_IAdapter()
        {
            string commandText = string.Format("select count(*) as count from {0} where cint > {1}",
                TableName(), ParameterName("cint"));
            IDataObjectAdapter parameters = CreateIAdapter();
            parameters.Set("cint", 101);

            using (DbContent db = new DbContent(DbName()))
            {
                var count = db.TextScalar(commandText, parameters);
                Assert.GreaterOrEqual(Convert.ToInt32(count), -1);
            }
        }

        [Test]
        public void TestTextScalar_IAdapterNull()
        {
            string commandText = string.Format("select count(*) as count from {0} where cint > {1}",
                TableName(), 101);
            IDataObjectAdapter parameters = null;
            
            using (DbContent db = new DbContent(DbName()))
            {
                var count = db.TextScalar(commandText, parameters);
                Assert.GreaterOrEqual(Convert.ToInt32(count), -1);
            }
        }

        [Test]
        public void TestTextScalar_Params()
        {
            string commandText = string.Format("select count(*) as count from {0} where cint > {1}",
                TableName(), ParameterName("cint"));
            
            using (DbContent db = new DbContent(DbName()))
            {
                var count = db.TextScalar(commandText, 101);
                Assert.GreaterOrEqual(Convert.ToInt32(count), -1);
            }
        }

        [Test]
        public void TestTextScalar_ParamsNull()
        {
            string commandText = string.Format("select count(*) as count from {0} where cint > {1}",
                TableName(), 101);

            using (DbContent db = new DbContent(DbName()))
            {
                var count = db.TextScalar(commandText);
                Assert.GreaterOrEqual(Convert.ToInt32(count), -1);
            }
        }

        [Test]
        public void TestExecuteReader_Close()
        {
            using (DbContent db = new DbContent(DbName()))
            {
                var cmd = db.Driver().CreateCommand();
                cmd.CommandText = string.Format("select * from {0} where cint > 101 ", TableName());
                var reader = db.ExecuteReader(cmd, CommandBehavior.CloseConnection, true);

                Assert.AreEqual(db.State(), ConnectionState.Closed);
                while (reader.Read()) { }
                Assert.GreaterOrEqual(reader.FieldCount, 1);
            }           
        }

        [Test]
        public void TestExecuteReader_NotClose()
        {
            using (DbContent db = new DbContent(DbName()))
            {
                var cmd = db.Driver().CreateCommand();
                cmd.CommandText = string.Format("select * from {0} where cint > 101 ",  TableName());
                var reader = db.ExecuteReader(cmd, CommandBehavior.Default, false);

                Assert.AreNotEqual(db.State(), ConnectionState.Closed);
                while (reader.Read()) { }
                Assert.AreNotEqual(db.State(), ConnectionState.Closed);
                Assert.GreaterOrEqual(reader.FieldCount, 1);
            }
        }

        [Test]
        public void TestExecuteScalar()
        {
            using (DbContent db = new DbContent(DbName()))
            {
                var cmd = db.Driver().CreateCommand();
                cmd.CommandText = string.Format("select count(*) from {0} ", TableName());
                var count = db.ExecuteScalar(cmd);
                Assert.GreaterOrEqual(Convert.ToInt32(count), -1);
            }
        }

        [Test]
        public void TestExecuteNonQuery()
        {
            using (DbContent db = new DbContent(DbName()))
            {
                var cmd = db.Driver().CreateCommand();
                cmd.CommandText = string.Format("update {0} set cstring = {1} where cint > 1 ",
                    TableName(), QuoteString(Current()));

                var i = db.ExecuteNonQuery(cmd);
                Assert.GreaterOrEqual(i, -1);
            }
        }
        #endregion

    }

    public class TestMsSqlContentCommand : TestSqlContentCommand
    {
        protected override string DbName()
        {
            return "mssql";
        }
    }
}
