using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Kiss.Data;
using Kiss.Data.Entity;

namespace Kiss.DataTest.Entity
{
    [TestFixture]
    public class TestActiveEntitySqLite : TestActiveEntityMsSql
    {
        protected override string DbName()
        {
            return "sqlite";
        }

    }

    [TestFixture]
    public class TestActiveEntityMySql : TestActiveEntityMsSql
    {
        protected override string DbName()
        {
            return "mysql";
        }

    }

    [TestFixture]
    public class TestActiveEntityPostgres : TestActiveEntityMsSql
    {
        protected override string DbName()
        {
            return "postgres";
        }
    }

    [TestFixture]
    public class TestActiveEntityOracle : TestActiveEntityMsSql
    {
        protected override string DbName()
        {
            return "oracle";
        }
    }

    [TestFixture]
    public class TestActiveEntityMsSql
    {
        protected virtual string DbName()
        {
            return "mssql";
        }

        protected List<CEntity> BuildTestData()
        {
            using (DbContent db = new DbContent(DbName()))
            {
                ActiveEntity<CEntity> ae = new ActiveEntity<CEntity>(db);
                ae.Delete(null);
            }

            List<CEntity> list = new List<CEntity>();
            using (DbContent db = new DbContent(DbName()))
            {
                ActiveEntity<CEntity> ae = new ActiveEntity<CEntity>(db);
                for (var i = 0; i < 10; i++)
                {
                    var data = CEntity.NewEntity();
                    var key = ae.Add(data);
                    data.PK = Convert.ToInt32(key);
                    list.Add(data);
                }
            }
            return list;
        }

        [Test]
        public void TestAdd()
        {
            using (DbContent db = new DbContent(DbName()))
            {
                var data = CEntity.NewEntity();
                ActiveEntity<CEntity> ae = new ActiveEntity<CEntity>(db);
                var key = ae.Add(data);

                string sql = string.Format("select max(pk) from {0} ", ActiveEntity<CEntity>.GetTableName());
                object want = db.TextScalar(sql);
                Assert.AreEqual(want, key, "Add");
            }

        }

        [Test]
        public void TestDelete()
        {
            List<CEntity> list = BuildTestData();
            int index = 0;
            using (DbContent db = new DbContent(DbName()))
            {
                ActiveEntity<CEntity> ae = new ActiveEntity<CEntity>(db);

                var entity = list[index];
                int key = entity.PK;
                ae.DeleteByKey(key);
                var actual = ae.QueryByKey(key);
                Assert.IsNull(actual, "DeleteByKey");

                index++;
                entity = list[index];
                key = entity.PK;
                ae.DeleteByFields((x) => x.CGuid, entity.CGuid);
                actual = ae.QueryByKey(key);
                Assert.IsNull(actual, "DeleteByFields");

                index++;
                entity = list[index];
                key = entity.PK;
                ae.DeleteByFields((x) => x.CString, entity.CString, (x) => x.CInt, entity.CInt);
                actual = ae.QueryByKey(key);
                Assert.IsNull(actual, "DeleteByFields");

                index++;
                entity = list[index];
                key = entity.PK;
                var cstring = entity.CString;
                var cint = entity.CInt;
                ae.Delete((x) => (x.CString == cstring && x.CInt == cint) || (x.PK == key));
                actual = ae.QueryByKey(key);
                Assert.IsNull(actual, "Delete");


                for (index++; index < list.Count; index++)
                {
                    entity = list[index];
                    key = entity.PK;
                    actual = ae.QueryByKey(key);
                    Assert.IsNotNull(actual, "QueryByKey");
                }
            }
        }

        [Test]
        public void TestQuery()
        {
            List<CEntity> list = BuildTestData();
            
            using (DbContent db = new DbContent(DbName()))
            {
                ActiveEntity<CEntity> ae = new ActiveEntity<CEntity>(db);
      
                var testEntity = list[0];
                object key = testEntity.PK;
                var entity = ae.QueryByKey(key);
                Assert.AreEqual(testEntity.ToString(), entity.ToString(), "QueryByKey");

                entity = ae.QueryByFields((x) => x.CGuid, testEntity.CGuid).FirstOrDefault();
                Assert.AreEqual(testEntity.ToString(), entity.ToString(), "QueryByFields");

                entity = ae.QueryByFields((x) => x.CString, testEntity.CString, (x) => x.CInt, testEntity.CInt).FirstOrDefault();
                Assert.AreEqual(testEntity.ToString(), entity.ToString(), "QueryByFields");

                var cstring = testEntity.CString;
                var cint = testEntity.CInt;
                var ckey = testEntity.PK;
                var cbool = testEntity.CBool;

                entity = ae.Query((x) => (x.CString == cstring && x.CInt == cint && x.CBool == cbool) || x.PK == ckey).FirstOrDefault();
                Assert.AreEqual(testEntity.ToString(), entity.ToString(), "Query");

            }
         
        }

       
        [Test]
        public void TestUpdate()
        {
            List<CEntity> list = BuildTestData();
            var testEntity = list[0];
            

            using (DbContent db = new DbContent(DbName()))
            {
                ActiveEntity<CEntity> ae = new ActiveEntity<CEntity>(db);

                CEntity newEntity = CEntity.NewEntity();
                object key = testEntity.PK;
                ae.UpdateByKey(newEntity, key);
                var entity = ae.QueryByKey(key);
                Assert.AreEqual(newEntity.Dump(), entity.Dump(), "UpdateByKey");

                newEntity = CEntity.NewEntity();

                ae.UpdateByFields(newEntity, (x) => x.CGuid, entity.CGuid);
                entity = ae.QueryByKey(key);
                Assert.AreEqual(newEntity.Dump(), entity.Dump(), "UpdateByFields");

                newEntity = CEntity.NewEntity();

                ae.UpdateByFields(newEntity, (x) => x.CString, entity.CString, (x) => x.CInt, entity.CInt);
                entity = ae.QueryByKey(key);
                Assert.AreEqual(newEntity.Dump(), entity.Dump(), "UpdateByFields");

                entity.CString = DateTime.Now.Ticks.ToString();
                ae.UpdateFieldsByKey(key, (x) => x.CString, entity.CString);
                var actual = entity = ae.QueryByKey(key);
                Assert.AreEqual(entity.Dump(), actual.Dump(), "UpdateFieldsByKey");

                entity.CString = DateTime.Now.Ticks.ToString();
                entity.CInt = Guid.NewGuid().GetHashCode();
                ae.UpdateFieldsByKey(key, (x) => x.CString, entity.CString, (x) => x.CInt, entity.CInt);
                actual = ae.QueryByKey(key);
                Assert.AreEqual(entity.Dump(), actual.Dump(), "UpdateFieldsByKey");

                entity = ae.QueryByKey(key);
                var cstring = entity.CString;
                var cint = entity.CInt;
                entity.CString = DateTime.Now.Ticks.ToString();
                ae.UpdateFields((x) => (x.CString == cstring && x.CInt == cint) || x.PK == entity.PK, (x) => x.CString, entity.CString);
                actual = ae.QueryByKey(key);
                Assert.AreEqual(entity.Dump(), actual.Dump(), "UpdateFields");

                entity = ae.QueryByKey(key);
                cstring = entity.CString;
                cint = entity.CInt;
                entity.CString = DateTime.Now.Ticks.ToString();
                entity.CInt = Guid.NewGuid().GetHashCode();
                ae.UpdateFields((x) => (x.CString == cstring && x.CInt == cint) || x.PK == entity.PK, (x) => x.CString, entity.CString, (x) => x.CInt, entity.CInt);
                actual = ae.QueryByKey(key);
                Assert.AreEqual(entity.Dump(), actual.Dump(), "UpdateFields");

            }

        }
    }
}
