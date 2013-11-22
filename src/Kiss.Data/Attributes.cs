using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Kiss.Data
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
    public class DbTableAttribute : Attribute
    {
        public string Name { get; set; }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class DbProcedureAttribute : Attribute
    {
        public string Name { get; set; }
    }

    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class DbParameterAttribute : Attribute
    {
        public string Name { get; set; }
        //public DbType? DbType { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class DbColumnAttribute : Attribute
    {
        public string Name { get; set; }
        public bool IsKey { get; set; }
        public bool UpdateAble { get; set; }
        public bool InsertAble { get; set; }

        public DbColumnAttribute()
        {
            IsKey = false;
        }
    }

}
