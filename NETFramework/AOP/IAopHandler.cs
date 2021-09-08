using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Contexts;
using System.Text;

namespace Framework
{
    public interface IAopHandler
    {
        /// <summary>
        /// 执行方法前调用
        /// </summary>
        /// <param name="method">方法名</param>
        /// <param name="arguments">传递的参数</param>
        /// <returns>是否执行aop标记的方法</returns>
        Boolean BeforeExecuting(MethodBase method, DefaultAopReturnMessage arguments);
        /// <summary>
        /// 执行方法之后对结果进行处理
        /// </summary>
        /// <param name="method">方法名</param>
        /// <param name="result">返回的结果</param>
        void AfterExecuted(MethodBase method, DefaultAopReturnMessage result);
    }
}
