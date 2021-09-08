using Framework;
using StandFramework.Helpers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NetConsole.AOPTest
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class CacheAttribute:MethodAttribute
    {
        /// <summary>
        /// 限制
        /// 1.类中需有其参数一模一样的非三地部署的方法,且返回值为IEnumerable<Object>,
        /// 2.此方法不要缓存,否则直接取缓存的值,不会改变
        /// 3.此方法 返回值 为集合,否则 三地获取到的值无法添加
        /// </summary>
        public String MethodName { get; set; }
        public String Name { get; set; }
        public CacheAttribute() : base(typeof(CacheHandler))
        {

        }
    }
    public class CacheHandler : IAopHandler
    {
        public void AfterExecuted(MethodBase method, DefaultAopReturnMessage result)
        {
        }

        public bool BeforeExecuting(MethodBase method, DefaultAopReturnMessage arguments)
        {
            Func<Object, Object> func;
            if(!m_func.TryGetValue(method.ReflectedType,out func))
            {
                func = ExpressionHelper.BuildNewFunc(method.ReflectedType, typeof(String));
            }
            var item = arguments.MethodAttribute as CacheAttribute;
            var methodInvoke = method.ReflectedType.GetMethod(item.MethodName);
            IEnumerable<Object> str1 = methodInvoke.Invoke(func("T1"), arguments.Args) as IEnumerable<Object>;
            IEnumerable<Object> str2 = methodInvoke.Invoke(func("T2"), arguments.Args) as IEnumerable<Object>;
            var returnResult = str1.Union(str2);
            arguments.ReturnValue = returnResult;
            return false;
        }
        private static ConcurrentDictionary<Type, Func<Object, Object>> m_func = new ConcurrentDictionary<Type, Func<object, object>>();
    }
}
