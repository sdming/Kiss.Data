﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Kiss.Data;
using System.Configuration;
using System.Data;

namespace Kiss.DataTest.Driver
{
    [TestFixture]
    public class TestOracleDriver
    {
        private const string dbName = "oracle";

        [Test]
        public void TestCompileQuery()
        {
            var query = new ExpressionData().BuildQuery();
            var driver = Kiss.Data.Driver.SqlDriverFactory.OracleManaged();
            var command = driver.Compile(query, null);

            string expected = @"
select * from ( select kiss_row_.*, rownum kiss_rownum_ from ( 
SELECT DISTINCT cbool, cint, cfloat, cdatetime, cnumeric AS a_cnumeric, cstring AS a_cstring, COUNT(cint) AS avg_cint, COUNT(cint) AS count_cint, SUM(cint) AS sum_cint, MIN(cint) AS min_cint, MAX(cint) AS max_cint, cint - 1 AS exp_cint 
FROM ttable
WHERE
cbool = :P_0
AND
cbool <> :P_1
AND
cstring < :P_2
AND
cstring <= :P_3
AND
cstring > :P_4
AND
cstring >= :P_5
AND
cstring = :P_6
AND
cstring <> :P_7
AND
cstring IN (:P_8, :P_9, :P_10)
AND
cstring NOT IN (:P_11, :P_12, :P_13)
AND
cstring LIKE :P_14
AND
cstring NOT LIKE :P_15
AND
cint < :P_16
AND
cint <= :P_17
AND
cint > :P_18
AND
cint >= :P_19
AND
cint = :P_20
AND
cint <> :P_21
AND
cint IN (:P_22, :P_23, :P_24, :P_25, :P_26)
AND
cint NOT IN (:P_27, :P_28, :P_29, :P_30, :P_31)
AND
cfloat < :P_32
AND
cfloat <= :P_33
AND
cfloat > :P_34
AND
cfloat >= :P_35
AND
cfloat = :P_36
AND
cfloat <> :P_37
AND
cfloat IN (:P_38, :P_39, :P_40, :P_41, :P_42)
AND
cfloat NOT IN (:P_43, :P_44, :P_45, :P_46, :P_47)
AND
cnumeric < :P_48
AND
cnumeric <= :P_49
AND
cnumeric > :P_50
AND
cnumeric >= :P_51
AND
cnumeric = :P_52
AND
cnumeric <> :P_53
AND
cnumeric IN (:P_54, :P_55, :P_56, :P_57, :P_58)
AND
cnumeric NOT IN (:P_59, :P_60, :P_61, :P_62, :P_63)
AND
cdatetime < :P_64
AND
cdatetime <= :P_65
AND
cdatetime > :P_66
AND
cdatetime >= :P_67
AND
cdatetime = :P_68
AND
cdatetime <> :P_69
AND
cdatetime IN (:P_70, :P_71)
AND
cdatetime NOT IN (:P_72, :P_73)
AND
cguid = :P_74
AND
cguid <> :P_75
AND
(
	(
		cbytes IS NULL
		OR
		cbytes IS NOT NULL
	)
	OR
	(
		 1!=2 
		AND
		EXISTS(select count(*) from ttable where cint > 1)
		AND
		NOT EXISTS(select count(*) from ttable where cint > 10000)
		AND
		cint IN (select cint from ttable)
		AND
		cint NOT IN (select cint from ttable)
	)
) 
GROUP BY cbool, cint, cnumeric, cstring, cfloat, cdatetime, cint - 1 
HAVING
cstring LIKE :P_76
AND
cint NOT IN (:P_77, :P_78, :P_79, :P_80, :P_81)
AND
(
	cint < :P_82
	OR
	cint >= :P_83
)
AND
cnumeric = :P_84
AND
cbool IS NOT NULL
AND
COUNT(cint) < :P_85
AND
COUNT(cint) > :P_86
AND
SUM(cint) <> :P_87
AND
MIN(cint) <= 501
AND
MAX(cint) >= :P_88 
ORDER BY cint ASC, cfloat ASC, cnumeric DESC, cstring DESC, cdatetime ASC  
) kiss_row_ where rownum <=101) where kiss_rownum_ >3
";

            //Console.WriteLine(command.CommandText);

            Assert.AreEqual(AUtils.FS(expected), AUtils.FS(command.CommandText));
        }

        [Test]
        public void TestExecuteQuery()
        {
            var query = new ExpressionData().BuildQuery();

            var driver = Kiss.Data.Driver.SqlDriverFactory.OracleManaged();
            var command = driver.Compile(query, null);

            using (var db = new DbContent(dbName))
            {
                command.Connection = db.OpenConnection();
                command.ExecuteReader();
                db.CloseConnection();
            }
            Assert.True(true);
        }

        [Test]
        public void TestCompileInsert()
        {
            var insert = new ExpressionData().BuildInsert();
            var driver = Kiss.Data.Driver.SqlDriverFactory.OracleManaged();
            var command = driver.Compile(insert, null);

            string expected = @"
INSERT INTO TTABLE(CBOOL, CINT, CFLOAT, CNUMERIC, CSTRING, CDATETIME, CGUID)
VALUES(:P_0, :P_1, :P_2, :P_3, :P_4, :P_5, :P_6)
";

            //Console.WriteLine(command.CommandText);

            Assert.AreEqual(AUtils.FS(expected), AUtils.FS(command.CommandText));
        }

        [Test]
        public void TestExecuteInsert()
        {
            var insert = new ExpressionData().BuildInsert();
            var driver = Kiss.Data.Driver.SqlDriverFactory.OracleManaged();
            var command = driver.Compile(insert, null);

            using (var db = new DbContent(dbName))
            {
                command.Connection = db.OpenConnection();
                command.ExecuteNonQuery();
                db.CloseConnection();
            }
            Assert.True(true);

        }

        [Test]
        public void TestCompileUpdate()
        {
            var update = new ExpressionData().BuildUpdate();
            var driver = Kiss.Data.Driver.SqlDriverFactory.OracleManaged();
            var command = driver.Compile(update, null);

            string expected = @"
UPDATE TTABLE SET
CBOOL = :P_0, CINT = :P_1, CFLOAT = :P_2, CNUMERIC = :P_3, CSTRING = :P_4, CDATETIME = :P_5, CGUID = :P_6
WHERE
CINT = :P_7 
";
            //Console.WriteLine(command.CommandText);

            Assert.AreEqual(AUtils.FS(expected), AUtils.FS(command.CommandText));
        }

        [Test]
        public void TestExecuteUpdate()
        {
            var update = new ExpressionData().BuildUpdate();
            var driver = Kiss.Data.Driver.SqlDriverFactory.OracleManaged();
            var command = driver.Compile(update, null);

            using (var db = new DbContent(dbName))
            {
                command.Connection = db.OpenConnection();
                command.ExecuteNonQuery();
                db.CloseConnection();
            }
            Assert.True(true);

        }

        [Test]
        public void TestCompileDelete()
        {
            var delete = new ExpressionData().BuildDelete();
            var driver = Kiss.Data.Driver.SqlDriverFactory.OracleManaged();
            var command = driver.Compile(delete, null);

            string expected = @"
DELETE FROM TTABLE
WHERE
CINT = :P_0 
";
            //Console.WriteLine(command.CommandText);

            Assert.AreEqual(AUtils.FS(expected), AUtils.FS(command.CommandText));
        }

        [Test]
        public void TestExecuteDelete()
        {
            var delete = new ExpressionData().BuildDelete();
            var driver = Kiss.Data.Driver.SqlDriverFactory.OracleManaged();
            var command = driver.Compile(delete, null);

            using (var db = new DbContent(dbName))
            {
                command.Connection = db.OpenConnection();
                command.ExecuteNonQuery();
                db.CloseConnection();
            }
            Assert.True(true);

        }


        [Test]
        public void TestSchema()
        {
            var driver = Kiss.Data.Driver.SqlDriverFactory.OracleManaged();
            var table = driver.GetTable("ttable", ConfigurationManager.ConnectionStrings[dbName].ConnectionString);

            var pk = table.FindColumn("pk");
            Assert.True(pk.AllowDBNull == false, "pk.AllowDBNull");
            Assert.True(pk.DbType == DbType.Decimal, "pk.DbType");
            //Assert.True(pk.RuntimeType == typeof(Decimal), "pk.RuntimeType");
            Assert.True(pk.IsAutoIncrement == false, "pk.IsAutoIncrement");
            Assert.True(pk.IsReadOnly == false, "pk.IsReadOnly"); //
            Assert.True(pk.IsKey == true, "pk.IsKey");

            var cbool = table.FindColumn("cbool");
            Assert.True(cbool.AllowDBNull == true, "cbool.IsKey");
            //Assert.True(cbool.DbType == DbType.Boolean, "cbool.DbType");
            Assert.True(cbool.DbType == DbType.Int16, "cbool.DbType");
            //Assert.True(cbool.RuntimeType == typeof(Int16), "cbool.RuntimeType");
            Assert.True(cbool.IsAutoIncrement == false, "cbool.IsAutoIncrement");
            Assert.True(cbool.IsReadOnly == false, "cbool.IsReadOnly");
            Assert.True(cbool.IsKey == false, "cbool.IsKey");

            var cint = table.FindColumn("cint");
            Assert.True(cint.AllowDBNull == true, "cint.IsKey");
            Assert.True(cint.DbType == DbType.Decimal, "cint.DbType");
            //Assert.True(cint.RuntimeType == typeof(Decimal), "cint.RuntimeType");
            Assert.True(cint.IsAutoIncrement == false, "cint.IsAutoIncrement");
            Assert.True(cint.IsReadOnly == false, "cint.IsReadOnly");
            Assert.True(cint.IsKey == false, "cint.IsKey");

            var cfloat = table.FindColumn("cfloat");
            Assert.True(cfloat.AllowDBNull == true, "cfloat.IsKey");
            Assert.True(cfloat.DbType == DbType.Decimal, "cfloat.DbType");
            //Assert.True(cfloat.RuntimeType == typeof(Decimal), "cfloat.RuntimeType");
            Assert.True(cfloat.IsAutoIncrement == false, "cfloat.IsAutoIncrement");
            Assert.True(cfloat.IsReadOnly == false, "cfloat.IsReadOnly");
            Assert.True(cfloat.IsKey == false, "cfloat.IsKey");

            var cnumeric = table.FindColumn("cnumeric");
            Assert.True(cnumeric.AllowDBNull == true, "cnumeric.IsKey");
            Assert.True(cnumeric.DbType == DbType.Double, "cnumeric.DbType");
            //Assert.True(cnumeric.RuntimeType == typeof(Double), "cnumeric.RuntimeType");
            Assert.True(cnumeric.IsAutoIncrement == false, "cnumeric.IsAutoIncrement");
            Assert.True(cnumeric.IsReadOnly == false, "cnumeric.IsReadOnly");
            Assert.True(cnumeric.IsKey == false, "cnumeric.IsKey");

            var cstring = table.FindColumn("cstring");
            Assert.True(cstring.AllowDBNull == true, "cstring.IsKey");
            Assert.True(cstring.DbType == DbType.String, "cstring.DbType");
            //Assert.True(cstring.RuntimeType == typeof(string), "cstring.RuntimeType");
            Assert.True(cstring.IsAutoIncrement == false, "cstring.IsAutoIncrement");
            Assert.True(cstring.IsReadOnly == false, "cstring.IsReadOnly");
            Assert.True(cstring.IsKey == false, "cstring.IsKey");

            var cdatetime = table.FindColumn("cdatetime");
            Assert.True(cdatetime.AllowDBNull == true, "cdatetime.IsKey");
            Assert.True(cdatetime.DbType == DbType.DateTime || cdatetime.DbType == DbType.Date, "cdatetime.DbType");
            //Assert.True(cdatetime.RuntimeType == typeof(DateTime), "cdatetime.RuntimeType");
            Assert.True(cdatetime.IsAutoIncrement == false, "cdatetime.IsAutoIncrement");
            Assert.True(cdatetime.IsReadOnly == false, "cdatetime.IsReadOnly");
            Assert.True(cdatetime.IsKey == false, "cdatetime.IsKey");

            var cguid = table.FindColumn("cguid");
            Assert.True(cguid.AllowDBNull == true, "cguid.IsKey");
            Assert.True(cguid.DbType == DbType.String || cguid.DbType == DbType.Binary, "cguid.DbType");
            //Assert.True(cguid.RuntimeType == typeof(String) || cguid.RuntimeType == typeof(byte[]), "cguid.RuntimeType");
            Assert.True(cguid.IsAutoIncrement == false, "cguid.IsAutoIncrement");
            Assert.True(cguid.IsReadOnly == false, "cguid.IsReadOnly");
            Assert.True(cguid.IsKey == false, "cguid.IsKey");

            var cbytes = table.FindColumn("cbytes");
            Assert.True(cbytes.AllowDBNull == true, "cbytes.IsKey");
            Assert.True(cbytes.DbType == DbType.Binary || cbytes.DbType == DbType.String, "cbytes.DbType");
            //Assert.True(cbytes.RuntimeType == typeof(byte[]) || cbytes.RuntimeType == typeof(string), "cbytes.RuntimeType");
            Assert.True(cbytes.IsAutoIncrement == false, "cbytes.IsAutoIncrement");
            Assert.True(cbytes.IsReadOnly == false, "cbytes.IsReadOnly");
            Assert.True(cbytes.IsKey == false, "cbytes.IsKey");
        }

    }
}
