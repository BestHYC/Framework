using System;
using System.Linq.Expressions;
using System.Reflection;

namespace StandFramework.Helpers
{
    public static class ExpressionHelper
    {

        /// BuildGetPropertyValueExpression("Name")
        /// 编译的 Func 为 Func<T, Object> func = (item) => item.Name;
        public static Func<T, Object> BuildGetPropertyValueExpression<T>(String name)
        {
            Type type = typeof(T);
            PropertyInfo prop = type.GetProperty(name);
            return BuildGetPropertyValueExpression<T>(prop);
        }
        /// <summary>
        /// BuildGetPropertyValueExpression(Name)
        /// 编译的 Func 为 Func<T, Object> func = (item) => item.Name;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Func<T, Object> BuildGetPropertyValueExpression<T>(PropertyInfo str)
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
    }
}
