using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kiss.Data;

namespace Kiss.DataTest.Entity
{


    [DbTable(Name = "ttable")]
    public class CEntity
    {
        [DbColumn(IsKey = true, UpdateAble = false, InsertAble = false)]
        public int PK { get; set; }

        public bool CBool { get; set; }

        public int CInt { get; set; }

        public float CFloat { get; set; }

        [DbColumn(Name = "CNumeric")]
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
            e.CFloat = (float)(i * 1.01);
            e.ColNumeric = (decimal)(i * 2.02);
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
