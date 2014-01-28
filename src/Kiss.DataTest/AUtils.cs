using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using Kiss.Core.Reflection;

namespace Kiss.DataTest
{
    /// <summary>
    /// AUtils
    /// </summary>
    internal class AUtils
    {
        public static bool EqualsDateTime(DateTime v1, DateTime v2)
        {
            return v1.ToString("yyyy-MM-dd HH:mm:ss") == v2.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static bool EqualsDouble(double v1, double v2)
        {
            return (v1 - v2) < 0.0001; 
        }

        public static bool EqualsTo(object v1, object v2)
        {
            if (v1 == v2)
            {
                return true;
            }

            if( (v1 == null && v2 != null) || (v1 != null && v2 == null))
            {
                return false;
            }

            IConvertible c = v1 as IConvertible;
            if (c != null)
            {
                v1 = System.Data.Linq.DBConvert.ChangeType(v1, v2.GetType());
            }
            if (Convert.ToString(v1).Equals(Convert.ToString(v2), StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (v1 is DateTime || v2 is DateTime)
            {
                return EqualsDateTime(Convert.ToDateTime(v1), Convert.ToDateTime(v2));
            }

            if (TypeSystem.IsArithmetic(v1.GetType()) || TypeSystem.IsArithmetic(v1.GetType()))
            {
                return EqualsDouble(Convert.ToDouble(v1), Convert.ToDouble(v2));
            }

            return v1 == v2;
        }

        public static string FS(string s)
        {
            if(s == null)
            {
                return s;
            }

            Regex rgx = new Regex(@"\s");
            s = rgx.Replace(s, "");
            return s.ToLower().Trim();
        }

        public static string[] IgnoreList = new string[] {"mysql","sqlite","oracle", "postgres" };

        public static void RunAllTypes(TextWriter writer)
        {
            var a = System.Reflection.Assembly.GetExecutingAssembly();
            var types = a.GetTypes();
            foreach (var t in types)
            {
                if (!t.IsClass || t.IsAbstract)
                {
                    continue;
                }
                if (!t.Name.StartsWith("Test"))
                {
                    continue;
                }

                bool ignore = false;
                foreach (var s in IgnoreList)
                {
                    if (t.Name.ToLowerInvariant().Contains(s.ToLowerInvariant()))
                    {
                        ignore = true;
                        break;
                    }
                }
                if (ignore)
                {
                    continue;
                }
            
                Console.WriteLine("run:{0}", t.Name);

                writer.WriteLine("\r\nType: {0} ", t.Name);
                writer.WriteLine("===  ");
                Object instance = Activator.CreateInstance(t);
                RunAllMethods(writer, instance);
                writer.WriteLine("\t");
            }

        }

        public static void RunAllMethods(TextWriter writer, object obj)
        {
            var type = obj.GetType();
            //writer.WriteLine("run tests: {0} ", type.Name);

            var methods = type.GetMethods();
            foreach (var m in methods)
            {
                if(!m.Name.StartsWith("Test"))
                {
                    continue;
                }
                try
                {
                    m.Invoke(obj, new object[0]);
                    writer.WriteLine("* sucess: {0} ", m.Name);
                }
                catch (Exception ex)
                {
                    writer.WriteLine("* error: {0} \t{1} ", m.Name, ex.Message);
                }
            }
        }

    }
}
