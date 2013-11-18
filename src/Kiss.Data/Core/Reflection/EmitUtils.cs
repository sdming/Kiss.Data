using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;

using Kiss.Core;

namespace Kiss.Core.Reflection
{
    public static class EmitUtils
    {
        private const string dynamicEmitNamePrefix = "Kiss_";

        public static void AddInterface(TypeBuilder builder, Type type)
        {
            builder.AddInterfaceImplementation(type);
        }

        public static MethodBuilder GetMethodBuilder(TypeBuilder typeBuilder, MethodInfo method)
        {
            ParameterInfo[] paramInfo = method.GetParameters();
            Type[] paramType = new Type[paramInfo.Length];
            for (int i = 0; i < paramInfo.Length; i++)
            {
                paramType[i] = paramInfo[i].ParameterType;
            }

            MethodBuilder methodBuilder = typeBuilder.DefineMethod(method.Name, MethodAttributes.Public | MethodAttributes.Virtual, method.ReturnType, paramType);
            return methodBuilder;
        }

        public static TypeBuilder BuildInterfaceImplementationBuilder(Type targetType, Type parentType)
        {
            if (!targetType.IsInterface)
            {
                throw new Exception(string.Format("{0} isn't a interface", targetType.FullName));
            }

            AppDomain appDomain = System.Threading.Thread.GetDomain();
            AssemblyName assemblyName = new AssemblyName();
            assemblyName.Name = dynamicEmitNamePrefix + targetType.Name;
            AssemblyBuilder assemblyBuilder = appDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(dynamicEmitNamePrefix + "Module");
            Type[] newTypeInterfaces = new Type[] { targetType }; ;

            TypeBuilder typeBuilder = moduleBuilder.DefineType(dynamicEmitNamePrefix + targetType.Name, TypeAttributes.Public, parentType, newTypeInterfaces);
            typeBuilder.AddInterfaceImplementation(targetType);

            return typeBuilder;
        }

        public static Action<object, T> BuildMemberSet<T>(MemberInfo memberInfo)
        {
            PropertyInfo pi = memberInfo as PropertyInfo;
            DynamicMethod dm = new DynamicMethod("set_" + typeof(T).Name + memberInfo.Name, typeof(void), new Type[] { typeof(object), typeof(T) }, memberInfo.Module);
            ILGenerator il = dm.GetILGenerator();
            ILBuilder builder = new ILBuilder(il);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);

            if (pi != null)
            {
                //builder.Unbox(pi.PropertyType);
                il.Emit(OpCodes.Callvirt, pi.GetSetMethod());
            }
            else
            {
                FieldInfo fi = memberInfo as FieldInfo;
                il.Emit(OpCodes.Stfld, fi);
            }
            il.Emit(OpCodes.Ret);
            return (Action<object, T>)dm.CreateDelegate(typeof(Action<object, T>));
        }

        public static Action<object, object> BuildMemberSetDelegate(MemberInfo memberInfo)
        {
            PropertyInfo pi = memberInfo as PropertyInfo;
            DynamicMethod dm = new DynamicMethod("set_" + memberInfo.Name, typeof(void), new Type[] { typeof(object), typeof(object) }, memberInfo.Module);
            ILGenerator il = dm.GetILGenerator();
            ILBuilder builder = new ILBuilder(il);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);

            if (pi != null)
            {
                builder.Unbox(pi.PropertyType);
                il.EmitCall(OpCodes.Call, pi.GetSetMethod(), null);
            }
            else
            {
                FieldInfo fi = memberInfo as FieldInfo;
                builder.Unbox(fi.FieldType);
                il.Emit(OpCodes.Stfld, fi);
            }
            il.Emit(OpCodes.Ret);
            return (Action<object, object>)dm.CreateDelegate(typeof(Action<object, object>));
        }

        public static Func<object, T> BuildMemberGet<T>(MemberInfo memberInfo)
        {
            PropertyInfo pi = memberInfo as PropertyInfo;
            DynamicMethod dm = new DynamicMethod("get_" + typeof(T).Name + memberInfo.Name, typeof(T), new Type[] { typeof(object) }, memberInfo.Module);
            ILGenerator il = dm.GetILGenerator();
            ILBuilder builder = new ILBuilder(il);
            il.Emit(OpCodes.Ldarg_0);

            if (pi != null)
            {
                il.EmitCall(OpCodes.Call, pi.GetGetMethod(), null);
                //builder.Box(pi.PropertyType);
            }
            else
            {
                FieldInfo fi = memberInfo as FieldInfo;
                il.Emit(OpCodes.Ldfld, fi);
                //builder.Box(fi.FieldType);
            }
            il.Emit(OpCodes.Ret);
            return (Func<object, T>)dm.CreateDelegate(typeof(Func<object, T>));
        }

        public static Func<object, object> BuildMemberGetDelegate(MemberInfo memberInfo)
        {
            PropertyInfo pi = memberInfo as PropertyInfo;
            DynamicMethod dm = new DynamicMethod("get_" + memberInfo.Name, typeof(object), new Type[] { typeof(object) }, memberInfo.Module);
            ILGenerator il = dm.GetILGenerator();
            ILBuilder builder = new ILBuilder(il);
            il.Emit(OpCodes.Ldarg_0);

            if (pi != null)
            {
                il.EmitCall(OpCodes.Call, pi.GetGetMethod(), null);
                builder.Box(pi.PropertyType);
            }
            else
            {
                FieldInfo fi = memberInfo as FieldInfo;
                il.Emit(OpCodes.Ldfld, fi);
                builder.Box(fi.FieldType);
            }
            il.Emit(OpCodes.Ret);
            return (Func<object, object>)dm.CreateDelegate(typeof(Func<object, object>));
        }

        public static Func<object> BuildDefaultConstructorDelegate(Type targetType)
        {
            DynamicMethod dm = new DynamicMethod("new_" + targetType.Name, typeof(object), Type.EmptyTypes, targetType.Module);
            ILGenerator il = dm.GetILGenerator();
            ILBuilder builder = new ILBuilder(il);
            il.Emit(OpCodes.Newobj, targetType.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null));
            il.Emit(OpCodes.Ret);
            return (Func<object>)dm.CreateDelegate(typeof(Func<object>));
        }

    }

}
