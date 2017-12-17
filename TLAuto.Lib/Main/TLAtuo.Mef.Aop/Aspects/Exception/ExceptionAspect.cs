// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;

using Castle.DynamicProxy;
#endregion

namespace TLAtuo.Mef.Aop.Aspects.Exception
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ExceptionAspect : Attribute, IAspectFactory
    {
        public IInterceptor GetAroundInvokeApectInvoker { get; } = new ExceptionInterceptor();
    }
}