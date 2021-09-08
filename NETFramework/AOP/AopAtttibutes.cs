using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Activation;
using System.Runtime.Remoting.Contexts;
using System.Text;

namespace Framework
{
    /// <summary>
    /// 默认执行特性
    /// </summary>
    /// 
    [AttributeUsage(AttributeTargets.Class)]
    public class DefaultAopAttribute : ContextAttribute
    {
        public DefaultAopAttribute() : base("DefaultAopAttribute")
        {

        }
        public override void GetPropertiesForNewContext(IConstructionCallMessage ctorMsg)
        {
            if (ctorMsg == null) return;
            if (!DefaultAopCache.ContainType(ctorMsg.ActivationType)) return;
            AopContextProperty property = new AopContextProperty();
            ctorMsg.ContextProperties.Add(property);
        }
    }
    /// <summary>
    /// 如果打上此标记,就忽略所有的Aop执行方式
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class AopIgnoreAttribute : Attribute
    {

    }
    /// <summary>
    /// 只有在method上使用的 attribute,才允许操作 aop
    /// 如果 多个AOP在一个 Method上执行,只会保留第一次的返回值,其他的aop可以对返回值进行更改和操作
    /// 如 
    /// [Method(typeof(Handler1, order=1))]
    /// [Method(typeof(Handler2, order=2))]
    /// int A(){  console.write("只会执行一次") return 3;}
    /// Handler2只会对返回值 3进行更改,而不会执行A()本身
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class MethodAttribute : Attribute
    {
        public int Order { get; set; }
        public Type HandlerType { get; }
        public MethodAttribute(Type handlerType)
        {
            if (!typeof(IAopHandler).IsAssignableFrom(handlerType)) throw new ArgumentException("参数错误,请确认");
            HandlerType = handlerType;
        }
    }
    public static class DefaultAopCache
    {
        private static HashSet<Type> m_typeCache = new HashSet<Type>();
        private static HashSet<MethodBase> m_methodCache = new HashSet<MethodBase>();
        static DefaultAopCache()
        {
            var assemblys = AppDomain.CurrentDomain.GetAssemblies();

            //获取程序路径下的DLL
            assemblys = assemblys.Where(x => Path.GetDirectoryName(x.Location).TrimEnd('\\')
                                 .Equals(AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\'), StringComparison.OrdinalIgnoreCase)).ToArray();
            foreach (var asm in assemblys)
            {
                var types = asm.GetTypes().Where(x => x.IsClass && typeof(AopBoundContext).IsAssignableFrom(x));
                foreach (var type in types)//遍历类
                {
                    var xx = type.GetCustomAttributes(true);
                    if (xx.Where(item => typeof(AopIgnoreAttribute) == item.GetType()).Count() != 0) continue;
                    //遍历方法
                    foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))//遍历类中的方法
                    {

                        var all = method.GetCustomAttributes(true);
                        if (all == null || all.Count() == 0) continue;
                        if (all.Where(item => typeof(AopIgnoreAttribute) == item.GetType()).Count() != 0) continue;
                        var methodAttributes = all.Where(item => typeof(MethodAttribute).IsAssignableFrom(item.GetType()));
                        if (methodAttributes == null || methodAttributes.Count() == 0) continue;
                        m_methodCache.Add(method);
                        m_typeCache.Add(type);
                    }
                }
            }
        }
        public static Boolean ContainType(Type type)
        {
            if (type == null) return false;
            return m_typeCache.Contains(type);
        }
        public static Boolean ContainMethod(MethodBase method)
        {
            if (method == null) return false;
            return m_methodCache.Contains(method);
        }
    }
}
