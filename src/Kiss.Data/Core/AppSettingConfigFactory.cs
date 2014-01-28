using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Kiss.Data.Core
{
    public class AppSettingConfigFactory
    {
        public static int GetInt32(string name, int defaultValue)
        {
            var v = System.Configuration.ConfigurationManager.AppSettings[name];
            if (string.IsNullOrEmpty(v))
            {
                return defaultValue;
            }
            int i;
            if (Int32.TryParse(v, out i))
            {
                return i;
            }
            return defaultValue;

        }

        public static string GetString(string name, string defaultValue)
        {
            var v = System.Configuration.ConfigurationManager.AppSettings[name];
            if (v == null)
            {
                return defaultValue;
            }
            return v;

        }

        private static object ChangeType(object value, Type conversionType)
        {
            return Convert.ChangeType(value, conversionType);
        }

        private static bool IsReadOnly(MemberInfo member)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    FieldInfo fi = (FieldInfo)member;
                    return (fi.Attributes & FieldAttributes.InitOnly) != 0 || !fi.IsPublic;
                case MemberTypes.Property:
                    PropertyInfo pi = (PropertyInfo)member;
                    return !pi.CanWrite || pi.GetSetMethod() == null || !pi.GetSetMethod().IsPublic;
                default:
                    return true;
            }
        }

        public static T Create<T>() where T : new()
        {
            return Create<T>(System.Configuration.ConfigurationManager.AppSettings);
        }

        public static T Create<T>(System.Collections.Specialized.NameValueCollection source) where T : new()
        {
            T data = new T();
            Type type = typeof(T);
            string prefix = type.Name;

            var properties = type.GetProperties();
            foreach (var p in properties)
            {
                if (IsReadOnly(p))
                {
                    continue;
                }
                string name = string.Format("{0}:{1}", prefix, p.Name);
                var value = source[name];
                if (value == null)
                {
                    continue;
                }

                object v = null;
                try
                {
                    v = ChangeType(value, p.PropertyType);
                    p.SetValue(data, v, null);
                }
                catch
                {
                    //?? log?
                }
            }

            var fields = type.GetFields();
            foreach (var f in fields)
            {
                if (IsReadOnly(f))
                {
                    continue;
                }
                string name = string.Format("{0}:{1}", prefix, f.Name);
                var value = source[name];
                if (value == null)
                {
                    continue;
                }

                object v = null;
                try
                {
                    v = ChangeType(value, f.FieldType);
                    f.SetValue(data, v);
                }
                catch
                {
                    //?? log?
                }
            }

            return data;

        }
    }
}
