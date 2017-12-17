// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.ComponentModel.Composition;
#endregion

namespace TLAtuo.Mef.Aop
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class)]
    public class AspectsStatusExportAttribute : ExportAttribute
    {
        public AspectsStatusExportAttribute(Type contractType) : base(contractType) { }

        public bool AreAspectsEnabled { get; set; }
    }
}