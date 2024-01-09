using StandFramework.Helpers;
using System;

namespace CoreConsole
{
    public class A
    {
        public A()
        {
            
        }
    }
    class Program
    {

        static void Main(string[] args)
        {
            A a = ExpressionHelper.BuildNewByDefaultFunc<A>()();
            Console.Read();
        }
    }
}
