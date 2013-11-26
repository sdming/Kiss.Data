using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kiss.Data;
using Kiss.DataTest.Entity;
using Kiss.Data.Entity;
using Kiss.Data.Expression;

namespace Kiss.DataTest
{
    public class Demo
    {

        public void Text()
        {
            using (DbContent db = new DbContent("mssql"))
            {
                var data = Kiss.Core.Adapter.Dictionary();
                data.Set("cint", 101);
                data.Set("pk", 11606);
                db.TextNonQuery("update TTABLE set cint = @cint where pk = @pk", data);
            }

            using (DbContent db = new DbContent("mssql"))
            {
                var i = db.TextNonQuery("update TTABLE set cint = @cint where pk = @pk", 102, 11606);
            }

            using (DbContent db = new DbContent("mssql"))
            {
                var exp = new Kiss.Data.Expression.Text("update TTABLE set cint = @cint where pk = @pk")
                    .Set("cint", 103)
                    .Set("pk", 11606);

                db.ExecuteNonQuery(exp);
            }
        }

        public void Procedure()
        {
            using (DbContent db = new DbContent("mssql"))
            {
                Dictionary<string, object> data = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                data["cint"] = 101;
                var table = db.ProcedureReader("usp_query", Kiss.Core.Adapter.Dictionary(data)).ToTable();
            }

            using (DbContent db = new DbContent("mssql"))
            {
                db.ProcedureNonQuery("usp_exec", 11606);
            }
   
            using (DbContent db = new DbContent("mssql"))
            {
                var data = Kiss.Core.Adapter.Dictionary();
                data.Set("x", 2);
                data.Set("y", 7);
                IExecuteResult r;
                db.ProcedureNonQuery("usp_inout", data, out r);
                var output = r.Output();
                Console.WriteLine("y:{0},sum:{1}", output["y"], output["sum"]);
            }

            using (DbContent db = new DbContent("mssql"))
            {
                var exp = new Kiss.Data.Expression.Procedure("usp_exec")
                    .Set("cint", 64);

                db.ExecuteNonQuery(exp);               
            }

            using (DbContent db = new DbContent("mssql"))
            {
                IPorxyTest proxy = FunctionProxy<IPorxyTest>.Create(db);
                var reader = proxy.usp_query(101);
                reader.Dispose();
            }

            using (DbContent db = new DbContent("mssql"))
            {
                IPorxyTest proxy = FunctionProxy<IPorxyTest>.Create(db);
                var reader = proxy.Exec(101);
                Console.WriteLine(reader.RecordsAffected);
                reader.Dispose();
            }

            using (DbContent db = new DbContent("mssql"))
            {
                int x = 3;
                int y = 7;
                int sum;

                IPorxyTest proxy = FunctionProxy<IPorxyTest>.Create(db);
                proxy.usp_inout(x, ref y, out sum);
                Console.WriteLine("y:{0},sum:{1}", y, sum);
            } 
        }

        public void Insert()
        {
            using (DbContent db = new DbContent("mssql"))
            {
                var insert = new Kiss.Data.Expression.Insert("ttable")
                    .Set("cbool", true)
                    .Set("cint", 42)
                    .Set("cfloat", 3.14)
                    .Set("cnumeric", 1.1)
                    .Set("cstring", "string")
                    .Set("cdatetime", "2004-07-24");

                db.ExecuteNonQuery(insert);
            }

            using (DbContent db = new DbContent("mssql"))
            {
                var data = Kiss.Core.Adapter.Dictionary();
                data.Set("A_cbool", true);
                data.Set("A_cint", 42);
                data.Set("A_cfloat", 3.14);
                data.Set("A_cnumeric", 1.1);
                data.Set("A_cstring", "string");
                data.Set("A_cdatetime", "2004-07-24");
                
                db.Table("ttable").Insert(data, (x)=> "A_" + x, null, new string[]{"A_cint"});
            }

            using (DbContent db = new DbContent("mssql"))
            {
                var data = CEntity.NewEntity();
                ActiveEntity<CEntity> ae = new ActiveEntity<CEntity>(db);
                var pk = ae.Add(data);
                Console.WriteLine(pk);
            }
        }

        public void Delete()
        {
            using (DbContent db = new DbContent("mssql"))
            {
                var delete = new Kiss.Data.Expression.Delete("ttable");

                delete.Where
                    .GreaterThan("cint", 10100)
                    .LessOrEquals("cint", 20200)
                    .OpenParentheses()
                    .IsNotNull("cbool")
                    .Or()
                    .IsNull("cbytes")
                    .CloseParentheses();

                db.ExecuteNonQuery(delete);
            }

            using (DbContent db = new DbContent("mssql"))
            {
                var delete = new Kiss.Data.Expression.Delete("ttable");

                delete.Where
                    .In("cint", new int[]{1,2,3,4,5});

                db.ExecuteNonQuery(delete);
            }

            using (DbContent db = new DbContent("mssql"))
            {
                db.Table("ttable").Delete("cint", 101); //cint = 101
            }

            using (DbContent db = new DbContent("mssql"))
            {                
                ActiveEntity<CEntity> ae = new ActiveEntity<CEntity>(db);
                ae.DeleteByKey(101); //pk = 101
            }

            using (DbContent db = new DbContent("mssql"))
            {
                ActiveEntity<CEntity> ae = new ActiveEntity<CEntity>(db);
                ae.DeleteByFields((x) => x.CInt, 101); //cint = 101
            }

            using (DbContent db = new DbContent("mssql"))
            {
                ActiveEntity<CEntity> ae = new ActiveEntity<CEntity>(db);
                ae.Delete((x) => x.CInt > 10100 && (x.CInt < 20200 || x.CFloat < 12.12) ); 
                //cint > 10100 and(cint < 20200 or cfloat < 12.12)
            }
        }

        public void Update()
        {
            using (DbContent db = new DbContent("mssql"))
            {
                var update = new Kiss.Data.Expression.Update("ttable");
                update
                    .Set("cstring", "new string")
                    .Set("cdatetime", DateTime.Now)
                .Limit(10)
                .Where
                    .EqualsTo("cint", 101);

                db.ExecuteNonQuery(update);
            }

            using (DbContent db = new DbContent("mssql"))
            {
                var data = Kiss.Core.Adapter.Dictionary();
                data.Set("cint", 420);
                data.Set("cfloat", 3.141);
                data.Set("cnumeric", 1.12);

                db.Table("ttable").Update(data, "cint", 101);
            }

            using (DbContent db = new DbContent("mssql"))
            {
                var where = new Where()
                    .EqualsTo("cint", 101);

                db.Table("ttable").UpdateColumn("cstring", "a string", where);
            }

            using (DbContent db = new DbContent("mssql"))
            {
                CEntity entity = CEntity.NewEntity();
                ActiveEntity<CEntity> ae = new ActiveEntity<CEntity>(db);
                ae.UpdateByKey(entity, 11606);
            }

            using (DbContent db = new DbContent("mssql"))
            {
                CEntity entity = CEntity.NewEntity();
                ActiveEntity<CEntity> ae = new ActiveEntity<CEntity>(db);
                ae.UpdateByFields(entity, (x) => x.CInt, 101);
            }

            using (DbContent db = new DbContent("mssql"))
            {
                var data = new Dictionary<string, object>();
                data["cstring"] = "some string";
                data["cfloat"] = 3.14 * 3.14;
                ActiveEntity<CEntity> ae = new ActiveEntity<CEntity>(db);
                ae.UpdateFields((x) => x.CInt > 101 && x.CInt < 202, data);
            }
        }

        public void Query()
        {
            using (DbContent db = new DbContent("mssql"))
            {
                var query= new Data.Expression.Query("ttable");
                query.Where
                    .EqualsTo("cint" , 10100)
                    .EqualsTo("cint", 20200);

                var reader = db.ExecuteReader(query);
                reader.Dispose();
            }

            using (DbContent db = new DbContent("mssql"))
            {
                var reader = db.Table("ttable").Read("cint", 10100, "cint", 20200);
                reader.Dispose();
            }

            using (DbContent db = new DbContent("mssql"))
            {
                var i = db.Table("ttable").ReadCell("cint", "pk", "11606");
            }

            using (DbContent db = new DbContent("mssql"))
            {
                var list = db.Table("ttable").ReadColumn<string>("cstring", false, "cint", SqlOperator.GreaterThan, 202);
            }

            using (DbContent db = new DbContent("mssql"))
            {                
                ActiveEntity<CEntity> ae = new ActiveEntity<CEntity>(db);
                var list = ae.QueryByFields((x)=>x.PK, 11606);
            }

            using (DbContent db = new DbContent("mssql"))
            {
                ActiveEntity<CEntity> ae = new ActiveEntity<CEntity>(db);
                var list = ae.Query((x) => x.CInt > 101 && x.CInt < 20200);
            }
        }
    }
}
