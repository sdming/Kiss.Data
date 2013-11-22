using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Kiss.Data.Expression;
using Kiss.Data.Entity;
using Kiss.Data;

namespace Kiss.DataTest.Entity
{
    [TestFixture]
    public class TestLambdaVisitor
    {
        [Test]
        public void TestCompile()
        {
            if (true)
            {
                var actual = Compile<CEntity>((x) => x.CBool == true);
                var want = "where cbool = true";
                Assert.AreEqual(AUtils.FS(want), AUtils.FS(actual.ToString()), "x.CBool == true");
            }

            if (true)
            {
                var actual = Compile<CEntity>((x) => x.CInt >= 1 && x.CInt <= 101);
                var want = "where (cint >= 1) and (cint <= 101)";
                Assert.AreEqual(AUtils.FS(want), AUtils.FS(actual.ToString()), "x.CInt >= 1 && x.CInt <= 101");
            }

            if (true)
            {
                var actual = Compile<CEntity>((x) => x.CFloat < 1.1 || x.CFloat > 11.11 || (x.CString != "ABC"));
                var want = "where ((cfloat < 1.1) or (cfloat > 11.11)) or (cstring <> ABC)";
                Assert.AreEqual(AUtils.FS(want), AUtils.FS(actual.ToString()), "x.CFloat < 1.1 || x.CFloat > 11.11 || (x.CString != ABC");
            }

            if (true)
            {
                var actual = Compile<CEntity>((x) => x.CInt < 1.1 || x.CInt > 11.11 || (x.CInt != 1 && x.CInt != 17 ));
                var want = "where ((cint < 1.1) or (cint > 11.11)) or ((cint <> 1) and (cint <> 17))";
                Assert.AreEqual(AUtils.FS(want), AUtils.FS(actual.ToString()), "x.CInt < 1.1 || x.CInt > 11.11 || (x.CInt != 1 && x.CInt != 17");
            }

            if (true)
            {
                int i = 3;
                var actual = Compile<CEntity>((x) => x.CInt > i);
                var want = "where cint > 3";
                Assert.AreEqual(AUtils.FS(want), AUtils.FS(actual.ToString()), "x.CInt > i");
            }

            if (true)
            {
                var actual = Compile<CEntity>((x) => x.CGuid == Guid.Empty);
                var want = "where cguid = 00000000-0000-0000-0000-000000000000";
                Assert.AreEqual(AUtils.FS(want), AUtils.FS(actual.ToString()), "x.CGuid == Guid.Empty");
            }

            if (true)
            {
                var actual = Compile<CEntity>((x) => x.CDateTime == DateTime.Parse("2001-01-01"));
                var want = "where cdatetime = 2001-01-01 00:00:00";
                Assert.AreEqual(AUtils.FS(want), AUtils.FS(actual.ToString()), "x.CDateTime == DateTime.Parse(\"2001-01-01\")");
            }

            if (true)
            {
                var t = new HelperClass();
                var actual = Compile<CEntity>((x) => x.CInt == t.P);
                var want = "where cint = 5";
                Assert.AreEqual(AUtils.FS(want), AUtils.FS(actual.ToString()), "x.CInt == t.P");
            }

            if (true)
            {
                var t = new HelperClass();
                var actual = Compile<CEntity>((x) => x.CInt == t.F);
                var want = "where cint = 13";
                Assert.AreEqual(AUtils.FS(want), AUtils.FS(actual.ToString()), "x.CInt == t.F");
            }

            if (true)
            {
                var t = new HelperClass();
                var actual = Compile<CEntity>((x) => x.CInt == t.M());
                var want = "where cint = 7";
                Assert.AreEqual(AUtils.FS(want), AUtils.FS(actual.ToString()), "x.CInt == t.M()");
            }

            if (true)
            {
                var t = new HelperClass();
                var actual = Compile<CEntity>((x) => x.CInt == t.MArgs(7));
                var want = "where cint = 21";
                Assert.AreEqual(AUtils.FS(want), AUtils.FS(actual.ToString()), "x.CInt == t.MArgs(7)");
            }

           
        }

        private Where Compile<T>(System.Linq.Expressions.Expression<Func<T, bool>> exp)
        {
            LambdaVisitor<CEntity> visitor = new LambdaVisitor<CEntity>();
            return visitor.Compile(exp);
        }
    }



    [DbTable( Name="ttable")]
    public class CEntity
    {
        [DbColumn(IsKey=true, UpdateAble=false, InsertAble=false)]
        public int PK { get; set; }
        
        public bool CBool { get; set; }
        
        public int CInt { get; set; }
        
        public float CFloat { get; set; }

        [DbColumn(Name="CNumeric")]
        public decimal ColNumeric { get; set; }
        
        public string CString { get; set; }
        
        public DateTime CDateTime;
        
        public Guid CGuid;

        public override string ToString()
        {
            return string.Format("Key={0},CBool={1},CInt={2},CFloat={3},ColNumeric={4},CString={5},CDateTime={6},CGuid={7}",
                PK, CBool, CInt, CFloat.ToString("####.0000"), ColNumeric.ToString("####.0000"), CString, CDateTime, CGuid);
        }

        public string Dump()
        {
            return string.Format("CBool={0},CInt={1},CFloat={2},ColNumeric={3},CString={4},CDateTime={5},CGuid={6}",
                CBool, CInt, CFloat.ToString("####.0000"), ColNumeric.ToString("####.0000"), CString, CDateTime, CGuid);
        }

        public static CEntity NewEntity(int i)
        {
            CEntity e = new CEntity();
            e.CGuid = Guid.NewGuid();            
            e.CBool = i > 500;
            e.CInt = i;
            e.CFloat = (float)(i * 1.1);
            e.ColNumeric = (decimal)(i * 2.2);
            e.CString = string.Concat(i, "_", e.CGuid.ToString());
            e.CDateTime = DateTime.Now.AddHours(i);
            return e;
        }

        public static CEntity NewEntity()
        {
            int i = new Random(Guid.NewGuid().GetHashCode()).Next(0, 1000);
            return NewEntity(i);
        }
      
    }

    public class HelperClass
    {
        public int P { get; set; }
        public int F { get; set; }

        public HelperClass()
        {
            this.P = 5;
            this.F = 13;
        }

        public int MArgs(int i)
        {
            return i * 3;
        }

        public int M()
        {
            return 7;
        }
    }
}
