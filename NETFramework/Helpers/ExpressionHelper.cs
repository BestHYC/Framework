using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace StandFramework.Helpers
{
    public static class ExpressionHelper
    {

        /// BuildGetPropertyValueExpression("Name")
        /// 编译的 Func 为 Func<T, Object> func = (item) => item.Name;
        public static Func<T, Object> BuildGetPropertyValueFunc<T>(String name)
        {
            Type type = typeof(T);
            PropertyInfo prop = type.GetProperty(name);
            return BuildGetPropertyValueFunc<T>(prop);
        }
        /// <summary>
        /// BuildGetPropertyValueExpression(Name)
        /// 编译的 Func 为 Func<T, Object> func = (item) => item.Name;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Func<T, Object> BuildGetPropertyValueFunc<T>(PropertyInfo str)
        {
            Type type = typeof(T);
            ParameterExpression param = Expression.Parameter(type, "c");
            MemberExpression left = Expression.Property(param, str);
            var convertExpression = Expression.Convert(left, typeof(Object));
            var lambdaExpression = Expression.Lambda<Func<T, Object>>(convertExpression, param);
            return lambdaExpression.Compile();
        }
        /// <summary>
        /// BuildSetPropertyValueAction(Name)
        /// 编译的 Action 为 Action<T, Object> action = (item, val) => item.Name = val;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Action<T, Object> BuildSetPropertyValueAction<T>(String name)
        {
            var property = typeof(T).GetProperty(name);
            var target = Expression.Parameter(typeof(T));
            var propertyValue = Expression.Parameter(typeof(Object));
            var castPropertyValue = Expression.Convert(propertyValue, property.PropertyType);
            var setPropertyValue = Expression.Call(target, property.GetSetMethod(), castPropertyValue);
            return Expression.Lambda<Action<T, Object>>(setPropertyValue, target, propertyValue).Compile();
        }
        /// <summary>
        /// 通过Type 获取创建此Type的新对象
        /// Func<A> a = () => new A();
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Func<T> BuildNewByDefaultFunc<T>()
        {
            Type type = typeof(T);
            var constructor = type.GetConstructor(new Type[0]);
            var constructorCall = Expression.New(constructor);
            return Expression.Lambda<Func<T>>(constructorCall).Compile();
        }

        /// <summary>
        /// 通过Type 对象创建 Object对象
        /// Type a = typeof(A);
        /// BuildNewByDefaultFunc(a);
        /// Func<Object> func = ()=>new A(); 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Func<Object> BuildNewByDefaultFunc(Type type)
        {
            var constructor = type.GetConstructor(new Type[0]);
            var constructorCall = Expression.New(constructor);
            return Expression.Lambda<Func<Object>>(constructorCall).Compile();
        }
        public static Func<Object, Object> BuildNewFunc(Type type, params Type[] param)
        {
            List<Type> argumentTypes = new List<Type>();
            if (param != null)
            {
                foreach (var t in param)
                {
                    argumentTypes.Add(t);
                }
            }
            List<ParameterExpression> expressions = new List<ParameterExpression>();
            List<UnaryExpression> parameters = new List<UnaryExpression>();
            for (int i = 0; i < argumentTypes.Count; i++)
            {
                var paramExpress = Expression.Parameter(typeof(object), string.Concat("param", i));
                expressions.Add(paramExpress);
                parameters.Add(Expression.Convert(paramExpress, argumentTypes[i]));
            }
            var constructor = type.GetConstructor(argumentTypes.ToArray());
            var constructorCall = Expression.New(constructor, parameters);
            return Expression.Lambda<Func<Object, Object>>(constructorCall, expressions).Compile();
        }
    }
}
