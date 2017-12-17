// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Linq;

using Castle.DynamicProxy;
#endregion

namespace TLAtuo.Mef.Aop
{
    public class AspectProxy
    {
        public static object Factory(object obj)
        {
            var generator = new ProxyGenerator();
            var attribs = obj.GetType().GetCustomAttributes(typeof(IAspectFactory), true);
            var interceptors = (from x in attribs
                                select ((IAspectFactory)x).GetAroundInvokeApectInvoker).ToArray();
            var proxy = generator.CreateClassProxy(obj.GetType(), interceptors);
            return proxy;
        }
    }
}