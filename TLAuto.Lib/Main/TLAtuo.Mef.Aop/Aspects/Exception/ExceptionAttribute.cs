// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
#endregion

namespace TLAtuo.Mef.Aop.Aspects.Exception
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
    public class ExceptionAttribute : Attribute { }
}