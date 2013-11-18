using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using System.Globalization;

namespace Kiss.Core.Reflection
{
    /// <summary>
    /// IQToolkit
    /// </summary>
    public class ILBuilder
    {
        private ILGenerator ilGen;

        public ILBuilder(ILGenerator ilGen)
        {
            this.ilGen = ilGen;
        }

        public void Ret()
        {
            this.ilGen.Emit(OpCodes.Ret);
        }

        public void Add()
        {
            this.ilGen.Emit(OpCodes.Add);
        }

        public void And()
        {
            this.ilGen.Emit(OpCodes.And);
        }

        public void Box(Type type)
        {
            if (type.IsValueType)
            {
                this.ilGen.Emit(OpCodes.Box, type);
            }
        }

        public void Unbox(Type type)
        {
            if (type.IsValueType)
            {
                this.ilGen.Emit(OpCodes.Unbox_Any, type);
            }
            else if (type != typeof(object))
            {
                this.ilGen.Emit(OpCodes.Castclass, type);
            }
        }

        public void Castclass(Type target)
        {
            this.ilGen.Emit(OpCodes.Castclass, target);
        }

        public void Call(ConstructorInfo ctor)
        {
            this.ilGen.Emit(OpCodes.Call, ctor);
        }

        public void Call(MethodInfo methodInfo)
        {
            if (methodInfo.IsVirtual)
            {
                this.ilGen.Emit(OpCodes.Callvirt, methodInfo);
            }
            else if (methodInfo.IsStatic)
            {
                this.ilGen.Emit(OpCodes.Call, methodInfo);
            }
            else
            {
                this.ilGen.Emit(OpCodes.Call, methodInfo);
            }
        }

        public LocalBuilder DeclareLocal(Type type, string name)
        {
            return this.DeclareLocal(type, name, false);
        }

        public LocalBuilder DeclareLocal(Type type, string name, bool pinned)
        {
            LocalBuilder lb = this.ilGen.DeclareLocal(type, pinned);
            return lb;
        }

        public void Ldarg(int slot)
        {
            switch (slot)
            {
                case 0:
                    this.ilGen.Emit(OpCodes.Ldarg_0);
                    return;

                case 1:
                    this.ilGen.Emit(OpCodes.Ldarg_1);
                    return;

                case 2:
                    this.ilGen.Emit(OpCodes.Ldarg_2);
                    return;

                case 3:
                    this.ilGen.Emit(OpCodes.Ldarg_3);
                    return;
            }
            if (slot <= 0xff)
            {
                this.ilGen.Emit(OpCodes.Ldarg_S, slot);
            }
            else
            {
                this.ilGen.Emit(OpCodes.Ldarg, slot);
            }
        }

        public LocalBuilder DeclareLocal(Type type, string name,  bool pinned, object initialValue)
        {
            LocalBuilder lb = this.DeclareLocal(type, name, pinned);
            if (initialValue == null)
            {
                this.ilGen.Emit(OpCodes.Ldnull);
            }
            else
            {
                this.Ldc(initialValue);
            }
            this.ilGen.Emit(OpCodes.Stloc, lb);
            return lb;
        }

        public void Ldtoken(Type value)
        {
            this.ilGen.Emit(OpCodes.Ldtoken, value);
        }

        public void Ldstr(string value)
        {
            this.ilGen.Emit(OpCodes.Ldstr, value);
        }

        public void EmitCast(Type type)
        {
            if (type.IsValueType)
            {
                this.ilGen.Emit(OpCodes.Unbox_Any, type);
            }
            else
            {
                this.ilGen.Emit(OpCodes.Castclass, type);
            }
        }

        public void Ldc(bool value)
        {
            if (value)
            {
                this.ilGen.Emit(OpCodes.Ldc_I4_1);
            }
            else
            {
                this.ilGen.Emit(OpCodes.Ldc_I4_0);
            }
        }

        public void Ldc(double value)
        {
            this.ilGen.Emit(OpCodes.Ldc_R8, value);
        }

        public void Ldc(int value)
        {
            switch (value)
            {
                case -1:
                    this.ilGen.Emit(OpCodes.Ldc_I4_M1);
                    return;
                case 0:
                    this.ilGen.Emit(OpCodes.Ldc_I4_0);
                    return;
                case 1:
                    this.ilGen.Emit(OpCodes.Ldc_I4_1);
                    return;
                case 2:
                    this.ilGen.Emit(OpCodes.Ldc_I4_2);
                    return;
                case 3:
                    this.ilGen.Emit(OpCodes.Ldc_I4_3);
                    return;
                case 4:
                    this.ilGen.Emit(OpCodes.Ldc_I4_4);
                    return;
                case 5:
                    this.ilGen.Emit(OpCodes.Ldc_I4_5);
                    return;
                case 6:
                    this.ilGen.Emit(OpCodes.Ldc_I4_6);
                    return;
                case 7:
                    this.ilGen.Emit(OpCodes.Ldc_I4_7);
                    return;
                case 8:
                    this.ilGen.Emit(OpCodes.Ldc_I4_8);
                    return;
            }

            if (value > -129 && value < 128)
            {
                this.ilGen.Emit(OpCodes.Ldc_I4_S, (SByte)value);
            }
            else
            {
                this.ilGen.Emit(OpCodes.Ldc_I4, value);
            }
        }

        public void Ldc(long value)
        {
            this.ilGen.Emit(OpCodes.Ldc_I8, value);
        }

        public void Ldc(object value)
        {
            Type enumType = value.GetType();
            if (value is Type)
            {
                this.Ldtoken((Type)value);
                //this.Call(GetTypeFromHandle);
            }
            else if (enumType.IsEnum)
            {
                this.Ldc(((IConvertible)value).ToType(Enum.GetUnderlyingType(enumType), null));
            }
            else
            {
                switch (Type.GetTypeCode(enumType))
                {
                    case TypeCode.Boolean:
                        this.Ldc((bool)value);
                        return;

                    case TypeCode.Char:
                    //throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SR.GetString("CharIsInvalidPrimitive")));

                    case TypeCode.SByte:
                    case TypeCode.Byte:
                    case TypeCode.Int16:
                    case TypeCode.UInt16:
                        this.Ldc(((IConvertible)value).ToInt32(CultureInfo.InvariantCulture));
                        return;

                    case TypeCode.Int32:
                        this.Ldc((int)value);
                        return;

                    case TypeCode.UInt32:
                        this.Ldc((int)((uint)value));
                        return;

                    case TypeCode.Int64:
                        this.Ldc((long)value);
                        return;

                    case TypeCode.UInt64:
                        this.Ldc((long)((ulong)value));
                        return;

                    case TypeCode.Single:
                        this.Ldc((float)value);
                        return;

                    case TypeCode.Double:
                        this.Ldc((double)value);
                        return;

                    case TypeCode.Decimal:
                        //this.Ldc((double)value);
                        this.Ldc(((IConvertible)value).ToDecimal(CultureInfo.InvariantCulture));
                        return;

                    case TypeCode.String:
                        this.Ldstr((string)value);
                        return;
                }
                throw new Exception("UnknownConstantType");
            }
        }

        public void NewArray(Type elementType, int len)
        {
            this.Load(len);
            this.ilGen.Emit(OpCodes.Newarr, elementType);
        }

        public void Load(object value)
        {
            if (value == null)
            {
                this.ilGen.Emit(OpCodes.Ldnull);
            }
            else
            {
                this.Ldc(value);
            }
        }

        public void Ldc(float value)
        {
            this.ilGen.Emit(OpCodes.Ldc_R4, value);
        }
    }
}
