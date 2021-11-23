using System;
using System.Reflection;
using Delegates.Helper;

namespace Delegates.Extensions
{
    internal static class MethodInfoExtensions
    {
        public static TDelegate CreateDelegate<TDelegate>(this MethodInfo method)
            where TDelegate : class
        {
            DelegateHelper.CheckDelegateReturnType<TDelegate>(method);
            return method.CreateDelegate(typeof(TDelegate)) as TDelegate;
        }

        public static Delegate CreateDelegate(this MethodInfo method, Type delegateType)
        {
            return method.CreateDelegate(delegateType);
        }

        public static void IsEventArgsTypeCorrect<TEventArgs>(this MethodInfo method)
        {
            var argsType = method.GetParameters()[0].ParameterType;
            if (argsType != typeof(TEventArgs))
            {
                throw new ArgumentException("Provided event args type '" + typeof(TEventArgs).Name + "' is not compatible with expected type '" + argsType.Name + "'");
            }
        }
    }
}