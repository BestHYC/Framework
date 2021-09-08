using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;

namespace Framework
{
    public class DefaultAopReturnMessage : IMethodReturnMessage
    {
        public DefaultAopReturnMessage(IMethodMessage message)
        {
            MethodName = message.MethodName;
            TypeName = message.TypeName;
            MethodSignature = message.MethodSignature;
            ArgCount = message.ArgCount;
            HasVarArgs = message.HasVarArgs;
            Args = message.Args;
            LogicalCallContext = message.LogicalCallContext;
            MethodBase = message.MethodBase;
            Properties = message.Properties;
        }
        public MethodAttribute MethodAttribute { get; set; }
        public int OutArgCount { get; set; }

        public object[] OutArgs { get; set; }

        public Exception Exception { get; set; }

        public object ReturnValue { get; set; }

        public string Uri { get; set; }

        public string MethodName { get; set; }

        public string TypeName { get; set; }

        public object MethodSignature { get; set; }

        public int ArgCount { get; set; }

        public object[] Args { get; set; }

        public bool HasVarArgs { get; set; }

        public LogicalCallContext LogicalCallContext { get; set; }

        public MethodBase MethodBase { get; set; }

        public System.Collections.IDictionary Properties { get; set; }

        public object GetArg(int argNum)
        {
            throw new NotImplementedException();
        }

        public string GetArgName(int index)
        {
            throw new NotImplementedException();
        }

        public object GetOutArg(int argNum)
        {
            throw new NotImplementedException();
        }

        public string GetOutArgName(int index)
        {
            throw new NotImplementedException();
        }
    }
}
