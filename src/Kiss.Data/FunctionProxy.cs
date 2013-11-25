using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Remoting.Proxies;
using System.Runtime.Remoting.Messaging;
using System.Reflection;
using System.Data;

namespace Kiss.Data
{
    public class FunctionProxy<T> : RealProxy
    {
        private static Type procedureType = typeof(DbProcedureAttribute);
        private static Type parameterType = typeof(DbParameterAttribute);

        public string DbName {get; set; }
        public DbContent DbContent { get; set; }

        private FunctionProxy(string dbName)
            : base(typeof(T))
        {
            this.DbName = dbName;
        }

        private FunctionProxy(DbContent conent)
            : base(typeof(T))
        {
            this.DbContent = conent;
        }


        public static T Create(string dbName)
        {
            if (string.IsNullOrWhiteSpace(dbName))
            {
                throw new ArgumentNullException("dbName");
            }
            return (T)(new FunctionProxy<T>(dbName).GetTransparentProxy());
        }

        public static T Create(DbContent conent)
        {
            return (T)(new FunctionProxy<T>(conent).GetTransparentProxy());
        }

        protected object TryFindAttribute(MemberInfo info, Type attributeType)
        {
            var attributes = info.GetCustomAttributes(attributeType, false);
            if (attributes == null || attributes.Length == 0)
            {
                return null;
            }
            return attributes[0];
        }

        protected object TryFindAttribute(ParameterInfo info, Type attributeType)
        {
            var attributes = info.GetCustomAttributes(attributeType, false);
            if (attributes == null || attributes.Length == 0)
            {
                return null;
            }
            return attributes[0];
        }

        protected string GetProcedureName(IMethodCallMessage method)
        {
            var attribute = TryFindAttribute(method.MethodBase, procedureType) as DbProcedureAttribute;
            if (attribute == null)
            {
                return method.MethodName;
            }
            return attribute.Name;
        }

        protected string GetParameterName(ParameterInfo parameter)
        {
            var attribute = TryFindAttribute(parameter, parameterType) as DbParameterAttribute;
            if (attribute == null)
            {
                return parameter.Name;
            }
            return attribute.Name;
        }

        protected Dictionary<string, object> GetParameters(IMethodCallMessage method)
        {
            Dictionary<string, object> data = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            var parameters = method.MethodBase.GetParameters();
            if (parameters != null)
            {
                for (var i = 0; i < parameters.Length && i< method.Args.Length; i++)
                {
                    var p = parameters[i];
                    var attribute = TryFindAttribute(p, parameterType) as DbParameterAttribute;
                    if (attribute == null)
                    {
                        data.Add(p.Name, method.Args[i]);
                    }
                    else
                    {
                        data.Add(attribute.Name, method.Args[i]);
                    }                    
                }
            }
            return data;
        }

        public override IMessage Invoke(IMessage msg)
        {
            IMethodCallMessage methodCall = (IMethodCallMessage)msg;
            IMethodReturnMessage methodReturn = null;
            //object[] copiedArgs = Array.CreateInstance(typeof(object), methodCall.Args.Length) as object[];
            //methodCall.Args.CopyTo(copiedArgs, 0);

            var procedureName = GetProcedureName(methodCall);
            int outCount = 0;
            Dictionary<string, object> data = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            var parameterInfos = methodCall.MethodBase.GetParameters();
            if (parameterInfos != null)
            {
                for (var i = 0; i < parameterInfos.Length && i < methodCall.Args.Length; i++)
                {
                    var p = parameterInfos[i];
                    var attribute = TryFindAttribute(p, parameterType) as DbParameterAttribute;
                    if (attribute == null)
                    {
                        data.Add(p.Name, methodCall.Args[i]);
                    }
                    else
                    {
                        data.Add(attribute.Name, methodCall.Args[i]);
                    }

                    if (p.IsOut || p.ParameterType.IsByRef)
                    {
                        outCount++;
                    }
                }
            }
            
            IDataReader reader = null;
            IExecuteResult result;

            try
            {
                if (DbContent == null)
                {
                    using (DbContent content = new DbContent(DbName))
                    {
                        reader = content.ProcedureReader(procedureName, Kiss.Core.Adapter.Dictionary(data), out result);
                    }
                }
                else
                {
                    reader = DbContent.ProcedureReader(procedureName, Kiss.Core.Adapter.Dictionary(data), out result);
                }

                if (outCount == 0)
                {
                    methodReturn = new ReturnMessage(reader, null, 0, methodCall.LogicalCallContext, methodCall);
                }
                else
                {
                    object[] outArgs = new object[methodCall.Args.Length];
                    for (int i = 0; i < parameterInfos.Length && i < methodCall.Args.Length; i++)
                    {
                        var p = parameterInfos[i];
                        if (p.IsOut || p.ParameterType.IsByRef)
                        {
                            string parameterName = GetParameterName(parameterInfos[i]);
                            var output = result.Output();
                            if (output.Contains(parameterName))
                            {
                                outArgs[i] = output[parameterName];
                            }
                            else
                            {
                                outArgs[i] = methodCall.Args[i];
                            }
                        }
                    }

                    methodReturn = new ReturnMessage(reader, outArgs, outArgs.Length, methodCall.LogicalCallContext, methodCall);
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    methodReturn = new ReturnMessage(ex.InnerException, methodCall);
                }
                else
                {
                    methodReturn = new ReturnMessage(ex, methodCall);
                }
            }
            finally
            {
                //write log?
            }

            return methodReturn;
        }
    }

    
}
