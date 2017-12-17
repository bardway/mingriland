// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using Castle.DynamicProxy;
#endregion

namespace TLAtuo.Mef.Aop
{
    public interface IAspectFactory
    {
        IInterceptor GetAroundInvokeApectInvoker { get; }
    }
}