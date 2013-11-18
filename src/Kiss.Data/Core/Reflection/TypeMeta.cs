using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using System.Threading;

namespace Kiss.Core.Reflection
{
    public class TypeMeta
    {
        public Type SourceType { get; private set; }
        private List<MemberMeta> members = new List<MemberMeta>();
        private Dictionary<string, int> membersIndex = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        public void SetValue(object instance, string memberName, object value)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            MemberMeta member = GetMember(memberName);
            member.SetValue(instance, value);
        }

        public object GetValue(string memberName, object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            MemberMeta member = GetMember(memberName);
            return member.GetValue(instance);
        }

        public MemberMeta TryGetMember(string memberName)
        {
            int index = GetIndex(memberName);
            if (index >= 0)
            {
                return members[index];
            }
            return null;
        }

        public MemberMeta GetMember(string memberName)
        {
            MemberMeta member = TryGetMember(memberName);
            if (member == null)
            {
                throw new Exception("can not find member:" + memberName);
            }
            return member;
        }

        private int GetIndex(string memberName)
        {
            int index;
            if (membersIndex.TryGetValue(memberName, out index))
            {
                return index;
            }
            return -1;
        }

        public bool Contains(string name)
        {
            return membersIndex.ContainsKey(name);
        }

        public IList<MemberMeta> Members()
        {
            return this.members;
        }

        internal Func<object> creater;

        public object CreateInstance()
        {
            if (creater == null)
            {
                lock (this)
                {
                    if (creater == null)
                    {
                        creater = EmitUtils.BuildDefaultConstructorDelegate(SourceType);
                    }
                }
            }
            return creater();
        }

        public IList<PropertyInfo> Properties()
        {
            return members
                    .Where((x) => x.MetaType == MemberMeta.MemberMetaType.Property)
                    .Cast<PropertyInfo>().ToList();
        }

        public IList<FieldInfo> Fields()
        {
            return members
                    .Where((x) => x.MetaType == MemberMeta.MemberMetaType.Field)
                    .Cast<FieldInfo>().ToList();
        }

        public TypeMeta(Type type)
        {
            this.SourceType = type;
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.Static);
            foreach (PropertyInfo pi in properties)
            {
                MemberMeta meta = new MemberMeta(type, pi);
                members.Add(meta);
                membersIndex[pi.Name] = members.Count - 1;
            }

            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetField | BindingFlags.Static);
            foreach (FieldInfo fi in fields)
            {
                MemberMeta meta = new MemberMeta(type, fi);
                members.Add(meta);
                membersIndex[fi.Name] = members.Count - 1;
            }
        }
    }

}
