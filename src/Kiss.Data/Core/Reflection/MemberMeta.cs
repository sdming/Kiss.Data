using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using System.Threading;

namespace Kiss.Core.Reflection
{
    public class MemberMeta
    {
        //public enum MemberMetaType
        //{
        //    Property = 0,
        //    Field = 1,
        //    Event = 2,
        //    Method = 3,
        //}

        public string Name { get; internal set; }
        public int Index { get; internal set; }
        public MemberInfo MemberInfo { get; internal set; }
        public Type MemberType { get; internal set; }
        public TypeCode MemberTypeCode { get; internal set; }
        public bool CanRead { get; internal set; }
        public bool CanWrite { get; internal set; }
        public bool IsNullable { get; internal set; }
        public bool IsNullAssignable { get; internal set; }
        internal MemberTypes MetaType { get; set; }

        internal Action<object, object> setter;
        internal Func<object, object> getter;
        internal Delegate setterT;
        internal Delegate getterT;

        internal MemberMeta(Type objectType, MemberInfo member)
        {
            this.Name = member.Name;
            this.MemberInfo = member;
            this.MemberType = TypeSystem.GetMemberType(member);
            this.MemberTypeCode = Type.GetTypeCode(MemberType);
            this.IsNullable = TypeSystem.IsNullableType(this.MemberType);
            this.IsNullAssignable = TypeSystem.IsNullAssignable(this.MemberType);

            FieldInfo fi = member as FieldInfo;
            if (fi != null)
            {
                this.MetaType = MemberTypes.Field;
                CanRead = fi.IsPublic;
                CanWrite = fi.IsPublic && !TypeSystem.IsReadOnly(member);
            }
            else
            {
                PropertyInfo pi = member as PropertyInfo;
                this.MetaType = MemberTypes.Property;
                MethodInfo getMethod = pi.GetGetMethod();
                MethodInfo setMethod = pi.GetSetMethod();
                CanRead = pi.CanRead && getMethod != null && pi.GetGetMethod().IsPublic;
                CanWrite = pi.CanWrite && setMethod != null && setMethod.IsPublic;
            }
        }

        public void SetValue<T>(object instance, T value)
        {
            if (setterT == null)
            {
                lock (this)
                {
                    if (setterT == null)
                    {
                        setterT = EmitUtils.BuildMemberSet<T>(MemberInfo);
                    }
                }
            }
            (setterT as Action<object, T>)(instance, value);
        }

        public void SetValue(object instance, object value)
        {
            if (instance == null || !CanWrite)
            {
                return;
            }
            if (setter == null)
            {
                lock (this)
                {
                    setter = EmitUtils.BuildMemberSetDelegate(MemberInfo);
                }
            }
            setter(instance, value);
        }

        public object GetValue(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            if (getter == null)
            {
                lock (this)
                {
                    if (getter == null)
                    {
                        getter = EmitUtils.BuildMemberGetDelegate(MemberInfo);
                    }
                }
            }
            return getter(instance);
        }

        public T GetValue<T>(object instance)
        {
            if (getterT == null)
            {
                lock (this)
                {
                    if (getterT == null)
                    {
                        getterT = EmitUtils.BuildMemberGet<T>(MemberInfo);
                    }
                }
            }
            return (getterT as Func<object, T>)(instance);
        }
    }
}
