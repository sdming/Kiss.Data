using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Kiss.Core.Reflection
{
    public static class TypeMetaFactory
    {
        private static Dictionary<Type, TypeMeta> typeMetaCache = new Dictionary<Type, TypeMeta>();
        private static readonly object syncLock = new object();

        public static TypeMeta CreateTypeMeta(Type type)
        {
            var key = type;
            TypeMeta result;

            if(typeMetaCache.TryGetValue(key, out result))
            {
                return result;
            }

            lock (syncLock)
            {
                if (typeMetaCache.ContainsKey(key))
                {
                    return typeMetaCache[key];
                }

                result = new TypeMeta(type);
                typeMetaCache[key] = result;                
            }
            return result;
        }
    }
}
