using StandFramework.Helpers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;

namespace Framework
{
    /// <summary>
    /// Aop执行
    /// </summary>
    public class AopMessageSink : IMessageSink
    {
        public IMessageSink NextSink { get; }
        public AopMessageSink(IMessageSink next)
        {
            NextSink = next;
        }
        public static void Init()
        {

        }
        /// <summary>
        /// 异步操作
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="replySink"></param>
        /// <returns></returns>
        public IMessageCtrl AsyncProcessMessage(IMessage msg, IMessageSink replySink)
        {
            return NextSink.AsyncProcessMessage(msg, replySink);
        }
        /// <summary>
        /// 同步执行数据
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public IMessage SyncProcessMessage(IMessage msg)
        {
            var message = msg as IMethodMessage;
            if (message == null) return NextSink.SyncProcessMessage(msg);
            var attributes = message.MethodBase.GetCustomAttributes(true)?.Where(item => typeof(MethodAttribute).IsAssignableFrom(item.GetType()));
            if (attributes == null || attributes.Count() == 0) return NextSink.SyncProcessMessage(msg);
            var orderByAttibutes = attributes.OrderBy(item => (item as MethodAttribute).Order);
            DefaultAopReturnMessage defaultMessage = new DefaultAopReturnMessage(message);
            Boolean isExecuted = true;
            foreach (var item in orderByAttibutes)
            {
                MethodAttribute attribue = item as MethodAttribute;
                Func<Object> func;
                if (!m_func.TryGetValue(attribue.HandlerType, out func))
                {
                    func = ExpressionHelper.BuildNewByDefaultFunc(attribue.HandlerType);
                    m_func.TryAdd(attribue.HandlerType, func);
                }
                var hanlder = func() as IAopHandler;
                if (hanlder == null) return NextSink.SyncProcessMessage(msg);
                defaultMessage.MethodAttribute = attribue;
                if (hanlder.BeforeExecuting(message.MethodBase, defaultMessage) && isExecuted)
                {

                    var returnmessage = NextSink.SyncProcessMessage(msg) as IMethodReturnMessage;
                    defaultMessage.OutArgs = returnmessage.OutArgs;
                    defaultMessage.OutArgCount = returnmessage.OutArgCount;
                    defaultMessage.ReturnValue = returnmessage.ReturnValue;
                }
                isExecuted = false;
                hanlder.AfterExecuted(message.MethodBase, defaultMessage);
            }
            return defaultMessage;
        }
        private static ConcurrentDictionary<Type, Func<Object>> m_func = new ConcurrentDictionary<Type, System.Func<Object>>();
    }
}
