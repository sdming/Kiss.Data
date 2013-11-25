using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Kiss.Core;
using Kiss.Data;
using Kiss.Data.Expression;

namespace Kiss.DataTest
{
    [TestFixture]
    public class TestTableGate
    {
        protected virtual string DbName
        {
            get { return "mssql"; }
        }

        protected virtual string TableName
        {
            get { return "ttable"; }
        }

        protected virtual string Current()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        protected IDataObjectAdapter CreateIAdapter()
        {
            return Kiss.Core.Adapter.Dictionary();
        }

        protected IDataObjectAdapter BuildAdapter()
        {
            var data = ExpressionData.BuildDataTypeMap();
            return Kiss.Core.Adapter.Dictionary(data);
        }

        protected ContentListener listener = new ContentListener();

        [Test]
        public void TestExists()
        {
            using (DbContent db = new DbContent(DbName))
            {
                var t = db.Table(TableName);
                db.Listener = listener;

                listener.Clear();
                t.Exists("cint", 101);
                Assert.AreEqual(AUtils.FS(@"
SELECT COUNT(*) AS [count_number] 
FROM [ttable]
WHERE
[cint] = @cint_1;
                "), AUtils.FS(listener.CommandText));
            }

            using (DbContent db = new DbContent(DbName))
            {
                var t = db.Table(TableName);
                db.Listener = listener;

                listener.Clear();
                t.Exists("cint", SqlOperator.GreaterThan, 101);
                Assert.AreEqual(AUtils.FS(@"
SELECT COUNT(*) AS [count_number] 
FROM [ttable]
WHERE
[cint] > @cint_1;
                "), AUtils.FS(listener.CommandText));
            }

            using (DbContent db = new DbContent(DbName))
            {
                var t = db.Table(TableName);
                db.Listener = listener;

                listener.Clear();
                t.Exists("cint", 101, "cstring", "string");
                Assert.AreEqual(AUtils.FS(@"
SELECT COUNT(*) AS [count_number] 
FROM [ttable]
WHERE
[cint] = @cint_1
and
[cstring] = @cstring_2;
                "), AUtils.FS(listener.CommandText));
            }

            using (DbContent db = new DbContent(DbName))
            {
                var t = db.Table(TableName);
                db.Listener = listener;

                listener.Clear();
                t.Exists("cint", SqlOperator.GreaterThan, 101, "cstring", SqlOperator.LessThan, "string");
                Assert.AreEqual(AUtils.FS(@"
SELECT COUNT(*) AS [count_number] 
FROM [ttable]
WHERE
[cint] > @cint_1
and
[cstring] < @cstring_2;
                "), AUtils.FS(listener.CommandText));
            }

        }

        [Test]
        public void TestDelete()
        {
            using (DbContent db = new DbContent(DbName))
            {
                var t = db.Table(TableName);
                db.Listener = listener;

                listener.Clear();
                t.Delete("cint", 101);
                Assert.AreEqual(AUtils.FS(@"
DELETE FROM [ttable] 
WHERE
[cint] = @cint_1
;
                "), AUtils.FS(listener.CommandText));
            }

            using (DbContent db = new DbContent(DbName))
            {
                var t = db.Table(TableName);
                db.Listener = listener;

                listener.Clear();
                t.Delete("cint", SqlOperator.GreaterThan, 101);
                Assert.AreEqual(AUtils.FS(@"
DELETE FROM [ttable]
WHERE
[cint] > @cint_1
;
                "), AUtils.FS(listener.CommandText));
            }

            using (DbContent db = new DbContent(DbName))
            {
                var t = db.Table(TableName);
                db.Listener = listener;

                listener.Clear();
                t.Delete("cint", 101, "cstring", "string");
                Assert.AreEqual(AUtils.FS(@"
DELETE FROM [ttable]
WHERE
[cint] = @cint_1
and
[cstring] = @cstring_2
;
                "), AUtils.FS(listener.CommandText));
            }

            using (DbContent db = new DbContent(DbName))
            {
                var t = db.Table(TableName);
                db.Listener = listener;

                listener.Clear();
                t.Delete("cint", SqlOperator.GreaterThan, 101, "cstring", SqlOperator.LessThan, "string");
                Assert.AreEqual(AUtils.FS(@"
DELETE FROM [ttable]
WHERE
[cint] > @cint_1
and
[cstring] < @cstring_2
;
                "), AUtils.FS(listener.CommandText));
            }

        }

        [Test]
        public void TestReadCount()
        {
            using (DbContent db = new DbContent(DbName))
            {
                var t = db.Table(TableName);
                db.Listener = listener;

                listener.Clear();
                t.ReadCount("cint", 101);
                Assert.AreEqual(AUtils.FS(@"
SELECT COUNT(*) AS [count_number] 
FROM [ttable]
WHERE
[cint] = @cint_1;
                "), AUtils.FS(listener.CommandText));
            }

            using (DbContent db = new DbContent(DbName))
            {
                var t = db.Table(TableName);
                db.Listener = listener;

                listener.Clear();
                t.ReadCount("cint", SqlOperator.GreaterThan, 101);
                Assert.AreEqual(AUtils.FS(@"
SELECT COUNT(*) AS [count_number] 
FROM [ttable]
WHERE
[cint] > @cint_1;
                "), AUtils.FS(listener.CommandText));
            }

            using (DbContent db = new DbContent(DbName))
            {
                var t = db.Table(TableName);
                db.Listener = listener;

                listener.Clear();
                t.ReadCount("cint", 101, "cstring", "string");
                Assert.AreEqual(AUtils.FS(@"
SELECT COUNT(*) AS [count_number] 
FROM [ttable]
WHERE
[cint] = @cint_1
and
[cstring] = @cstring_2;
                "), AUtils.FS(listener.CommandText));
            }

            using (DbContent db = new DbContent(DbName))
            {
                var t = db.Table(TableName);
                db.Listener = listener;

                listener.Clear();
                t.ReadCount("cint", SqlOperator.GreaterThan, 101, "cstring", SqlOperator.LessThan, "string");
                Assert.AreEqual(AUtils.FS(@"
SELECT COUNT(*) AS [count_number] 
FROM [ttable]
WHERE
[cint] > @cint_1
and
[cstring] < @cstring_2;
                "), AUtils.FS(listener.CommandText));
            }

        }

        [Test]
        public void TestReadCell()
        {
            using (DbContent db = new DbContent(DbName))
            {
                var t = db.Table(TableName);
                db.Listener = listener;

                listener.Clear();
                t.ReadCell("cnumeric", "cint", 101);
                Assert.AreEqual(AUtils.FS(@"
SELECT [cnumeric]
FROM [ttable]
WHERE
[cint] = @cint_1;
                "), AUtils.FS(listener.CommandText));
            }

            using (DbContent db = new DbContent(DbName))
            {
                var t = db.Table(TableName);
                db.Listener = listener;

                listener.Clear();
                t.ReadCell("cnumeric", "cint", 101, "cstring", "string");
                Assert.AreEqual(AUtils.FS(@"
SELECT [cnumeric]
FROM [ttable]
WHERE
[cint] = @cint_1
and
[cstring] = @cstring_2;
                "), AUtils.FS(listener.CommandText));
            }

            
        }

        [Test]
        public void TestReadColumn()
        {
            using (DbContent db = new DbContent(DbName))
            {
                var t = db.Table(TableName);
                db.Listener = listener;

                listener.Clear();
                t.ReadColumn<double>("cnumeric", true, "cint", 101);
                Assert.AreEqual(AUtils.FS(@"
SELECT DISTINCT [cnumeric]
FROM [ttable]
WHERE
[cint] = @cint_1;
                "), AUtils.FS(listener.CommandText));
            }

            using (DbContent db = new DbContent(DbName))
            {
                var t = db.Table(TableName);
                db.Listener = listener;

                listener.Clear();
                t.ReadColumn<double>("cnumeric", false, "cint", SqlOperator.GreaterThan, 101);
                Assert.AreEqual(AUtils.FS(@"
SELECT [cnumeric]
FROM [ttable]
WHERE
[cint] > @cint_1;
                "), AUtils.FS(listener.CommandText));
            }

            using (DbContent db = new DbContent(DbName))
            {
                var t = db.Table(TableName);
                db.Listener = listener;

                listener.Clear();
                t.ReadColumn<double>("cnumeric", true, "cint", 101, "cstring", "string");
                Assert.AreEqual(AUtils.FS(@"
SELECT DISTINCT [cnumeric]
FROM [ttable]
WHERE
[cint] = @cint_1
and
[cstring] = @cstring_2;
                "), AUtils.FS(listener.CommandText));
            }

            using (DbContent db = new DbContent(DbName))
            {
                var t = db.Table(TableName);
                db.Listener = listener;

                listener.Clear();
                t.ReadColumn<double>("cnumeric", false, "cint", SqlOperator.GreaterThan, 101, "cstring", SqlOperator.LessThan, "string");
                Assert.AreEqual(AUtils.FS(@"
SELECT [cnumeric]
FROM [ttable]
WHERE
[cint] > @cint_1
and
[cstring] < @cstring_2;
                "), AUtils.FS(listener.CommandText));
            }

        }

        
        [Test]
        public void TestInsert()
        {
            using (DbContent db = new DbContent(DbName))
            {
                var t = db.Table(TableName);
                db.Listener = listener;

                listener.Clear();
                var data = BuildAdapter();
                t.Insert(data);
                Assert.AreEqual(AUtils.FS(@"
INSERT INTO [ttable]([cbool], [cint], [cfloat], [cnumeric], [cstring], [cdatetime], [cguid])
OUTPUT INSERTED.pk 
VALUES(@cbool , @cint , @cfloat , @cnumeric , @cstring , @cdatetime , @cguid ) ;
                "), AUtils.FS(listener.CommandText));
            }

            using (DbContent db = new DbContent(DbName))
            {
                var t = db.Table(TableName);
                db.Listener = listener;

                listener.Clear();
                var data = BuildAdapter();
                data.Set("new_cint", 123);
                t.Insert(data, ((x) => x == "cint" ? "new_cint" : x), data.Fields(), new string[]{"cguid", "cbytes", "cdatetime"}  );
                Assert.AreEqual(AUtils.FS(@"
INSERT INTO [ttable]([cbool], [cint], [cfloat], [cnumeric], [cstring])
OUTPUT INSERTED.pk 
VALUES(@cbool , @cint , @cfloat , @cnumeric , @cstring ) ;
                "), AUtils.FS(listener.CommandText));
            }

        }

        [Test]
        public void TestUpdate()
        {
            using (DbContent db = new DbContent(DbName))
            {
                var t = db.Table(TableName);
                db.Listener = listener;

                listener.Clear();
                var data = BuildAdapter();
                t.Update(data, "cint", 101);
                Assert.AreEqual(AUtils.FS(@"
UPDATE [ttable] SET
[cbool] = @cbool , [cint] = @cint , [cfloat] = @cfloat , [cnumeric] = @cnumeric , [cstring] = @cstring , [cdatetime] = @cdatetime , [cguid] = @cguid 
WHERE
[cint] = @cint_1
;
                "), AUtils.FS(listener.CommandText));
            }

            using (DbContent db = new DbContent(DbName))
            {
                var t = db.Table(TableName);
                db.Listener = listener;

                listener.Clear();
                var data = BuildAdapter();
                t.Update(data, "cint", SqlOperator.GreaterThan, 101);
                Assert.AreEqual(AUtils.FS(@"
UPDATE [ttable] SET
[cbool] = @cbool , [cint] = @cint , [cfloat] = @cfloat , [cnumeric] = @cnumeric , [cstring] = @cstring , [cdatetime] = @cdatetime , [cguid] = @cguid 
WHERE
[cint] > @cint_1 
;
                "), AUtils.FS(listener.CommandText));
            }

            using (DbContent db = new DbContent(DbName))
            {
                var t = db.Table(TableName);
                db.Listener = listener;

                listener.Clear();
                var data = BuildAdapter();
                t.Update(data, "cint", 101, "cstring", "string");
                Assert.AreEqual(AUtils.FS(@"
UPDATE [ttable] SET
[cbool] = @cbool , [cint] = @cint , [cfloat] = @cfloat , [cnumeric] = @cnumeric , [cstring] = @cstring , [cdatetime] = @cdatetime , [cguid] = @cguid 
WHERE
[cint] = @cint_1 
AND
[cstring] = @cstring_2  
;
                "), AUtils.FS(listener.CommandText));
            }

            using (DbContent db = new DbContent(DbName))
            {
                var t = db.Table(TableName);
                db.Listener = listener;

                listener.Clear();
                var data = BuildAdapter();
                t.Update(data, "cint", SqlOperator.GreaterThan, 101, "cstring", SqlOperator.LessThan, "string");
                Assert.AreEqual(AUtils.FS(@"
UPDATE [ttable] SET
[cbool] = @cbool , [cint] = @cint , [cfloat] = @cfloat , [cnumeric] = @cnumeric , [cstring] = @cstring , [cdatetime] = @cdatetime , [cguid] = @cguid 
WHERE
[cint] > @cint_1 
AND
[cstring] < @cstring_2  
;
                "), AUtils.FS(listener.CommandText));
            }

            using (DbContent db = new DbContent(DbName))
            {
                var t = db.Table(TableName);
                db.Listener = listener;

                listener.Clear();
                var data = BuildAdapter();
                t.Update(data, null, ((x) => x == "cint" ? "new_cint" : x), data.Fields(), new string[] { "cguid", "cbytes", "cdatetime" });
                Assert.AreEqual(AUtils.FS(@"
UPDATE [ttable] SET
[cbool] = @cbool , [cfloat] = @cfloat , [cnumeric] = @cnumeric , [cstring] = @cstring 
;
                "), AUtils.FS(listener.CommandText));
            }
        }

        [Test]
        public void TestUpdateColumn()
        {
            using (DbContent db = new DbContent(DbName))
            {
                var t = db.Table(TableName);
                db.Listener = listener;

                listener.Clear();
                var data = BuildAdapter();
                t.UpdateColumn("cnumeric", data.Get("cnumeric"), "cint", 101);
                Assert.AreEqual(AUtils.FS(@"
UPDATE [ttable] SET
[cnumeric] = @cnumeric 
WHERE
[cint] = @cint_1  
;
                "), AUtils.FS(listener.CommandText));
            }

            using (DbContent db = new DbContent(DbName))
            {
                var t = db.Table(TableName);
                db.Listener = listener;

                listener.Clear();
                var data = BuildAdapter();
                t.UpdateColumn("cnumeric", data.Get("cnumeric"), ExpressionData.BuildSimpleWhere());
                Assert.AreEqual(AUtils.FS(@"
UPDATE [ttable] SET
[cnumeric] = @cnumeric 
WHERE
[cbool] = @P_0
AND
[cint] <= @P_1
AND
[cfloat] > @P_2
AND
[cnumeric] >= @P_3
AND
[cstring] LIKE @P_4
AND
[cdatetime] > @P_5
AND
[cbytes] IS NULL  
;
                "), AUtils.FS(listener.CommandText));
            }

            
        }
    }
}
