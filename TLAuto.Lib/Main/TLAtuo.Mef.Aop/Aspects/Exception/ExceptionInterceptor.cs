// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;

using Castle.Core.Internal;
using Castle.DynamicProxy;
#endregion

namespace TLAtuo.Mef.Aop.Aspects.Exception
{
    public class ExceptionInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            if (invocation.Method.HasAttribute<ExceptionAttribute>())
            {
                try
                {
                    invocation.Proceed();
                }
                catch (System.Exception ex)
                {
                    var currentColor = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Red;

                    Console.WriteLine("Error:");
                    Console.WriteLine(ex.InnerException?.Message);
                    Console.WriteLine();

                    Console.ForegroundColor = currentColor;
                    if (invocation.Method.ReturnType != Type.GetType("System.Void"))
                    {
                        invocation.ReturnValue = CreateTypeDefaultValue(invocation.Method.ReturnType);
                    }
                }
            }
            else
            {
                invocation.Proceed();
            }
        }

        private object CreateTypeDefaultValue(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }

            return null;
        }
    }
}